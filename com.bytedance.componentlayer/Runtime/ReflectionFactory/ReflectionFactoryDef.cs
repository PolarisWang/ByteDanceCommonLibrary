using System;
using ByteDance.Foundation;

namespace ByteDance.ComLayer.Reflection
{
    internal sealed class ReflectionFactoryDef
    {
        public static readonly MyLogger Logger = new MyLogger("ByteDance.Settings.Assembly");
    }

    public interface IRefectionAttr
    {
        T[] KeyTypes<T>() where T : IConvertible;
    }
}