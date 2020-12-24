using System.Collections.Generic;
using JetBrains.Annotations;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 自动使用池子管理的Dictionary
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class PoolDictionary<TKey, TItem> where TItem : ByteDancePoolItem, new()
    {
        /// <summary> 池子对象，所有待分配对象都储存在这里 </summary>
        private ByteDancePoolList<TItem> _pool;
        /// <summary> 使用中的列表 </summary>
        private Dictionary<TKey, TItem> _dictionary;

        public PoolDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _pool = new ByteDancePoolList<TItem>();
            _dictionary = new Dictionary<TKey, TItem>(capacity, comparer);
            for (int index = 0; index < capacity; index++)
                _pool.PushItem(new TItem(), EPoolListState.Idle);
        }

        public PoolDictionary() : this(0, null) { }
        public PoolDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) { }

        /// <summary>
        /// 向列表新增Item.
        /// 使用方法：
        /// 1. 先Add()返回对象
        /// 2. 对Item赋值
        /// </summary>
        /// <param name="key">KEY</param>
        public TItem Add(TKey key)
        {
            if (_dictionary.TryGetValue(key, out var item))
                return item;
            var new_item = _pool.PeekToUse();
            if (new_item == null)
            {
                new_item = new TItem();
                _pool.PushItem(new_item, EPoolListState.Using);
            }

            _dictionary.Add(key, new_item);
            return new_item;
        }

        /// <summary> 移除Key </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out var item))
            {
                item.Return();
                _dictionary.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary> 获取 </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [CanBeNull]
        public TItem Get(TKey key)
        {
            return _dictionary[key];
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TItem item)
        {
            return _dictionary.TryGetValue(key, out item);
        }

        public void Clear()
        {
            _dictionary.Clear();
            _pool.Dispose();
        }
    }
}