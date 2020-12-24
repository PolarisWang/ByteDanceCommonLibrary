namespace ByteDance.Foundation
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="DictionaryDictionary{TKey1, TKey2, TValue}" />.
    /// </summary>
    /// <typeparam name="TKey1">.</typeparam>
    /// <typeparam name="TKey2">.</typeparam>
    /// <typeparam name="TValue">.</typeparam>
    public class DictionaryDictionary<TKey1, TKey2, TValue>
    {
        /// <summary>
        /// Defines the _table.
        /// </summary>
        private Dictionary<TKey1, Dictionary<TKey2, TValue>> _table;

        /// <summary>
        /// Defines the _key2Comparer.
        /// </summary>
        private IEqualityComparer<TKey2> _key2Comparer;

        /// <summary>
        /// Defines the key2 Dictionary Capacity;
        /// </summary>
        private int _key2Capacity = 16;

        /// <summary>
        /// Getter of TKey1 Dictionary.Count
        /// </summary>
        public int Key1DictCount { get { return _table == null ? 0 : _table.Count; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDictionary{TKey1, TKey2, TValue}"/> class.
        /// </summary>
        /// <param name="key1Comparer">The key1Comparer<see cref="IEqualityComparer{TKey1}"/>.</param>
        /// <param name="key2Comparer">The key2Comparer<see cref="IEqualityComparer{TKey2}"/>.</param>
        public DictionaryDictionary(IEqualityComparer<TKey1> key1Comparer = null,
            IEqualityComparer<TKey2> key2Comparer = null)
        {
            _table = new Dictionary<TKey1, Dictionary<TKey2, TValue>>(key1Comparer);
            _key2Comparer = key2Comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDictionary{TKey1, TKey2, TValue}"/> class.
        /// </summary>
        /// <param name="key1Capacity">TKey1 Dictionary</param>
        /// <param name="key2Capacity">TKey2 Dictionary</param>
        public DictionaryDictionary(int key1Capacity, int key2Capacity = 0)
        {
            _table = new Dictionary<TKey1, Dictionary<TKey2, TValue>>(key1Capacity);
            if (key2Capacity > 0)
                _key2Capacity = key2Capacity;
        }

        /// <summary>
        /// The AddOrSet.
        /// </summary>
        /// <param name="key1">The key1<see cref="TKey1"/>.</param>
        /// <param name="key2">The key2<see cref="TKey2"/>.</param>
        /// <param name="val">The val<see cref="TValue"/>.</param>
        public void AddOrSet(TKey1 key1, TKey2 key2, TValue val)
        {
            if (_table.TryGetValue(key1, out var value1))
            {
                value1[key2] = val;
            }
            else
            {
                Dictionary<TKey2, TValue> dic = new Dictionary<TKey2, TValue>(_key2Capacity, _key2Comparer);
                dic.Add(key2, val);
                _table.Add(key1, dic);
            }
        }

        /// <summary>
        /// The Remove.
        /// </summary>
        /// <param name="key1">The key1<see cref="TKey1"/>.</param>
        /// <param name="key2">The key2<see cref="TKey2"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (_table.TryGetValue(key1, out var value1))
            {
                return value1.Remove(key2);
            }

            return false;
        }

        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="key1">The key1<see cref="TKey1"/>.</param>
        /// <param name="key2">The key2<see cref="TKey2"/>.</param>
        /// <returns>The <see cref="TValue"/>.</returns>
        public TValue Get(TKey1 key1, TKey2 key2)
        {
            if (_table.TryGetValue(key1,out var value1))
            {
                if (value1.TryGetValue(key2,out TValue value2))
                    return value2;
            }

            return default(TValue);
        }

        /// <summary>
        /// The TryGetValue
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(TKey1 key1, TKey2 key2, out TValue value)
        {
            if (_table.TryGetValue(key1, out var value1))
            {
                if (value1.TryGetValue(key2, out TValue value2))
                {
                    value = value2;
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// 确保 Key1 字典初始化
        /// 功能：
        ///     确保目标空间非空，防止直接访问报空
        /// </summary>
        public void GetKey1Ready(TKey1 key1)
        {
            if (!_table.ContainsKey(key1))
            {
                _table[key1] = new Dictionary<TKey2, TValue>(_key2Capacity, _key2Comparer);
            }
        }

        /// <summary>
        /// The Has.
        /// </summary>
        /// <param name="key1">The key1<see cref="TKey1"/>.</param>
        /// <param name="key2">The key2<see cref="TKey2"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Has(TKey1 key1, TKey2 key2)
        {
            if (_table.TryGetValue(key1, out var value1))
            {
                return value1.ContainsKey(key2);
            }

            return false;
        }

        /// <summary>
        /// The Clear.
        /// </summary>
        public void Clear()
        {
            _table.Clear();
        }
    }
}
