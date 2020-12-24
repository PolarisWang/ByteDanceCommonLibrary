using UnityEngine;

namespace ByteDance.Foundation
{
    public static class ExtensionMath {

        public static float SinEular(this float eularAngle)
        {
            return Mathf.Sin(eularAngle.ToPI());
        }

        public static float CosEular(this float eularAngle)
        {
            return Mathf.Cos(eularAngle.ToPI());
        }

        public static float ToPI(this float eularAngle)
        {
            return eularAngle /180.0f * Mathf.PI;
        }

        public static float Lerp(this float t1, float t2, float ratio)
        {
            return t1 + (t2 - t1) * ratio;
        }

        public static float Accel(this float t, float acc, float deltaTime)
        {
            return t + acc * deltaTime;
        }

        //public static float Accel(this float t, float acc, float deltaTime, float tarSpeed)
        //{
        //    var accel = Mathf.Abs(acc);
        //    if (t < tarSpeed)
        //    {
        //        var result = t + accel * deltaTime;
        //        return result > tarSpeed ? tarSpeed : result;
        //    }
        //    else
        //    {
        //        var result = t - accel * deltaTime;
        //        return result < tarSpeed ? tarSpeed : result;
        //    }
        //}

        public static float SpeedTo(this float t, float tarSpeed, float accel, float decel, float deltaTime)
        {
            if (t < tarSpeed)
            {
                accel = Mathf.Abs(accel);
                var result = t + accel * deltaTime;
                return result > tarSpeed ? tarSpeed : result;
            }
            else
            {
                decel = Mathf.Abs(decel);
                var result = t - decel * deltaTime;
                return result < tarSpeed ? tarSpeed : result;
            }
        }
    }
}
