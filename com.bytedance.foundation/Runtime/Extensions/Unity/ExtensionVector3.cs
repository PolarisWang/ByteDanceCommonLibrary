using UnityEngine;

namespace ByteDance.Foundation
{
    public static class ExtensionVector3
    {
        public static void Copy(ref Vector3 dest, Vector3 src, bool includeZ = true)
        {
            dest.x = src.x;
            dest.y = src.y;
            if (includeZ)
                dest.z = src.z;
        }

        public static void Set(ref Vector3 v, params float[] coords)
        {
            if (coords.Length == 0)
                return;
            else if (coords.Length > 3)
                throw new System.Exception("Vector3 has only three dimensions");

            if (coords.Length >= 1)
                v.x = coords[0];
            if (coords.Length >= 2)
                v.y = coords[1];
            if (coords.Length >= 3)
                v.z = coords[2];
        }

        public static Vector3 Lerp(Vector3 from, Vector3 to, params float[] t)
        {
            float x = Mathf.Lerp(from.x, to.x, t[0]);
            float y = Mathf.Lerp(from.y, to.y, t[1]);
            float z = Mathf.Lerp(from.z, to.z, t[2]);
            return new Vector3(x, y, z);
        }

        public static Vector3 Ratio(Vector3 from, Vector3 to, params float[] t)
        {
            float x = ratio(from.x, to.x, t[0]);
            float y = ratio(from.y, to.y, t[1]);
            float z = ratio(from.z, to.z, t[2]);
            return new Vector3(x, y, z);
        }

        private static float ratio(float from, float to, float t)
        {
            return (to - from) * t + from;
        }

        public static Vector3 MaxValue { get { return _MaxValue; } }
        private static Vector3 _MaxValue = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        public static Vector3 MinValue { get { return _MinValue; } }
        private static Vector3 _MinValue = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        public static Vector3 NullValue { get { return _NullValue; } }
        private static Vector3 _NullValue = new Vector3(float.MaxValue - 1, float.MaxValue - 1, float.MaxValue - 1);

        public static Vector3 NaN { get { return _NaN; } }
        private static Vector3 _NaN = new Vector3(float.NaN, float.NaN, float.NaN);

        public static string ToFullString(this Vector3 vec)
        {
            return "({0},{1},{2})".F(vec.x, vec.y, vec.z);
        }

        public static float CosAngle(this Vector3 l, Vector3 r)
        {
            return Vector3.Dot(l.normalized, r.normalized);
        }

        public static Vector3 CalReflect(this Vector3 inDir, Vector3 normal)
        {
            return Vector3.Reflect(inDir, normal);
            // return inDir - 2 * (Vector3.Dot(inDir, normal)) * normal;
        }
        public static Vector4 ToTransMatrixVector4(this Vector3 inVec)
        {
            return new Vector4(inVec.x, inVec.y, inVec.z, 1);
            // return inDir - 2 * (Vector3.Dot(inDir, normal)) * normal;
        }
        public static Vector3 ToTransMatrixVector3(this Vector4 inVec)
        {
            return new Vector3(inVec.x, inVec.y, inVec.z);
            // return inDir - 2 * (Vector3.Dot(inDir, normal)) * normal;
        }

        public static Vector3 WorldToLocalPostion(this Vector3 worldPos, Transform parent)
        {
            return parent.worldToLocalMatrix * worldPos.ToTransMatrixVector4();
        }

        public static Vector3 LocalToWorld(this Vector3 local, Transform parent)
        {
            return parent.localToWorldMatrix * local.ToTransMatrixVector4();
        }

        /// <summary>
        /// Returns the Vecto3 distance between these two points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static float DistanceTo(this Vector3 start, Vector3 dest)
        {
            return Vector3.Distance(start, dest);
        }

        /// <summary>
        /// 计算vector向量在planeNormal为法线的平面上的投影
        /// </summary>
        /// <param name="vector">任意向量</param>
        /// <param name="planeNormal">平面法线向量</param>
        /// <returns></returns>
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            return vector - planeNormal * Vector3.Dot(vector, planeNormal) / Vector3.Dot(planeNormal, planeNormal);
        }

        public static Vector3 ProjectOntoPlane(this Vector3 vector, Vector3 planeNormal)
        {
            return (vector - Vector3.Dot(vector, planeNormal) * planeNormal);
        }

        /// <summary>是否接近0</summary>
        /// <param name="v"></param>
        /// <returns>True if the square magnitude of the vector is within Epsilon of zero</returns>
        public static bool AlmostZero(this Vector3 v)
        {
            return v.sqrMagnitude < (ExtensionVector.Epsilon * ExtensionVector.Epsilon);
        }

        /// <summary>
        /// 移动到某个点
        /// </summary>
        /// <param name="srcPos">原位置</param>
        /// <param name="tarPos">目标位置</param>
        /// <param name="curSpeed">当前速度</param>
        /// <param name="deltaTime">时间差值</param>
        /// <param name="isReach">是否到达</param>
        /// <returns></returns>
        public static Vector3 MoveTo(this Vector3 srcPos, Vector3 tarPos, float curSpeed, float deltaTime,
            out bool isReach)
        {
            var dir = tarPos - srcPos;
            if (dir.magnitude <= curSpeed * deltaTime)
            {
                isReach = true;
                return tarPos;
            }

            isReach = false;
            return srcPos + dir.normalized * curSpeed * deltaTime;
        }

        public static Vector3 MoveTo(this Vector3 srcPos, Vector3 tarPos, Vector3 curSpeedDir, float deltaTime,
            out bool isReach)
        {
            //var dir = tarPos - srcPos;
            if ((tarPos - srcPos).magnitude <= curSpeedDir.magnitude * deltaTime)
            {
                isReach = true;
                return tarPos;
            }
            else
            {
                var dest_pos = srcPos + curSpeedDir * deltaTime;
                isReach = false;
                return dest_pos;
            }
        }

        public static Vector3 SpeedTo(this Vector3 srcSpeed, Vector3 tarSpeed, float accel, float decel, float angleRatio, float deltaTime)
        {
            var normalize = Vector3.Cross(srcSpeed, tarSpeed);
            var deltaAngle = Vector3.Angle(srcSpeed, tarSpeed);
            var rotate = 180;
            if (deltaAngle <= rotate)
            {
                var tarspeed = srcSpeed.magnitude.SpeedTo(tarSpeed.magnitude, accel, decel, deltaTime);
                return tarSpeed.normalized * tarspeed;
            }
            else
            {
                var tarVec = Quaternion.AngleAxis(rotate, normalize) * srcSpeed;
                var tarspeed = srcSpeed.magnitude.SpeedTo(tarSpeed.magnitude, accel, decel, deltaTime);
                return tarVec.normalized * tarspeed;
            }
        }
    }
}