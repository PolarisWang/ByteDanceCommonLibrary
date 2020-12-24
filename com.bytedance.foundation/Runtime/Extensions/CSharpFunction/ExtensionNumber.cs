using System;
using System.Text.RegularExpressions;

namespace ByteDance.Foundation
{
    /// <summary>
    /// Extensions for numbers (ints, longs, floats, decimals, etc)
    /// </summary>
    public static class ExtensionNumber
    {
        #region Decimal

        #region Rounded

        /// <summary>
        /// Rounds a decimal 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal Rounded(this decimal value, int decimals)
        {
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        #endregion

        #endregion

        #region IsNumeric

        /// <summary>
        /// returns true ONLY if the entire string is numeric
        /// </summary>
        /// <param name="input">the string to test</param>
        public static bool IsNumeric(this string input)
        {
            // return false if no string
            return (!String.IsNullOrEmpty(input)) && (new Regex(@"^-?[0-9]*\.?[0-9]+$").IsMatch(input.Trim()));

            //Valid user input
        }

        #endregion IsNumeric

        #region HasNumeric

        /// <summary>
        /// returns true if any part of the string is numeric
        /// </summary>
        /// <param name="input">the string to test</param>
        public static bool HasNumeric(this string input)
        {
            // if no string, return false
            return (!String.IsNullOrEmpty(input)) && (new Regex(@"[0-9]+").IsMatch(input));
        }

        #endregion HasNumeric

        #region Int

        /// <summary>
        /// Negates (* -1) the given integer.
        /// </summary>
        /// <param name="number">The given integer.</param>
        /// <returns>The negated integer.</returns>
        public static int Negate(this int number)
        {
            return number * -1;
        }

        /// <summary>
        /// Strips out the sign and returns the absolute value of given integer.
        /// </summary>
        /// <param name="number">The given integer.</param>
        /// <returns>The absolute value of given integer.</returns>
        public static int AbsoluteValue(this int number)
        {
            return Math.Abs(number);
        }

        // Int

        #endregion

        #region Long

        /// <summary>
        /// Negates (* -1) the given long number.
        /// </summary>
        /// <param name="number">The given long number.</param>
        /// <returns>The negated long number.</returns>
        public static long Negate(this long number)
        {
            return number * -1;
        }

        /// <summary>
        /// Strips out the sign and returns the absolute value of given long number.
        /// </summary>
        /// <param name="number">The given long number.</param>
        /// <returns>The absolute value of given long number.</returns>
        public static long AbsoluteValue(this long number)
        {
            return Math.Abs(number);
        }

        // Long

        #endregion

        #region Float

        /// <summary>
        /// Ratioes the specified to.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="ratio">The ratio.</param>
        /// <returns></returns>
        public static float Ratio(this float from, float to, float ratio)
        {
            return (to - from) * ratio + from;
        }
        #endregion

    }
}