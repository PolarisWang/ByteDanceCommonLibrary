using System.Collections.Generic;

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
    public class DictionaryList<TKey,TValue>
    {
        private static MyLogger _logger = new MyLogger("DictionaryList");
        private readonly Dictionary<TKey, TValue> _dic = new Dictionary<TKey, TValue>();
        private readonly List<TValue> _list = new List<TValue>();

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        /// <param name="isOverride">if set to <c>true</c> [is override].</param>
        /// <returns></returns>
        public bool Add(TKey key, TValue val, bool isOverride = false)
        {
            if (_dic.ContainsKey(key))
            {
                if (isOverride)
                {
                    Remove(key);
                    _dic.Add(key, val);
                    _list.Add(val);
                    return true;
                }
                else
                {
                    _logger.LogFatal("Add Key: {0}, Value: {1} failed, Key already exist.".F(key, val));
                    return false;
                }
            }
            else
            {
                _dic.Add(key, val);
                _list.Add(val);
                return true;
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            if (_dic.ContainsKey(key))
            {
                _list.Remove(_dic[key]);
                _dic.Remove(key);
                return true;
            }
            _logger.LogFatal("Remove Key: {0} failed, Key not exist.".F(key));
            return false;
        }

        /// <summary>
        /// Gets the or default.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="def">The definition.</param>
        /// <returns></returns>
        public TValue GetOrDefault(TKey key, TValue def = default(TValue))
        {
            if (_dic.TryGetValue(key,out TValue result))
                return result;
            
            return def;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        public void SetValue(TKey key, TValue val)
        {
            _dic[key] = val;
        }

        /// <summary>
        /// 确定容器中是否包含指定的键
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _dic.ContainsKey(key);
        }

        /// <summary>
        /// 获取与指定的键相关联的值
        /// </summary>
        /// <param name="key">要获取的值的键</param>
        /// <param name="value">当此方法返回值时，如果找到该键，便会返回与指定的键相关联的值；否则，则会返回 value 参数的类型默认值。该参数未经初始化即被传递</param>
        /// <returns>如果容器包含具有指定键的元素，则为 true；否则为 false</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dic.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the list. [TIPS] Dont modify the return list.
        /// </summary>
        /// <returns></returns>
        public IList<TValue> GetList()
        {
            return _list;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _dic.Clear();
            _list.Clear();
        }
    }
}