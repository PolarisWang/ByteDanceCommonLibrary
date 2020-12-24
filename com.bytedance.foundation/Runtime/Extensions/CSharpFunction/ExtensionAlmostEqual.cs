using System.Runtime.CompilerServices;
using UnityEngine;

namespace ByteDance.Foundation.CSharpFunction
{
    public static class ExtensionAlmostEqual
    {
        /// <summary>compares the squared magnitude of target - second to given float value</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        public static bool AlmostEquals(this Vector3 target, Vector3 second, float sqrMagnitudePrecision)
        {
            return (target - second).sqrMagnitude < sqrMagnitudePrecision;
        }

        /// <summary>compares the squared magnitude of target - second to given float value</summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        public static bool AlmostEquals(this Vector2 target, Vector2 second, float sqrMagnitudePrecision)
        {
            return (target - second).sqrMagnitude < sqrMagnitudePrecision;
        }

        /// <summary>compares the angle between target and second to given float value</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        public static bool AlmostEquals(this Quaternion target, Quaternion second, float maxAngle)
        {
            return Quaternion.Angle(target, second) < maxAngle;
        }

        /// <summary>compares two floats and returns true of their difference is less than floatDiff</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        public static bool AlmostEquals(this float target, float second, float floatDiff)
        {
            return Mathf.Abs(target - second) < floatDiff;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Implement in C# 4.0
        public static bool AlmostEquals(this float target, float second)
        {
            return Mathf.Approximately(target, second);
        }
    }

}