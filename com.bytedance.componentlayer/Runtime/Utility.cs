using ByteDance.Foundation;
using System.Collections.Generic;
using UnityEngine;

namespace ByteDance.ComLayer
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Gets the or create globals node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T GetOrCreateGlobalsNode<T>(string name) where T : MonoBehaviour
        {
            var go = HierachyUtility.GetByHierachyPath(FoundationConst.NameGlobals, name);
            if (go == null)
                go = HierachyUtility.GetOrCreateByHierachyPath(FoundationConst.NameGlobals, name);
            return go.AddComponentSafely<T>();
        }

        public static string GetGameObjectPath(Transform curr)
        {
            var nameStack = new Stack<string>();
            nameStack.Push(curr.name);

            while (curr.parent)
            {
                nameStack.Push(curr.parent.name);

                curr = curr.parent;
            }

            string fullPath = "/";
            while (nameStack.Count > 0)
            {
                fullPath += nameStack.Pop();

                if (nameStack.Count > 0)
                    fullPath += "/";
            }

            return fullPath;
        }

        public static int TraceTopLevel(Transform inMe)
        {
            int count = 0;
            var curr = inMe;

            while (curr.parent)
            {
                count++;
                curr = curr.parent;
            }

            return count;
        }

        public static Transform TraceTop(Transform inMe)
        {
            var curr = inMe;

            while (curr.parent)
            {
                curr = curr.parent;
            }

            return curr;
        }
    }
}