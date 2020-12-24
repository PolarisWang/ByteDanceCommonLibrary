using System;

namespace ByteDance.Foundation
{
    using TItemIndex = System.Int32;

    /// <summary>
    /// 排序环形结构
    /// 描述：
    ///     根据输入的Key排序, 默认升序排列， 环形结构存储数据。
    ///     为了乱序插入的时候能维持有序队列存储
    ///     本类追求较快的遍历速度，适用于有大量元素、遍历操作很多、增删操作也很多，还需快速索引定位的场合。
    /// 特点：
    ///     容量受限（但可手动扩容量）
    ///     快速索引 : O(1) 
    ///     快速插入/删除元素 ：O(1)
    ///     快速寻找空闲索引 ：O(1)
    ///     快速单向循环遍历 : O(N),实际为O(1)*N,N为有效元素个数
    /// </summary>
    public sealed class SortedRingStruct<TKey, TItem> 
        where TKey : IComparable
        where TItem : class, ByteDance.Foundation.Annotation.IDisposable, new() 
    {
        public const TItemIndex NullIndex = TItemIndex.MaxValue - 2;
        public const TItemIndex InvalidIndex = TItemIndex.MaxValue;
        public const TItemIndex MaxCapacity = TItemIndex.MaxValue - 1;

        private ExposedList<ItemInfo> _items;
        private GroupInfo _freeGroup;
        private GroupInfo _usingGroup;
        private TItemIndex _itemCount;

        /// <summary> constructor </summary>
        public SortedRingStruct(int capacity)
        {
            _itemCount = capacity;
            _usingGroup = new GroupInfo() {FirstIndex = NullIndex, LastIndex = NullIndex, ItemCount = 0};
            _freeGroup = new GroupInfo() {FirstIndex = NullIndex, LastIndex = NullIndex, ItemCount = 0};

            // Init items
            _items = new ExposedList<ItemInfo>(capacity);
            for (int index = 0; index < capacity; index++)
                _items.Add(new ItemInfo());

            _set_all_item_group_();
        }

        public TItem AllocateNew(TKey key)
        {
            // Remove Current
            TItemIndex itemIndex;
            if (_freeGroup.FirstIndex != NullIndex)
            {
                itemIndex = _freeGroup.FirstIndex;
                TItemIndex prev = _items.Items[itemIndex].PrevIndex;
                TItemIndex next = _items.Items[itemIndex].NextIndex;
                _items.Items[prev].NextIndex = next;
                _items.Items[next].PrevIndex = prev;
                _freeGroup.ItemCount = _freeGroup.ItemCount - 1;
                _freeGroup.FirstIndex = _freeGroup.ItemCount == 0 ? NullIndex : next;
                _freeGroup.LastIndex = _freeGroup.ItemCount == 0 ? NullIndex : prev;
            }
            else
            {
                itemIndex = _usingGroup.LastIndex;
                TItemIndex prev = _items.Items[itemIndex].PrevIndex;
                TItemIndex next = _items.Items[itemIndex].NextIndex;
                _items.Items[prev].NextIndex = next;
                _items.Items[next].PrevIndex = prev;
                _usingGroup.ItemCount = _usingGroup.ItemCount - 1;
                _usingGroup.FirstIndex = _usingGroup.ItemCount == 0 ? NullIndex : _usingGroup.FirstIndex;
                _usingGroup.LastIndex = _usingGroup.ItemCount == 0 ? NullIndex : prev;
            }

            // Set Value.
            _items.Items[itemIndex].Key = key;

            // Find Where to insert.
            _insert_into_using_(itemIndex,key);

            return _items.Items[itemIndex].Value;
        }

        public void GetClampItem(TKey key, out TItem bigger, out TItem smaller)
        {
            var preIndex = -1;
            var curIndex = _usingGroup.FirstIndex;
            for (int index = 0; index < _usingGroup.ItemCount; index++)
            {
                if (key.CompareTo(_items.Items[curIndex].Key) > 0)
                {
                    bigger = index == 0 ? null : _items.Items[preIndex].Value;
                    smaller = _items.Items[curIndex].Value;
                    return;
                }

                preIndex = curIndex;
                curIndex = _items.Items[curIndex].NextIndex;
            }

            bigger = _items.Items[_usingGroup.LastIndex].Value;
            smaller = null;
        }

        public void Clear()
        {
            _usingGroup = new GroupInfo() { FirstIndex = NullIndex, LastIndex = NullIndex, ItemCount = 0 };
            _freeGroup = new GroupInfo() { FirstIndex = NullIndex, LastIndex = NullIndex, ItemCount = 0 };
            _set_all_item_group_();
        }

        public override string ToString()
        {
            string msg = "UsingGroup: ";
            var curIndex = _usingGroup.FirstIndex;
            for (int index = 0; index < _usingGroup.ItemCount; index++)
            {
                msg += $"[Key:{_items.Items[curIndex].Key}, Index:{curIndex}] - ";
                curIndex = _items.Items[curIndex].NextIndex;
            }

            msg += "End\nFreeGroup: ";
            curIndex = _freeGroup.FirstIndex;
            for (int index = 0; index < _freeGroup.ItemCount; index++)
            {
                msg += $"[Key:{_items.Items[curIndex].Key}, Index:{curIndex}] - ";
                curIndex = _items.Items[curIndex].NextIndex;
            }

            msg += "End";
            return msg;
        }

        private void _insert_into_using_(TItemIndex itemIndex, TKey myKey)
        {
            TItemIndex usingIndex = _usingGroup.FirstIndex;
            if (_usingGroup.ItemCount == 0)
            {
                _items.Items[itemIndex].NextIndex = itemIndex;
                _items.Items[itemIndex].PrevIndex = itemIndex;
                _usingGroup.FirstIndex = itemIndex;
                _usingGroup.LastIndex = itemIndex;
                _usingGroup.ItemCount = 1;
            }
            else
            {
                var currentIndex = _usingGroup.FirstIndex;
                bool found = false;
                for (int index = 0; index < _usingGroup.ItemCount; index++)
                {
                    var key = _items.Items[currentIndex].Key;
                    if (myKey.CompareTo(key) >= 0)
                    {
                        _insert_into_before_(itemIndex, currentIndex);
                        found = true;
                        break;
                    }
                    currentIndex = _items.Items[currentIndex].NextIndex;
                }

                if (!found)
                    _insert_into_before_(itemIndex, itemIndex);
            }
        }

        private void _insert_into_before_(TItemIndex itemIndex, TItemIndex beforeIndex)
        {
            bool isFirst = beforeIndex == _usingGroup.FirstIndex;
            bool isLast = beforeIndex == itemIndex;

            var prevIndex = _items.Items[beforeIndex].PrevIndex;
            _items.Items[beforeIndex].PrevIndex = itemIndex;
            _items.Items[prevIndex].NextIndex = itemIndex;
            _items.Items[itemIndex].PrevIndex = prevIndex;
            _items.Items[itemIndex].NextIndex = beforeIndex;

            if (isFirst) _usingGroup.FirstIndex = itemIndex;
            if (isLast) _usingGroup.LastIndex = itemIndex;
            _usingGroup.ItemCount += 1;
        }

        private void _remove_item_(TItemIndex index)
        {
            TItemIndex prev = _items.Items[index].PrevIndex;
            TItemIndex next = _items.Items[index].NextIndex;
            _items.Items[prev].NextIndex = next;
            _items.Items[next].PrevIndex = prev;
        }

        private void _set_all_item_group_()
        {
            for (TItemIndex i = 0; i < _itemCount; i++)
            {
                _items.Items[i].NextIndex = (i + 1) % _itemCount;
                _items.Items[i].PrevIndex = i - 1 < 0 ? _itemCount - 1 : i - 1;
                _items.Items[i].Key = default(TKey);
                if (_items.Items[i].Value == null)
                    _items.Items[i].Value = new TItem();
                else
                    _items.Items[i].Value.Dispose();
            }

            TItemIndex last = _itemCount - 1;
            TItemIndex first = 0;
            _freeGroup.FirstIndex = first;
            _freeGroup.LastIndex = last;
            _freeGroup.ItemCount = _itemCount;
        }

        private struct ItemInfo
        {
            public TKey Key;
            public TItem Value;
            public TItemIndex NextIndex;
            public TItemIndex PrevIndex;
        }

        private struct GroupInfo
        {
            public TItemIndex FirstIndex;
            public TItemIndex LastIndex;
            public TItemIndex ItemCount;
        }
    }
}
