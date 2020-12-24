using System;
using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public static class ExtensionList
    {
#pragma warning disable 649
        public static bool IsCorrectIndex<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        /// <summary>
        /// Returns true if the list is null or empty
        /// </summary>
        /// <typeparam name="T">Template</typeparam>
        /// <param name="list">List</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static void RobustAdd<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static string ToRmText<T>(this IList<T> list)
        {
            if (list == null)
                return "NULL";
            else
            {
                int index = 0;
                string result = string.Empty;
                foreach (var text in list)
                {
                    var _text = (text == null) ? "Null" : text.ToString();

                    if (index == 0)
                        result += _text;
                    else
                        result += "," + _text;
                    index++;
                }
                return result;
            }
        }

        public static T GetOrDefault<T>(this IList<T> list, int index, T t = default(T))
        {
            return list.IsCorrectIndex(index) ? list[index] : t;
        }

        /// <summary>
        /// 返回第一个Item
        /// </summary>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T First<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                return default(T);
            else
                return list[0];
        }

        public static T Last<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                return default(T);
            else
                return list[list.Count - 1];
        }

        public static void AddAt<T>(this IList<T> list, int index, T v)
        {
            if (index < list.Count)
            {
                list[index] = v;
            }
            else
            {
                int count = index - list.Count;
                for (int i = 0; i < count; ++i)
                {
                    list.Add(default(T));
                }
                list.Add(v);
            }
        }

        /// <summary>
        /// 返回有序序列中第一个大于等于比较器的下标,类似std::lower_bound
        /// http://en.cppreference.com/w/cpp/algorithm/lower_bound
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int LowerBound<T>(this IList<T> list, Func<T, int> comparer)
        {
            int first = 0;
            int count = list.Count;
            int it, step;

            while (count > 0)
            {
                it = first;
                step = count / 2;
                it += step;
                if (comparer(list[it]) < 0)
                {
                    first = ++it;
                    count -= step + 1;
                }
                else
                {
                    count = step;
                }
            }
            return first;
        }

        public static int BinarySearch<T>(this IList<T> list, Func<T, int> comparer)
        {
            int first = list.LowerBound(comparer);
            return first != list.Count && comparer(list[first]) == 0 ? first : -1;
        }

        public static T BinaryFind<T>(this IList<T> list, Func<T, int> comparer)
        {
            int index = list.BinarySearch(comparer);
            return index == -1 ? default(T) : list[index];
        }

        public static void ClearSafely<T>(this IList<T> list)
        {
            if (list != null)
                list.Clear();
        }

        public static void AddSafely<T>(this IList<T> list, T item)
        {
            RobustAdd(list, item);
        }

        public static int FindIndex<T>(this IList<T> items, System.Func<T, bool> func)
        {
            for (int i = 0; i < items.Count; i++)
                if (func(items[i]))
                    return i;
            return -1;
        }
    }
}