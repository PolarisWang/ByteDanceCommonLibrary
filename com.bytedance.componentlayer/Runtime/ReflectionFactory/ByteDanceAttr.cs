using System;
using System.Collections.Generic;

namespace ByteDance.ComLayer.Reflection
{
    /// <summary>
    /// Warning: 如果特别频繁创建的类，不推荐使用反射
    ///     可以考虑之后用自动代码生成的方式改写
    ///     
    /// 反射工厂类，用于动态收集信息，创建类对象
    /// 关键变量
    ///     TKey : 创建实例时用的Key类型，例如某枚举类
    ///     TClass : 创建的目标实例类型
    ///     TAttr : 继承自ClassAttr用作类信息标记的特性
    /// 用法
    ///     1. 继承ClassAttr编写一个特性A
    ///     2. 注册 var creator = ReflectionFactory.Create<TKey, TClass>(搜索名字空间，特性类型A)
    ///     3. 使用 
    ///         creator.Create(key, args)
    ///         creator.Contains(key)
    ///         creator.TryCreate(key, args)
    ///         creator.Reload()
    /// 信息收集的工作会在第一次创建工厂类对象之前完成 
    /// </summary>
    public abstract class ByteDanceAttr : Attribute, IRefectionAttr
    {
        /// <summary>
        /// 返回搜索Key类型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract T[] KeyTypes<T>() where T : IConvertible;

        protected T[] fillInTypes<T, T2>(params T2[] types) where T : IConvertible
        {
            if (typeof(T) != typeof(T2))
                return null;

            var retTypes = new List<T>();
            foreach (var o in types)
            {
                var rv = (T)Convert.ChangeType(o, typeof(T));
                if (rv == null)
                {
                    ReflectionFactoryDef.Logger.LogError($"Can not Convert Value: {o} To Type: {typeof(T)}");
                    continue;
                }
                retTypes.Add(rv);
            }
            return retTypes.ToArray();
        }
    }
}