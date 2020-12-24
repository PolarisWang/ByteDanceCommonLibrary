using System;
using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public static class ExtensionQueue
    {
        /// <summary>
        /// deques an item, or returns null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static T DequeueOrNull<T>(this Queue<T> q)
        {
            try
            {
                return (q.Count > 0) ? q.Dequeue() : default(T);
            }

            catch (Exception)
            {
                return default(T);
            }
        }
    }
}