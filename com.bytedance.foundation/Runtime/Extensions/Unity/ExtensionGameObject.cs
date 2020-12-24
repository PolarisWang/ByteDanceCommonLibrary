using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ByteDance.Foundation
{
    /// <summary>
    /// Unity extensions on GameObjects
    /// </summary>
    public static class ExtensionGameObject
    {
        #region GetComponentOnObject

        /// <summary>
        /// Returns a component attached to the game object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="showErrorInConsole">when true, logs an error in the console if nothing found</param>
        /// <returns></returns>
        public static T GetComponentOnObject<T>(this GameObject go, bool showErrorInConsole) where T : Component
        {
            // get the component
            T component = go.GetComponent<T>();
            if ((showErrorInConsole) && (component == null))
                Debug.LogError(string.Format("Unable to find component '{0}' on object '{1}'", typeof(T).Name,
                    go.name));

            return component;
        }

        /// <summary>
        /// Returns a component attached to the game object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trans"></param>
        /// <param name="showErrorInConsole">when true, logs an error in the console if nothing found</param>
        /// <returns></returns>
        public static T GetComponentOnObject<T>(this Transform trans, bool showErrorInConsole) where T : Component
        {
            // get the component
            T component = trans.GetComponent<T>();
            if ((showErrorInConsole) && (component == null))
                Debug.LogError(string.Format("Unable to find component '{0}' on object '{1}'", typeof(T).Name,
                    trans.name));

            return component;
        }

        // GetComponentOnObject

        #endregion

        #region GetComponentOnObjectOrParent

        /// <summary>
        /// Returns a component attached to the game object, or its parent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="showErrorInConsole">when true, logs an error in the console if nothing found</param>
        /// <returns></returns>
        public static T GetComponentOnObjectOrParent<T>(this GameObject go, bool showErrorInConsole) where T : Component
        {
            // get the component
            T component = go.GetComponentInParent<T>();
            if ((showErrorInConsole) && (component == null))
                Debug.LogError(string.Format("Unable to find component '{0}' on object (or parent) '{1}'",
                    typeof(T).Name, go.name));

            return component;
        }

        // GetComponentOnObjectOrParent

        #endregion

        #region IsNullOrInactive

        /// <summary>
        /// Returns true if the GO is null or inactive
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool IsNullOrInactive(this GameObject go)
        {
            return ((go == null) || (!go.activeSelf));
        }

        // IsNullOrInactive

        #endregion

        #region IsActive

        /// <summary>
        /// Returns true if the GO is not null and is active
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool IsActive(this GameObject go)
        {
            return ((go != null) && (go.activeSelf) && go.activeInHierarchy);
        }

        // IsActive

        #endregion

        #region ActivateAndParent

        /// <summary>
        /// Activates this gameobject, starting with its parent
        /// </summary>
        /// <param name="go"></param>
        public static void ActivateAndParent(this GameObject go)
        {
            // exit if go is null
            if (go == null)
                return;

            // if this object has a parent, activate each parent first
            if (go.transform.parent != null)
                go.transform.parent.gameObject.ActivateAndParent();

            // activate this object
            go.SetActive(true);
        }

        // ActivateAndParent

        #endregion

        #region HasRigidbody

        /// <summary>
        /// Returns true if the object has a rigid body
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool HasRigidbody(this GameObject go)
        {
            return (go.GetComponent<Rigidbody>() != null);
        }

        // HasRigidbody

        #endregion

        #region HasCharacterController

        /// <summary>
        /// Returns true if the object has a character controller
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool HasCharacterController(this GameObject go)
        {
            return (go.GetComponent<CharacterController>() != null);
        }

        // HasCharacterController

        #endregion

        #region HasComponent

        /// <summary>
        /// Returns true if the game object has this component
        /// </summary>
        /// <param name="go"></param>
        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            return (go.GetComponent<T>() != null);
        }

        // HasComponent

        #endregion

        #region SetLayerRecursively

        /// <summary>
        /// Sets the layer for the game object and all its children
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform t in go.transform)
                t.gameObject.SetLayerRecursively(layer);
        }

        // SetLayerRecursively

        #endregion

        #region SetCollisionRecursively

        /// <summary>
        /// Enables or disables colliders on the game object and all its children
        /// </summary>
        /// <param name="go"></param>
        /// <param name="enabled"></param>
        public static void SetCollisionRecursively(this GameObject go, bool enabled)
        {
            Collider GCollide = go.GetComponent<Collider>();
            if (GCollide != null)
                GCollide.enabled = enabled;

            foreach (Transform t in go.transform)
                t.gameObject.SetCollisionRecursively(enabled);
        }

        // SetCollisionRecursively

        #endregion

        #region GetComponentsInChildrenWithTag

        /// <summary>
        /// Returns all components in the game object and children with the specified tag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static List<T> GetComponentsInChildrenWithTag<T>(this GameObject go, string tag) where T : Component
        {
            List<T> results = new List<T>();

            // check if the object has this tag
            if (go.CompareTag(tag))
                results.Add(go.GetComponent<T>());

            // loop through all children with this tag
            foreach (Transform t in go.transform)
                results.AddRange(t.gameObject.GetComponentsInChildrenWithTag<T>(tag));

            return results;
        }

        // GetComponentsInChildrenWithTag

        #endregion

        #region GetCollisionMask

        /// <summary>
        /// Returns the collision mask of the game object (all layers which this object can collide with)
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer">OPTIONAL. If omitted, it uses the layer of the calling GameObject, which is the most common/intuitive case (for me, at least). But you can specify a layer and it’ll hand you the collision mask for that layer instead.</param>
        /// <returns></returns>
        public static int GetCollisionMask(this GameObject go, int layer = -1)
        {
            // get the layer if one was not sent
            if (layer == -1)
                layer = go.layer;

            // get the mask on this layer
            int mask = 0;
            for (int i = 0; i < 32; i++)
                mask |= (Physics.GetIgnoreLayerCollision(layer, i) ? 0 : 1) << i;

            return mask;
        }

        // GetCollisionMask

        #endregion

        #region GetChildrenWithName

        /// <summary>
        /// Returns all children of the game object with the specified name
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <remarks>
        /// Suggested by: Vipsu
        /// Link: http://forum.unity3d.com/members/vipsu.138664/
        /// </remarks>
        //public static Transform[] GetChildrenWithName(this GameObject go, string name)
        //{
        //    // loop through and add matching children
        //    return go.transform.Select<Transform>()
        //        .Where(w => w.name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
        //        .Return(r=> r.ToArray(), (Transform[]) null);
        //}

        // GetChildrenWithName

        #endregion

     
        public static GameObject NewGameObjectAsChild(this GameObject go,  string name)
        {
            if (go == null)
                return null;

            GameObject newGo = new GameObject(name);
            newGo.transform.parent = go.transform;
            newGo.transform.Reset();
            return newGo;
        }

        public static void ResetTransform(this GameObject go)
        {
            if (go != null)
                go.transform.Reset();
        }

        public static GameObject Clone(this GameObject obj)
        {
            GameObject newObj = Object.Instantiate(obj) as GameObject;
            newObj.transform.parent = obj.transform.parent;
            newObj.transform.CopyFrom(obj.transform);
            return newObj;
        }

        public static void SetActiveSafely(this GameObject obj, bool active)
        {
            if (obj != null)
                obj.SetActive(active);
        }

        public static void SetActiveRecursivelyExt(this GameObject target, bool bActive)
        {
            for (int n = target.transform.childCount - 1; 0 <= n; n--)
                if (n < target.transform.childCount)
                    SetActiveRecursivelyExt(target.transform.GetChild(n).gameObject, bActive);
            target.SetActive(bActive);
        }

        public static void RemoveAllChildObject(this GameObject parent, bool bImmediate)
        {
            for (int n = parent.transform.childCount - 1; 0 <= n; n--)
            {
                if (n < parent.transform.childCount)
                {
                    Transform obj = parent.transform.GetChild(n);
                    if (bImmediate)
                        Object.DestroyImmediate(obj.gameObject);
                    else Object.Destroy(obj.gameObject);
                }
            }
        }

        public static T AddComponentSafely<T>(this GameObject pGameObject) where T : Component
        {
            T l_component = pGameObject.GetComponent<T>();
            if (l_component == null)
            {
                l_component = pGameObject.AddComponent<T>();
            }
            return l_component;
        }


        #region ToStringParentHierarchy

        /// <summary>
        /// Returns the name of the parent objects
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static string ToStringParentHierarchy(this GameObject go)
        {
            // exit if null
            if (go == null)
                return string.Empty;

            string ReturnName = string.Empty;

            // get the parent name first
            if (go.transform.parent != null)
                ReturnName = go.transform.parent.gameObject.ToStringParentHierarchy();

            // add this game oject to the return string
            return string.Format("{0}{1}",
                (!string.IsNullOrEmpty(ReturnName)) ? string.Format("{0} > ", ReturnName) : string.Empty,
                go.name);
        }

        /// <summary>
        /// To the string related path.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string ToStringRelatedPath(this GameObject obj)
        {
            List<string> pathComponents = new List<string>();
            Transform target = obj.transform;
            while (target != null)
            {
                pathComponents.Add(target.gameObject.name);
                target = target.parent;
            }

            pathComponents.Reverse();

            StringBuilder sb = new StringBuilder();
            foreach (string pathComponent in pathComponents)
                sb.Append(pathComponent).Append('/');
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        #endregion
    }
}