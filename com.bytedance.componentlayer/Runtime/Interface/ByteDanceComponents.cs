using System;
using System.Collections.Generic;
using ByteDance.Foundation;
using ByteDance.Foundation.Coroutine;
using UnityEngine;

namespace ByteDance.ComLayer
{
    public class ByteDanceComponents:IByteDanceGlobal, IByteDanceComponents
    {
        #region encapsule

        private readonly IDictionary<Type, IByteDanceGlobal> _dicManagers = new Dictionary<Type, IByteDanceGlobal>();
        private readonly IList<IByteDanceGlobal> _arList = new List<IByteDanceGlobal>();

        #endregion

        #region Public

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetManager<T>() where T : IByteDanceGlobal
        {
            IByteDanceGlobal result = null;
            _dicManagers.TryGetValue(typeof(T), out result);
            if (result != null)
                return (T)result;

            return default(T);
        }

        /// <summary>
        /// Adds the manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="globalComponent">Name of the type.</param>
        /// <returns></returns>
        public bool AddManager<T>(T globalComponent) where T : IByteDanceGlobal
        {
            IByteDanceGlobal result = null;
            _dicManagers.TryGetValue(typeof(T), out result);
            if (result != null)
                return false;

            _dicManagers.Add(typeof(T), globalComponent);
            _arList.Add(globalComponent);
            Const.Log.LogInfo("ByteDanceGlobal ready to Awake Component : " + typeof(T).Name);
            try
            {
                globalComponent.Awake();
            }
            catch (Exception e)
            {
                Const.Log.LogException(e);
            }

            return true;
        }

        /// <summary>
        /// Removes the manager.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        public bool RemoveManager(Type type)
        {
            if (!_dicManagers.ContainsKey(type))
            {
                Const.Log.LogError("ByteDanceGlobal::RemoveManager failed, cannot find " + type.Name);
                return false;
            }

            IByteDanceGlobal globalComponent;
            _dicManagers.TryGetValue(type, out globalComponent);
            _arList.Remove(globalComponent);
            _dicManagers.Remove(type);
            if (globalComponent!= null)
                globalComponent.Dispose();
            return true;
        }

        public void Awake()
        {
            var coroutine = CoroutineManager.Instance;
        }

        public void Update()
        {
            for (int index = 0; index < _arList.Count; index++)
            {
                try
                {
                    _arList[index].Update();
                }
                catch (Exception e)
                {
                    MyLogger.Log.LogError($"Update Exception : {e.ToString()}");
                }
            }
            //DelegateX.InvokeSafely(_arList[index].Update);
        }

        public void Dispose()
        {
            var count = _arList.Count;
            for (int index = 0; index < count; index++)
            {
                var component = _arList[0];
                RemoveManager(component.GetType());
            }
        }

        #endregion
    }
}