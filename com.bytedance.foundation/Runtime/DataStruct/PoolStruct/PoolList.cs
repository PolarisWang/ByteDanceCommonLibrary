using System.Collections.Generic;
using System.Diagnostics;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 自动使用池子管理的List
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class PoolList<TItem> where TItem : ByteDancePoolItem, new()
    {
        /// <summary> 池子对象，所有待分配对象都储存在这里 </summary>
        private ByteDancePoolList<TItem> _pool;
        /// <summary> 使用中的列表 </summary>
        private List<TItem> _list;

        public PoolList(int size = 1)
        {
            Assert.AssertTrue(size>0, "size must bigger than zero.");
            _list = new List<TItem>(size);
            _pool = new ByteDancePoolList<TItem>();
            for (int index = 0; index < size; index++)
                _pool.PushItem(new TItem(), EPoolListState.Idle);
        }

        /// <summary>
        /// 向列表新增Item.
        /// 使用方法：
        /// 1. 先Add()返回对象
        /// 2. 对Item赋值
        /// </summary>
        /// <returns></returns>
        public TItem Add()
        {
            var item = _pool.PeekToUse();
            if (item == null)
            {
                item = new TItem();
                _pool.PushItem(item, EPoolListState.Using);
                _list.Add(item);
            }
            return item;
        }

        /// <summary>
        /// 列表删除对象：
        /// 1. 自动向池子中返回对象，并调用ReturnToPool接口
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(TItem item)
        {
            if (_list.Remove(item))
            {
                item.Return();
                return true;
            }

            return false;
        }

        /// <summary> 返回List数量 </summary>
        public int Count => _list.Count;

        /// <summary> 获取第index个Item </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public TItem Get(int index)
        {
            return _list[index];
        }

        /// <summary> 清理 </summary>
        public void Clear()
        {
            _pool.Dispose();
            _list.Clear();
        }
    }
}