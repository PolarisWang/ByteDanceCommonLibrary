namespace ByteDance.Foundation
{
    public static class ExtensionArray
    {
        public static bool Contains<T>(this T[] array, T item)
        {
            if (array == null)
                return false;

            foreach (var content in array)
            {
                if (content.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static T[] Append<T>(this T[] array, T[] appendArray)
        {
            T[] result = new T[array.Length + appendArray.Length];
            for (int index = 0; index < array.Length; index++)
                result[index] = array[index];
            for (int index = 0; index < appendArray.Length; index++)
                result[array.Length + index] = appendArray[index];
            return result;
        }

        /// <summary>
        /// Returns true if the array is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this T[] data)
        {
            return ((data == null) || (data.Length == 0));
        }
    }
}