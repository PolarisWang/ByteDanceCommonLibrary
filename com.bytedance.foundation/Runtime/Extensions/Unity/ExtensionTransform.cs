using UnityEngine;

namespace ByteDance.Foundation
{
    /// <summary>
    /// Extensions for transforms and vector3
    /// </summary>
    public static class ExtensionTransform
    {
        #region SetPositionX

        /// <summary>
        /// Sets the X position value
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newX"></param>
        public static void SetPositionX(this Transform t, float newX, bool isLocal = false)
        {
            if (!isLocal)
                t.position = t.position.SetPositionX(newX);
            else
                t.localPosition = t.localPosition.SetPositionX(newX);
        }

        /// <summary>
        /// Sets the X position value
        /// </summary>
        /// <param name="v3"></param>
        /// <param name="newX"></param>
        public static Vector3 SetPositionX(this Vector3 v3, float newX)
        {
            return new Vector3(newX, v3.y, v3.z);
        }
        
        #endregion

        #region SetPositionY

        /// <summary>
        /// Sets the Y position value
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newY"></param>
        public static void SetPositionY(this Transform t, float newY, bool isLocal = false)
        {
            if (!isLocal)
                t.position = t.position.SetPositionY(newY);
            else
                t.localPosition = t.localPosition.SetPositionY(newY);
        }

        /// <summary>
        /// Sets the Y position value
        /// </summary>
        /// <param name="v3"></param>
        /// <param name="newY"></param>
        public static Vector3 SetPositionY(this Vector3 v3, float newY)
        {
            return new Vector3(v3.x, newY, v3.z);
        }

        // SetPositionY

        #endregion

        #region SetPositionZ

        /// <summary>
        /// Sets the Z position value
        /// </summary>
        /// <param name="t"></param>
        /// <param name="newZ"></param>
        public static void SetPositionZ(this Transform t, float newZ, bool isLocal = false)
        {
            if (!isLocal)
                t.position = t.position.SetPositionZ(newZ);
            else
                t.localPosition = t.localPosition.SetPositionZ(newZ);
        }

        /// <summary>
        /// Sets the Z position value
        /// </summary>
        /// <param name="v3"></param>
        /// <param name="newZ"></param>
        public static Vector3 SetPositionZ(this Vector3 v3, float newZ)
        {
            return new Vector3(v3.x, v3.y, newZ);
        }

        // SetPositionZ

        #endregion

        #region GetPositionX

        /// <summary>
        /// Returns X of position
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetPositionX(this Transform t)
        {
            return t.position.x;
        }

        // GetPositionX

        #endregion

        #region GetPositionY

        /// <summary>
        /// Returns Y of position
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetPositionY(this Transform t)
        {
            return t.position.y;
        }

        // GetPositionY

        #endregion

        #region GetPositionZ

        /// <summary>
        /// Returns Z of position
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetPositionZ(this Transform t)
        {
            return t.position.z;
        }

        // GetPositionZ

        #endregion

        public static void SetLocalPosition(this Transform xform, params float[] coords)
        {
            Vector3 v = xform.localPosition;
            ExtensionVector3.Set(ref v, coords);
            xform.localPosition = v;
        }

        public static void SetLocalX(this Transform xform, float x)
        {
            Vector3 v = xform.localPosition;
            v.x = x;
            xform.localPosition = v;
        }

        public static void SetLocalY(this Transform xform, float y)
        {
            Vector3 v = xform.localPosition;
            v.y = y;
            xform.localPosition = v;
        }

        public static void SetLocalZ(this Transform xform, float z)
        {
            Vector3 v = xform.localPosition;
            v.z = z;
            xform.localPosition = v;
        }

        public static void AdjustLocalX(this Transform xform, float xDelta)
        {
            Vector3 v = xform.localPosition;
            v.x += xDelta;
            xform.localPosition = v;
        }

        public static void AdjustLocalY(this Transform xform, float yDelta)
        {
            Vector3 v = xform.localPosition;
            v.y += yDelta;
            xform.localPosition = v;
        }

        public static void AdjustLocalZ(this Transform xform, float zDelta)
        {
            Vector3 v = xform.localPosition;
            v.z += zDelta;
            xform.localPosition = v;
        }

        public static void Flip(this Transform xform, bool isHorizontal)
        {
            Vector3 v = xform.localScale;
            if (isHorizontal)
                v.x *= -1;
            else
                v.y *= -1;
            xform.localScale = v;
        }

        public static void SetLocalPosition(this Transform xform, Vector3 pos, bool includeZ = true)
        {
            if (!includeZ)
                pos.z = xform.localPosition.z;
            xform.localPosition = pos;
        }

        public static void SetPosition(this Transform xform, Vector3 pos, bool includeZ = true)
        {
            if (!includeZ)
                pos.z = xform.position.z;
            xform.position = pos;
        }

        public static void SetLocalScale(this Transform xform, params float[] coords)
        {
            Vector3 v = xform.localScale;
            ExtensionVector3.Set(ref v, coords);
            xform.localScale = v;
        }

        public static void Reset(this Transform xform)
        {
            xform.localPosition = Vector3.zero;
            xform.localScale = Vector3.one;
            xform.localRotation = Quaternion.identity;
        }

        public static void CopyFrom(this Transform xform, Transform rhs)
        {
            xform.localPosition = rhs.localPosition;
            xform.localScale = rhs.localScale;
            xform.localRotation = rhs.localRotation;
        }
    }
}