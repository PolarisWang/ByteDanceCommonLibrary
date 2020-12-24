using UnityEngine;

namespace ByteDance.Foundation
{
    /// <summary>
    /// Extensions for rotating in Unity
    /// </summary>
    public static class UnityRotateExtensions
    {
        #region Rotate_DegreesPerSecond

        /// <summary>
        /// Rotates the game object around <paramref name="direction"/> every <paramref name="timeInSeconds"/>.
        /// Using 1 for <paramref name="timeInSeconds"/> will result in the object rotating <paramref name="direction"/> degress in one second.
        /// NOTE: Must be called from Update()
        /// </summary>
        /// <param name="go"></param>
        /// <param name="direction"></param>
        /// <param name="timeInSeconds"></param>
        public static void Rotate_DegreesPerSecond(this GameObject go, Vector3 direction, float timeInSeconds)
        {
            Rotate_DegreesPerSecond(go.transform, direction, timeInSeconds);
        }

        /// <summary>
        /// Rotates the game object around <paramref name="direction"/> every <paramref name="timeInSeconds"/>.
        /// Using 1 for <paramref name="timeInSeconds"/> will result in the object rotating <paramref name="direction"/> degress in one second.
        /// NOTE: Must be called from Update()
        /// </summary>
        /// <param name="goTrans"></param>
        /// <param name="direction"></param>
        /// <param name="timeInSeconds"></param>
        public static void Rotate_DegreesPerSecond(this Transform goTrans, Vector3 direction, float timeInSeconds)
        {
            goTrans.Rotate(direction * timeInSeconds * Time.deltaTime);
        }

        // Rotate_DegreesPerSecond

        #endregion

        #region RotateAroundAxis

        #region RotateAroundAxis_X

        /// <summary>
        /// Rotates the game object around the X axis, rotating <paramref name="degrees"/> every <paramref name="timeInSeconds"/>.
        /// Using 1 for <paramref name="timeInSeconds"/> will result in the object rotating <paramref name="degrees"/> in one second.
        /// NOTE: Must be called from Update()
        /// </summary>
        /// <param name="go"></param>
        /// <param name="degrees"></param>
        /// <param name="timeInSeconds"></param>
        public static void RotateAroundAxis_X(this GameObject go, float degrees, float timeInSeconds)
        {
            RotateAroundAxis_X(go.transform, degrees, timeInSeconds);
        }

        /// <summary>
        /// Rotates the game object around the X axis, rotating <paramref name="degrees"/> every <paramref name="timeInSeconds"/>.
        /// Using 1 for <paramref name="timeInSeconds"/> will result in the object rotating <paramref name="degrees"/> in one second.
        /// NOTE: Must be called from Update()
        /// </summary>
        /// <param name="goTrans"></param>
        /// <param name="degrees"></param>
        /// <param name="timeInSeconds"></param>
        public static void RotateAroundAxis_X(this Transform goTrans, float degrees, float timeInSeconds)
        {
            Rotate_DegreesPerSecond(goTrans, new Vector3(degrees, 0, 0), timeInSeconds);
        }

        // RotateAroundAxis_X

        #endregion

        #region RotateAroundAxis_Y

        /// <summary>
        /// Rotates the game object around the Y axis, rotating <paramref name="degrees"/> every <paramref name="timeInSeconds"/>.
        /// Using 1 for <paramref name="timeInSeconds"/> will result in the object rotating <paramref name="degrees"/> in one second.
        /// NOTE: Must be called from Update()
        /// </summary>
        /// <param name="go"></param>
        /// <param name="degrees"></param>
        /// <param name="timeInSeconds"></param>
        public static void RotateAroundAxis_Y(this GameObject go, float degrees, float timeInSeconds)
        {
            RotateAroundAxis_Y(go.transform, degrees, timeInSeconds);
        }

        /// <summary>
        /// Rotates the game object around the Y axis, rotating <paramref name="degrees"/> every <paramref name="timeInSeconds"/>.
        /// Using 1 for <paramref name="timeInSeconds"/> will result in the object rotating <paramref name="degrees"/> in one second.
        /// NOTE: Must be called from Update()
        /// </summary>
        /// <param name="goTrans"></param>
        /// <param name="degrees"></param>
        /// <param name="timeInSeconds"></param>
        public static void RotateAroundAxis_Y(this Transform goTrans, float degrees, float timeInSeconds)
        {
            Rotate_DegreesPerSecond(goTrans, new Vector3(0, degrees, 0), timeInSeconds);
        }

        // RotateAroundAxis_Y

        #endregion

        #region RotateAroundAxis_Z

        /// <summary>
        /// Rotates the game object around the Z axis, rotating <paramref name="degrees"/> every <paramref name="timeInSeconds"/>.
        /// Using 1 for <paramref name="timeInSeconds"/> will result in the object rotating <paramref name="degrees"/> in one second.
        /// NOTE: Must be called from Update()
        /// </summary>
        /// <param name="go"></param>
        /// <param name="degrees"></param>
        /// <param name="timeInSeconds"></param>
        public static void RotateAroundAxis_Z(this GameObject go, float degrees, float timeInSeconds)
        {
            RotateAroundAxis_Z(go.transform, degrees, timeInSeconds);
        }

        /// <summary>
        /// Rotates the game object around the Z axis, rotating <paramref name="degrees"/> every <paramref name="timeInSeconds"/>.
        /// Using 1 for <paramref name="timeInSeconds"/> will result in the object rotating <paramref name="degrees"/> in one second.
        /// NOTE: Must be called from Update()
        /// </summary>
        /// <param name="goTrans"></param>
        /// <param name="degrees"></param>
        /// <param name="timeInSeconds"></param>
        public static void RotateAroundAxis_Z(this Transform goTrans, float degrees, float timeInSeconds)
        {
            Rotate_DegreesPerSecond(goTrans, new Vector3(0, 0, degrees), timeInSeconds);
        }

        // RotateAroundAxis_Z

        #endregion

        // RotateAroundAxis

        #endregion

        #region LookAt

        /// <summary>
        /// No matter where the <paramref name="targetGo"/> goes, rotate toward him, like a gun turret following a target.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="targetGo">object to look at</param>
        public static void LookAt(this GameObject go, GameObject targetGo)
        {
            LookAt(go, targetGo.transform);
        }

        /// <summary>
        /// No matter where the <paramref name="targetTrans"/> goes, rotate toward him, like a gun turret following a target.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="targetTrans">object to look at</param>
        public static void LookAt(this GameObject go, Transform targetTrans)
        {
            go.transform.LookAt(targetTrans);
        }

        /// <summary>
        /// No matter where the <paramref name="targetVector"/> goes, rotate toward him, like a gun turret following a target.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="targetVector">object to look at</param>
        public static void LookAt(this GameObject go, Vector3 targetVector)
        {
            go.transform.LookAt(targetVector);
        }

        // LookAt

        #endregion


        /// <summary>This is a slerp that mimics a camera operator's movement in that
        /// it chooses a path that avoids the lower hemisphere, as defined by
        /// the up param</summary>
        /// <param name="qA">First direction</param>
        /// <param name="qB">Second direction</param>
        /// <param name="t">Interpolation amoun t</param>
        /// <param name="up">Defines the up direction.  Must have a length of 1.</param>
        public static Quaternion SlerpWithReferenceUp(
            Quaternion qA, Quaternion qB, float t, Vector3 up)
        {
            Vector3 dirA = (qA * Vector3.forward).ProjectOntoPlane(up);
            Vector3 dirB = (qB * Vector3.forward).ProjectOntoPlane(up);
            if (dirA.AlmostZero() || dirB.AlmostZero())
                return Quaternion.Slerp(qA, qB, t);

            // Work on the plane, in eulers
            Quaternion qBase = Quaternion.LookRotation(dirA, up);
            Quaternion qA1 = Quaternion.Inverse(qBase) * qA;
            Quaternion qB1 = Quaternion.Inverse(qBase) * qB;
            Vector3 eA = qA1.eulerAngles;
            Vector3 eB = qB1.eulerAngles;
            return qBase * Quaternion.Euler(
                       Mathf.LerpAngle(eA.x, eB.x, t),
                       Mathf.LerpAngle(eA.y, eB.y, t),
                       Mathf.LerpAngle(eA.z, eB.z, t));
        }

        /// <summary>
        /// Get the rotations, first about world up, then about (travelling) local right,
        /// necessary to align the quaternion's forward with the target direction.
        /// This represents the tripod head movement needed to look at the target.
        /// This formulation makes it easy to interpolate without introducing spurious roll.
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="lookAtDir">The worldspace target direction in which we want to look</param>
        /// <param name="worldUp">Which way is up.  Must have a length of 1.</param>
        /// <returns>Vector2.y is rotation about worldUp, and Vector2.x is second rotation,
        /// about local right.</returns>
        public static Vector2 GetCameraRotationToTarget(
            this Quaternion orient, Vector3 lookAtDir, Vector3 worldUp)
        {
            if (lookAtDir.AlmostZero())
                return Vector2.zero;  // degenerate

            // Work in local space
            Quaternion toLocal = Quaternion.Inverse(orient);
            Vector3 up = toLocal * worldUp;
            lookAtDir = toLocal * lookAtDir;

            // Align yaw based on world up
            float angleH = 0;
            {
                Vector3 targetDirH = lookAtDir.ProjectOntoPlane(up);
                if (!targetDirH.AlmostZero())
                {
                    Vector3 currentDirH = Vector3.forward.ProjectOntoPlane(up);
                    if (currentDirH.AlmostZero())
                    {
                        // We're looking at the north or south pole
                        if (Vector3.Dot(currentDirH, up) > 0)
                            currentDirH = Vector3.down.ProjectOntoPlane(up);
                        else
                            currentDirH = Vector3.up.ProjectOntoPlane(up);
                    }
                    angleH = ExtensionVector.SignedAngle(currentDirH, targetDirH, up);
                }
            }
            Quaternion q = Quaternion.AngleAxis(angleH, up);

            // Get local vertical angle
            float angleV = ExtensionVector.SignedAngle(
                q * Vector3.forward, lookAtDir, q * Vector3.right);

            return new Vector2(angleV, angleH);
        }

        /// <summary>
        /// Apply rotations, first about world up, then about (travelling) local right.
        /// rot.y is rotation about worldUp, and rot.x is second rotation, about local right.
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="rot">Vector2.y is rotation about worldUp, and Vector2.x is second rotation,
        /// about local right.</param>
        /// <param name="worldUp">Which way is up</param>
        public static Quaternion ApplyCameraRotation(
            this Quaternion orient, Vector2 rot, Vector3 worldUp)
        {
            Quaternion q = Quaternion.AngleAxis(rot.x, Vector3.right);
            return (Quaternion.AngleAxis(rot.y, worldUp) * orient) * q;
        }
    }
}