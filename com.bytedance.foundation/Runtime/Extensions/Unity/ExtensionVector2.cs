using UnityEngine;

namespace ByteDance.Foundation
{
    public static class ExtensionVector2
    {
        public static Vector2 Lerp(Vector2 from, Vector2 to, params float[] t)
        {
            float x = Mathf.Lerp(from.x, to.x, t[0]);
            float y = Mathf.Lerp(from.y, to.y, t[1]);
            return new Vector2(x, y);
        }

        public static float CosAngle(this Vector2 l, Vector2 r)
        {
            return Vector2.Dot(l.normalized, r.normalized);
        }

        public static Vector2 CalReflect(this Vector2 inDir, Vector2 normal)
        {
            return Vector2.Reflect(inDir, normal);
        }
    }
}