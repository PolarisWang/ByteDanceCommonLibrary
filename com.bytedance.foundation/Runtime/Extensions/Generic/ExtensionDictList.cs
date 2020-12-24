using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public static class ExtensionDictList
    {
        public static Value RobustGet<Key, Value>(this IDictionary<Key, Value> dic, Key key, bool assert = true)
        {
            Value v;
            if (!dic.TryGetValue(key, out v))
            {
                string msg = "No such key: " + key + ", in " + dic.GetType();

                if (assert)
                    MyLogger.Log.LogError(msg);
            }
            return v;
        }

        public static void RobustSave<Key, Value>(this IDictionary<Key, Value> dic, Key key, Value val)
        {
            dic[key] = val;
        }

        public static bool AddSafely<Key, Value>(this IDictionary<Key, IList<Value>> dic, Key key, Value val)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, new List<Value>());

            var list = dic[key];
            if (list.Contains(val))
                return false;
            else
            {
                list.Add(val);
                return true;
            }
        }

        public static bool RemoveSafely<Key, Value>(this IDictionary<Key, IList<Value>> dic, Key key, Value val)
        {
            if (!dic.ContainsKey(key))
                return false;

            var list = dic[key];
            if (list.Remove(val))
            {
                if (list.Count == 0)
                    dic.Remove(key);
                return true;
            }
            else
                return false;
        }

        public static bool RemoveOneValueSafely<Key, Value>(this IDictionary<Key, IList<Value>> dic, Value val)
        {
            foreach (var pair in dic)
            {
                if (pair.Value.Remove(val))
                {
                    if (pair.Value.Count == 0)
                        dic.Remove(pair.Key);
                    return true;
                }
            }
            return false;
        }

        public static bool HasKey<Key, Value>(this IDictionary<Key, IList<Value>> dic, Key key)
        {
            if (!dic.ContainsKey(key))
                return false;
            if (dic[key].Count == 0)
            {
                dic.Remove(key);
                return false;
            }
            else
                return true;
        }

        public static bool HasValue<Key, Value>(this IDictionary<Key, IList<Value>> dic, Key key, Value val)
        {
            if (!dic.ContainsKey(key))
                return false;
            if (dic[key].Count == 0)
            {
                dic.Remove(key);
                return false;
            }
            else
                return dic[key].Contains(val);
        }

        public static void AddItem<K, T>(this IDictionary<K, IList<T>> dict, K key, T t)
        {
            if (!dict.ContainsKey(key))
            {
                IList<T> items = new List<T>();
                items.Add(t);
                dict.Add(key, items);
            }
            else
            {
                dict[key].Add(t);
            }
        }

        public static bool PushToList(this IDictionary<string, IList<string>> list, string key, string data)
        {
            if (list.ContainsKey(key))
            {
                if (list[key].Contains(data))
                    return false;
                else
                {
                    list[key].Add(data);
                    return true;
                }
            }
            else
            {
                List<string> newList = new List<string>();
                newList.Add(data);
                list.Add(key, newList);
                return true;
            }
        }
    }
}