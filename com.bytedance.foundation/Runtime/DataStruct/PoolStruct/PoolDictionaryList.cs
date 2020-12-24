using System;
using System.Collections.Generic;
using UnityEngine;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 需要頻繁調用遍歷和通過Key尋址
    /// 使用環境説明：
    ///     1. 内存會生成一個List和Dictionary的堆占用，用空間換時間
    ///     2. 多綫程非安全
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Serializable]
    public sealed class PoolDictionaryList<TKey, TValue> where TValue : ByteDancePoolItem, new()
    {
        private static MyLogger _logger = new MyLogger("PoolDictionaryList");
        [SerializeField] private readonly Dictionary<TKey, TValue> _dic;
        private readonly List<TValue> _list;
        /// <summary> 池子对象，所有待分配对象都储存在这里 </summary>
        [SerializeField] private readonly ByteDancePoolList<TValue> _pool;

        public PoolDictionaryList(int size = 1, IEqualityComparer<TKey> comparer = null)
        {
            Assert.AssertTrue(size > 0, "size must bigger than zero.");
            _list = new List<TValue>(size);
            _dic = new Dictionary<TKey, TValue>(size, comparer);
            _pool = new ByteDancePoolList<TValue>();
            for (int index = 0; index < size; index++)
                _pool.PushItem(new TValue(), EPoolListState.Idle);
        }

        /// <summary> Adds the specified key. </summary>
        public TValue Add(TKey key, bool isOverride = false)
        {
            var defaultVal =  _dic.GetOrDefault(key);
            if (defaultVal != null)
            {
                if (isOverride)
                {
                    Remove(key);
                    var item = _peek_or_new_();
                    _dic.Add(key, item);
                    _list.Add(item);
                    return item;
                }
                else
                {
                    _logger.LogFatal($"Add Key: {key} failed, Key already exist.");
                    return default(TValue);
                }
            }
            else
            {
                var item = _peek_or_new_();
                _dic.Add(key, item);
                _list.Add(item);
                return item;
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            var val = _dic.GetOrDefault(key);
            if (val != null)
            {
                _list.Remove(val);
                _dic.Remove(key);
                val.Return();
                return true;
            }
            _logger.LogFatal("Remove Key: {0} failed, Key not exist.".F(key));
            return false;
        }

        /// <summary> 确定容器中是否包含指定的键 </summary>
        public bool ContainsKey(TKey key)
        {
            return _dic.ContainsKey(key);
        }

        /// <summary> 获取与指定的键相关联的值 </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dic.TryGetValue(key, out value);
        }

        /// <summary> Gets the list. [TIPS] Dont modify the return list. </summary>
        public IList<TValue> GetList()
        {
            return _list;
        }

        public IDictionary<TKey, TValue> GetDictionary()
        {
            return _dic;
        }

        /// <summary> Clears this instance. </summary>
        public void Clear()
        {
            for (int index = 0; index < _list.Count; index++)
                _list[index].Return();
            _dic.Clear();
            _list.Clear();
        }

        private TValue _peek_or_new_()
        {
            var item = _pool.PeekToUse();
            if (item == null)
            {
                item = new TValue();
                _pool.PushItem(item, EPoolListState.Using);
            }
            return item;
        }
    }
}