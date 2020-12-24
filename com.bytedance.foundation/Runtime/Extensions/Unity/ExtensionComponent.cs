
namespace ByteDance.Foundation
{
    public static class ExtensionComponent
    {
        public static T GetComponent<T>(this T t) where T :UnityEngine.Component
        {
            return t;
        }
    }
}