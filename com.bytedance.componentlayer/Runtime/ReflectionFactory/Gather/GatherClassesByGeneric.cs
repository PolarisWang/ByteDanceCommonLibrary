using System;
using System.Collections.Generic;
using ByteDance.Foundation;

namespace ByteDance.ComLayer.Reflection
{
    /// <summary>
    /// 通过template类反射实现类
    /// 使用说明:
    ///     Class  : TSubClass [TTemplateSubClass]
    ///     通过TemplateSubClass类型，创建TSubClass
    /// </summary>
    /// <typeparam name="TSubClass">The type of the sub class.</typeparam>
    /// <typeparam name="TTemplateSubClass">The type of the template sub class.</typeparam>
    /// <seealso cref="ByteDance.ComLayer.Reflection.IGatherClasses" />
    public class GatherClassesByGeneric<TSubClass, TTemplateSubClass> : IGatherClasses
        where TSubClass : ByteDancePoolItem
        where TTemplateSubClass : class
    {
        private readonly ByteDancePoolManager _pool = new ByteDancePoolManager();
        /// <summary> TemplateSubKey => Class Type </summary>
        private readonly Dictionary<Type, Type> _typeDict = new Dictionary<Type, Type>();
        /// <summary> The search done </summary>
        private bool _searchDone = false;
        /// <summary> Gets the logger. </summary>
        private Foundation.MyLogger logger => ReflectionFactoryDef.Logger;
        private readonly string _namespace;

        public GatherClassesByGeneric(string nspace)
        {
            _namespace = nspace;
        }

        /// <summary>
        /// Creates the specified template sub class.
        /// </summary>
        public TSubClass Create(Type genericSubClass)
        {
            var type = _typeDict.GetOrDefault(genericSubClass, null);
            if (type == null)
            {
                logger.LogError($"Cannot find class by template sub class: {genericSubClass}");
                return default(TSubClass);
            }

            var ins = _pool.PeekToUse(type) as TSubClass;
            if (ins == null)
            {
                logger.LogError($"Can not Create Instance Type Key = {type}");
                return default(TSubClass);
            }
            return ins;
        }

        public void Reload(Type[] allTypes)
        {
            _searchDone = false;
            _doGather(allTypes);
        }

        public void Dispose()
        {
            _pool.Dispose();
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
                var baseType = cls.BaseType;
                var typeInfo = (System.Reflection.TypeInfo) baseType;
                var _genericTypeArguments = typeInfo.GenericTypeArguments;
                if (_genericTypeArguments != null && _genericTypeArguments.Length == 1 )
                {
                    var t = _genericTypeArguments[0];
                    if (t.IsSubclassOf(typeof(TTemplateSubClass)))
                        _typeDict.Add(t, cls);
                }
            }
            _searchDone = true;
        }
    }
}