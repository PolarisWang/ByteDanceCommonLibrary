using UnityEngine;

namespace ByteDance.Foundation { 
    public static class ExtensionColor
    {
        /// <summary>
        /// Colors to hexadecimal.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static string ColorToHex(this Color color)
        {
            Color32 c = color;
            var hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
            return hex;
        }

        public static Color HexToColor(this string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static Color Lerp(this Color from, Color to, float t)
        {
            float r = Mathf.Lerp(from.r, to.r, t);
            float g = Mathf.Lerp(from.g, to.g, t);
            float b = Mathf.Lerp(from.b, to.b, t);
            float a = Mathf.Lerp(from.a, to.a, t);
            return new Color(r, g, b, a);
        }

        public static Vector3 ToVector3(this Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }

        public static Color Lerp(Color from, Color to, params float[] t)
        {
            float r = Mathf.Lerp(from.r, to.r, t[0]);
            float g = Mathf.Lerp(from.g, to.g, t[1]);
            float b = Mathf.Lerp(from.b, to.b, t[2]);
            float a = Mathf.Lerp(from.a, to.a, t[3]);
            return new Color(r, g, b, a);
        }
    }
}
