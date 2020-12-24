using System;
using System.Collections.Generic;
using System.Reflection;
using ByteDance.Foundation;

namespace ByteDance.ComLayer.Reflection
{
    public interface IGatherClasses
    {
        void Reload(Type[] allTypes);
        void Dispose();
    }

    /// <summary>
    /// 反射信息类集合
    /// </summary>
    /// <typeparam name="TAttr">The type of the attribute.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TSubClass">The type of the sub class.</typeparam>
    public class GatherClasses< TAttr, TKey, TSubClass>: IGatherClasses
        where TAttr : ByteDanceAttr
        where TKey : IConvertible
        where TSubClass : class 
    {
        /// <summary> Key => Class Type </summary>
        private readonly Dictionary<TKey, Type> _typeDict = new Dictionary<TKey, Type>();
        /// <summary> The search done </summary>
        private bool _searchDone = false;
        /// <summary> Gets the logger. </summary>
        private Foundation.MyLogger logger => ReflectionFactoryDef.Logger;
        private readonly string _namespace;

        public GatherClasses(string nspace)
        {
            _namespace = nspace;
        }

        public bool Contains(TKey key)
        {
            return _typeDict.ContainsKey(key);
        }

        public TSubClass Create(TKey key, params object[] args)
        {
            if (!_searchDone)
            {
                ReflectionFactoryDef.Logger.LogError("Gather classes not be loaded, Please use Load() to init.");
                return default(TSubClass);
            }

            var tKey = (TKey)Convert.ChangeType(key, typeof(TKey));
            if (!_typeDict.ContainsKey(tKey))
            {
#if UNITY_EDITOR
                //设计上,就是存在有VBuff没DBuff等情况,这个日志没用
                //logger.LogWarning($"Create Failed, No Class Gathered Type Key = {key}" );
#endif
                return default(TSubClass);
            }

            var ins = Activator.CreateInstance(_typeDict[tKey], args) as TSubClass;
            if (ins == null)
            {
                logger.LogError($"Can not Create Instance Type Key = {key}");
                return default(TSubClass);
            }
            return ins;
        }

        public TSubClass TryCreate(TKey key, params object[] args)
        {
            if (!Contains(key))
            {
                return default(TSubClass);
            }
            return Create(key, args);
        }

        public TSubClass CreateOrDefault(TKey key, Type defaultType, params object[] args)
        {
            if (Contains(key))
                return Create(key, args);
            else
                return Activator.CreateInstance(defaultType, args) as TSubClass;
        }

        public void Reload(Type[] allTypes)
        {
            _searchDone = false;
            _doGather(allTypes);
        }

        public void Dispose()
        {
            _searchDone = false;
            _typeDict.Clear();
        }

        void _doGather(Type[] allTypes)
        {
            if (_searchDone)
                return;

            var types = allTypes.Where(t => t.IsClass && t.Namespace == _namespace);
            foreach (var cls in types)
            {
                var t = cls.GetCustomAttributes(typeof(TAttr), false).FirstOrDefault(null);
                if (t != null)
                {
                    IRefectionAttr ta = (IRefectionAttr)t;
                    var keys = ta.KeyTypes<TKey>();
                    if (keys != null)
                    {
                        foreach (var k in keys)
                            _typeDict[k] = cls;
                    }
                    else
                        logger.LogError(
                            $"Can not Get Key Type Class = {cls}, KeyType = {typeof(TKey)}, Attr = {typeof(TAttr)}");
                }
            }
            _searchDone = true;
        }
    }
}
