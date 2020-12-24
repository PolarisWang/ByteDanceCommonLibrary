using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace ByteDance.Foundation
{
    using GroupID = System.Int32;
    using TItemIndex = System.Int32;
    using TGroupIndex = System.Int32;

    /// <summary>
    /// 用于快速循环遍历的,分组的，性能平滑的稀疏数组
    /// 特点：
    ///     容量受限（但可手动扩容量）
    ///     快速索引 : O(1) 
    ///     快速插入/删除元素 ：O(1)
    ///     快速寻找空闲索引 ：O(1)
    ///     快速单向循环遍历 : O(N),实际为O(1)*N,N为有效元素个数
    /// 用途：
    ///     本类追求较快的遍历速度，适用于有大量元素、遍历操作很多、增删操作也很多，还需快速索引定位的场合。
    ///     本类使用额外的内存空间记录内部状态。
    ///     如果元素量较小，本类并不合适。
    /// 注意：
    ///     遍历是循环进行的，由使用者根据业务需要终止遍历。
    ///     元素遍历的次序取决于元素加入的次序，如同循环队列。   
    /// 线程安全：
    ///     不支持多线程。
    /// 实现：
    ///     在预先分配的内存空间内实现循环队列的功能，以避免频繁分配内存，但队列的总容量有限。
    ///     将元素进行有序排列(放在数组中)，以提供按索引访问。
    ///     将数据容量与遍历器做在不同的类中，以实现功能细化。
    ///     所有的元素分组管理，每元素在且仅在某个组。可以将元素移到新的组。可以对组内的元素进行遍历。
    ///     初始化后，所有元素在缺省组(组0)中。
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class MultiCyclicQueue<TItem> : IDisposable
    {
        #region definaton
        public const int InvalidIndex = int.MaxValue;
        internal const int MaxCapacity = int.MaxValue - 1;
        internal const GroupID DefaultGroup = 0;

        private struct ItemFlag
        {
            public GroupID Group;
            public TItemIndex NextIndex;
            public TItemIndex PrevIndex;
        }

        private struct ItemInfo
        {
            public ItemFlag Flag;
            public TItem Value;
        }

        private struct GroupInfo
        {
            public GroupInfo(TItemIndex entrance, TItemIndex itemCount)
            {
                Entrance = entrance;
                ItemCount = itemCount;
            }

            public TItemIndex Entrance;
            public TItemIndex ItemCount;
        }
        #endregion

        #region encapsule
        // 元素数据
        private ExposedList<ItemInfo> _Items;
        private TItemIndex _ItemCount;

        // 分组管理信息
        private ExposedList<GroupInfo> _GroupInfos;
        private GroupID _GroupCount;

        //遍历锁，在遍历过程中将本容器标识对锁定状态，以防止元素移动
        private int _LockDepth;
        #endregion

        #region constructor

        public MultiCyclicQueue(TItemIndex capacity, GroupID groupCount)
        {
            _ItemCount = capacity;
            _LockDepth = 0;
            _GroupCount = groupCount;

            Assert.AssertTrue(capacity > 0, "Capacity cannot be zero");
            _Items = new ExposedList<ItemInfo>(_ItemCount);
            for (int index = 0; index < _ItemCount; index++)
                _Items.Add(new ItemInfo());

            _GroupInfos = new ExposedList<GroupInfo>(groupCount);
            for (int index = 0; index < groupCount; index++)
                _GroupInfos.Add(new GroupInfo(InvalidIndex, 0));

            SetAllItemGroup(DefaultGroup);
        }

        ~MultiCyclicQueue()
        {
            _Items.Clear();
            _GroupInfos.Clear();
        }

        #endregion

        #region private methods
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        private bool IsLockedQueue() { return _LockDepth != 0; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        private bool IsValidIndex(TItemIndex index) { return index < _ItemCount; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        private bool IsValidGroup(GroupID groupId) { return (groupId < _GroupCount); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        private ItemFlag GetItemFlag(TItemIndex index)
        {
            Assert.AssertTrue(IsValidIndex(index));
            return _Items.Items[index].Flag;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        private TItem GetItemValue(TItemIndex index)
        {
            Assert.AssertTrue(IsValidIndex(index));
            return _Items.Items[index].Value;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        private void SetGroupItemCount(GroupID groupId, TItemIndex itemCount)
        {
            Assert.AssertTrue(IsValidGroup(groupId));
            _GroupInfos.Items[groupId].ItemCount = itemCount;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        private void SetGroupEntrance(GroupID groupId, TItemIndex entrance)
        {
            Assert.AssertTrue(IsValidGroup(groupId));
            _GroupInfos.Items[groupId].Entrance = entrance;
        }

        private void SetAllItemGroup(GroupID groupId)
        {
            Assert.AssertTrue(!IsLockedQueue());
            for (TItemIndex i = 0; i < _ItemCount; i++)
            {
                _Items.Items[i].Flag.NextIndex = i + 1;
                _Items.Items[i].Flag.PrevIndex = i - 1;
                _Items.Items[i].Flag.Group = groupId;
            }

            TItemIndex last = _ItemCount - 1;
            TItemIndex first = 0;
            Assert.AssertTrue(IsValidIndex(first));
            _Items.Items[first].Flag.PrevIndex = last;
            Assert.AssertTrue(IsValidIndex(last));
            _Items.Items[last].Flag.NextIndex = first;

            // clear group info
            for (TGroupIndex i = 0; i < _GroupCount; i++)
            {
                _GroupInfos.Items[i] = new GroupInfo(InvalidIndex, 0);
            }

            SetGroupItemCount(groupId, _ItemCount);
            SetGroupEntrance(groupId, first);
        }

        private void RemoveFromGroup(TItemIndex index)
        {
            TItemIndex prev = GetItemFlag(index).PrevIndex;
            TItemIndex next = GetItemFlag(index).NextIndex;
            _Items.Items[prev].Flag.NextIndex = next;
            _Items.Items[next].Flag.PrevIndex = prev;
            GroupID groupId = GetItemGroup(index);
            if (GetGroupEntrance(groupId) == index)
            {
                _GroupInfos.Items[groupId].Entrance = next;
                if (next == index)
                    _GroupInfos.Items[groupId].Entrance = InvalidIndex;
            }

            SetGroupItemCount(groupId, GetGroupItemCount(groupId) - 1);
        }

        private void InsertIntoGroup(TItemIndex index, GroupID groupId)
        {
            Assert.AssertTrue(IsValidGroup(groupId));
            TItemIndex entrance = GetGroupEntrance(groupId);
            if (entrance == InvalidIndex)
            {
                _Items.Items[index].Flag.NextIndex = index;
                _Items.Items[index].Flag.PrevIndex = index;
                _GroupInfos.Items[groupId].Entrance = index;
            }
            else
            {
                TItemIndex prev = GetItemFlag(entrance).PrevIndex;
                _Items.Items[entrance].Flag.PrevIndex = index;
                _Items.Items[index].Flag.PrevIndex = prev;
                _Items.Items[prev].Flag.NextIndex = index;
                _Items.Items[index].Flag.NextIndex = entrance;
            }
            _Items.Items[index].Flag.Group = groupId;
            //SetItemGroup(index, groupId);
            SetGroupItemCount(groupId, GetGroupItemCount(groupId) + 1);
        }
        #endregion

        #region public methods

        public void LockQueue() { /*_LockDepth++; */}
        public void UnlockQueue() { /*_LockDepth--; */}

        public TItem GetItem(TItemIndex index)
        {
            return GetItemValue(index);
        }

        public void SetItem(TItemIndex index, TItem value)
        {
            Assert.AssertTrue(IsValidIndex(index));
            _Items.Items[index].Value = value;
        }

        public GroupID GetItemGroup(TItemIndex index)
        {
            return GetItemFlag(index).Group;
        }

        public void SetItemGroup(TItemIndex index, GroupID groupId)
        {
            Assert.AssertTrue(!IsLockedQueue());
            if (groupId != GetItemGroup(index))
            {
                RemoveFromGroup(index);
                InsertIntoGroup(index, groupId);
            }
        }

        public TItemIndex GetGroupItemCount(GroupID groupId)
        {
            Assert.AssertTrue(IsValidGroup(groupId));
            return _GroupInfos.Items[groupId].ItemCount;
        }

        public TItemIndex GetGroupEntrance(GroupID groupId)
        {
            Assert.AssertTrue(IsValidGroup(groupId));
            return _GroupInfos.Items[groupId].Entrance;
        }

        public TItemIndex RawGetNextIndex(TItemIndex index)
        {
            return GetItemFlag(index).NextIndex;
        }

        public TItemIndex Capacity() { return _ItemCount; }

        public GroupID GetGroupCount() { return _GroupCount; }

        public MultiCyclicQueueIterator<MultiCyclicQueue<TItem>, TItem> GetIteratorByGroup(int groupId)
        {
            Assert.AssertTrue(IsValidGroup(groupId));
            return new MultiCyclicQueueIterator<MultiCyclicQueue<TItem>, TItem>(this, groupId);
        }

        public bool AppendCapacity(GroupID initGroupID, TItemIndex moreItemCount)
        {
            Assert.AssertTrue(IsValidGroup(initGroupID));
            Assert.AssertTrue(moreItemCount > 0);
            if (MaxCapacity < moreItemCount + _ItemCount)
                return false;

            TItemIndex oldCapacity = _ItemCount;
            TItemIndex newCapacity = _ItemCount + moreItemCount;

            _Items.Capacity = _Items.Capacity + moreItemCount;

            _ItemCount = newCapacity;

            //将新元素放入initGroupID中
            for (TItemIndex i = oldCapacity; i < newCapacity; i++)
            {
                _Items.Add(new ItemInfo());
                _Items.Items[i].Flag.NextIndex = i + 1;
                _Items.Items[i].Flag.PrevIndex = i - 1;
                _Items.Items[i].Flag.Group = initGroupID;
            }

            //重建initGroup。修复其首尾元素相接，修复其入口
            TItemIndex entrance = GetGroupEntrance(initGroupID);
            TItemIndex first = oldCapacity;
            TItemIndex last = newCapacity - 1;
            if (entrance == InvalidIndex)
            {//initGroupID组没有元素
                _Items.Items[first].Flag.PrevIndex = last;     //ring them
                _Items.Items[last].Flag.NextIndex = first;
                SetGroupEntrance(initGroupID, first);
                SetGroupItemCount(initGroupID, moreItemCount);
            }
            else
            {//initGroupID组有元素
                TItemIndex prev = GetItemFlag(entrance).PrevIndex;
                _Items.Items[first].Flag.PrevIndex = prev;
                _Items.Items[prev].Flag.NextIndex = first;
                _Items.Items[entrance].Flag.PrevIndex = last;
                _Items.Items[last].Flag.NextIndex = entrance;
                SetGroupItemCount(initGroupID, GetGroupItemCount(initGroupID) + moreItemCount);
            }

            return true;
        }

        public void Dispose()
        {
            _Items.Clear();
            _ItemCount = 0;
            _GroupInfos.Clear();
            _GroupCount = 0;
        }
        #endregion
    }

    /// <summary>
    /// MultiCyclic迭代器
    /// </summary>
    /// <typeparam name="TMultiQueue">The type of the multi queue.</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.IEnumerator" />
    public class MultiCyclicQueueIterator<TMultiQueue, T> : IEnumerator
        where TMultiQueue : MultiCyclicQueue<T>
    {
        internal const int InvalidIndex = int.MaxValue;
        private TMultiQueue _Queue;
        private TItemIndex _CurItemIndex;
        private TItemIndex _NextIndex;
        private int _IterItemLeft;
        private GroupID _Group;

        public MultiCyclicQueueIterator(TMultiQueue queue, GroupID group)
        {
            _Queue = queue;
            _Group = group;
            _Queue.LockQueue();
            Reset();
        }

        ~MultiCyclicQueueIterator()
        {
            if (_IterItemLeft != -1) //没有遍历完就中途退出时，释放写锁。如果遍历完会自动unlock.
                _Queue.UnlockQueue();
        }

        public bool MoveNext()
        {
            if (_IterItemLeft > 0)
            {
                _IterItemLeft--;
                _CurItemIndex = _NextIndex;
                _NextIndex = _Queue.RawGetNextIndex(_NextIndex);
                return true;
            }
            else
            {
                Assert.AssertTrue(_IterItemLeft == 0, "MoveNext()返回false之后，应该结束循环，并删除本迭代器实例");
                _IterItemLeft = -1;//标识为已遍历完
                _Queue.UnlockQueue(); //遍历完成时，立即(及时性)释放写锁，而不是等到析构时才释放。
                return false;
            }
        }

        public void Reset()
        {
            _CurItemIndex = InvalidIndex;
            _IterItemLeft = (int)_Queue.GetGroupItemCount(_Group);
            TItemIndex firstIndex = _Queue.GetGroupEntrance(_Group);
            _NextIndex = firstIndex;
        }

        public object Current
        {
            get
            {
                Assert.AssertTrue(_CurItemIndex != InvalidIndex);
                return _Queue.GetItem(_CurItemIndex);
            }
        }

        public int CurIndex
        {
            get
            {
                Assert.AssertTrue(_CurItemIndex != InvalidIndex);
                return _CurItemIndex;
            }
        }
    }
}