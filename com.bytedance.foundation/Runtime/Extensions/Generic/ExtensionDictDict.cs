using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public static class ExtensionDictDict
    {
        public static Value RobustGet<Key1, Key2, Value>(this IDictionary<Key1, IDictionary<Key2, Value>> dic,
            Key1 key1, Key2 key2, Value defaultValue = default(Value))
        {
            if (dic == null)
                return defaultValue;
            if (!dic.ContainsKey(key1))
                return defaultValue;
            var secDic = dic[key1];
            if (!secDic.ContainsKey(key2))
                return defaultValue;
            return secDic[key2];
        }

        public static bool RobustAdd<Key1, Key2, Value>(this IDictionary<Key1, IDictionary<Key2, Value>> dic,
            Key1 key1, Key2 key2, Value value)
        {
            if (dic == null)
            {
                MyLogger.Log.LogError("ExtensionDictDict::RobustAdd failed. Dic is null");
                return false;
            }

            if (dic.ContainsKey(key1))
            {
                var secDic = dic[key1];
                if (secDic.ContainsKey(key2))
                {
                    MyLogger.Log.LogError(
                        "ExtensionDictDict::RobustAdd failed. Dic already has Key:{0}/{1} Value : {2}".F(key1, key2,
                            value));
                    return false;
                }
                else
                {
                    secDic.Add(key2, value);
                    return true;
                }
            }
            else
            {
                var secDic = new Dictionary<Key2, Value>();
                secDic.Add(key2, value);
                dic.Add(key1, secDic);
                return true;
            }
        }

        public static bool RobustRemove<Key1, Key2, Value>(this IDictionary<Key1, IDictionary<Key2, Value>> dic,
            Key1 key1, Key2 key2)
        {
            if (dic == null)
            {
                MyLogger.Log.LogError("ExtensionDictDict::RobustRemove failed. Dic is null");
                return false;
            }

            if (dic.ContainsKey(key1))
            {
                var secDic = dic[key1];
                if (secDic.ContainsKey(key2))
                {
                    secDic.Remove(key2);
                    if (secDic.Count == 0)
                        dic.Remove(key1);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
}