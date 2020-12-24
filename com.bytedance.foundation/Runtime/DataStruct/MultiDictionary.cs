using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public class MultiDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        public bool ContainsValue(TKey key, TValue value)
        {
            List<TValue> values;
            if (this.TryGetValue(key, out values))
            {
                if (values.Contains(value))
                    return true;
            }
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            List<TValue> values;
            if (!this.TryGetValue(key, out values))
            {
                values = new List<TValue>();
                this[key] = values;
            }
            values.Add(value);
        }

        public bool Remove(TKey key, TValue value)
        {
            List<TValue> values;
            if (this.TryGetValue(key, out values))
            {
                bool removed = values.Remove(value);
                if (values.Count == 0)
                {
                    this.Remove(key);
                }
                return removed;
            }
            return false;
        }
    }
}
