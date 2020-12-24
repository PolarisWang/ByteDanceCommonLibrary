/**************************************************************
 * FileName:	HierachyUtility.cs
 * Author:		王浩川
 * CreateTime:  2018-06-03 20:16
 * Copyright:   ByteDance
 * Description: 
 ***************************************************************/

using UnityEngine;

namespace ByteDance.Foundation
{
    public static class HierachyUtility
    {
        /// <summary>
        /// Gets the by hierachy path.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        public static GameObject GetByHierachyPath(params string[] param)
        {
            string result = "";
            for (int index = 0; index < param.Length; index++)
            {
                var content = param[index];
                if (string.IsNullOrEmpty(content))
                    continue;
                content = content.Replace("/", "");
                if (index > 0)
                    result += "/";
                result += content;
            }
            return GameObject.Find(result);
        }

        /// <summary>
        /// Gets the or create by hierachy path.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        public static GameObject GetOrCreateByHierachyPath(params string[] param)
        {
            GameObject current = null;
            for (int index = 0; index < param.Length; index++)
            {
                var content = param[index];
                if (string.IsNullOrEmpty(content))
                    continue;

                content = content.Replace("/", "");
                GameObject child = null;
                bool bNeedCreate = false;
                if (index == 0)
                {
                    child = GameObject.Find("/" + content);
                    bNeedCreate = child == null;
                    if (!bNeedCreate)
                    {
                        current = child;
                        continue;
                    }
                }
                else
                {
                    var childTrans = current.transform.Find(content);
                    bNeedCreate = childTrans == null;
                    if (!bNeedCreate)
                    {
                        current = childTrans.gameObject;
                        continue;
                    }
                }

                if (bNeedCreate)
                {
                    GameObject go = new GameObject(content);
                    go.transform.parent = current != null ? current.transform : null;
                    go.transform.Reset();
                    current = go;
                }
            }

            return current;
        }
    }
}