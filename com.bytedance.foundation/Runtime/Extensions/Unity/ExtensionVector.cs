using UnityEngine;

namespace ByteDance.Foundation
{
    public static class ExtensionVector
    {
        /// <summary>A useful Epsilon</summary>
        public const float Epsilon = 0.0001f;

        /// <summary>Much more stable for small angles than Unity's native implementation</summary>
        public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 up)
        {
            float angle = Angle(v1, v2);
            if (Mathf.Sign(Vector3.Dot(up, Vector3.Cross(v1, v2))) < 0)
                return -angle;
            return angle;
        }

        /// <summary>Much more stable for small angles than Unity's native implementation</summary>
        public static float Angle(Vector3 v1, Vector3 v2)
        {
#if false // Maybe this version is better?  to test....
            float a = v1.magnitude;
            v1 *= v2.magnitude;
            v2 *= a;
            return Mathf.Atan2((v1 - v2).magnitude, (v1 + v2).magnitude) * Mathf.Rad2Deg * 2;
#else            
            v1.Normalize();
            v2.Normalize();
            return Mathf.Atan2((v1 - v2).magnitude, (v1 + v2).magnitude) * Mathf.Rad2Deg * 2;
#endif
        }

        /// <summary>
        /// 将vp旋转 vp与vdir夹角的度数
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="vdir"></param>
        /// <returns></returns>
        public static Vector3 ReverseRotate(Vector3 vp, Vector3 vdir, Vector3 worldUp)
        {
            Vector3 direction = new Vector3(1,0,0);
            var angle = Vector3.Angle(vdir, direction);
            return Quaternion.AngleAxis(angle, worldUp) * vp;
        }
    }
}