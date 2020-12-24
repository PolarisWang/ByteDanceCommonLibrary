using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public class LinkedDictionary<TKey1, TKey2, TValue>
    {
        private Dictionary<TKey1, Dictionary<TKey2, TValue>> _data =
            new Dictionary<TKey1, Dictionary<TKey2, TValue>>();

        public void SetValue(TKey1 key1, TKey2 key2, TValue value)
        {
            if (!_data.ContainsKey(key1))
            {
                Dictionary<TKey2, TValue> content = new Dictionary<TKey2, TValue>();
                content.Add(key2, value);
                _data.Add(key1, content);
            }
            else
            {
                var content = _data[key1];
                content.AddOrReplace(key2, value);
            }
        }

        public TValue GetValue(TKey1 key1, TKey2 key2)
        {
            if (_data.ContainsKey(key1) && _data[key1].ContainsKey(key2))
                return _data[key1][key2];
            return default(TValue);
        }
    }
}