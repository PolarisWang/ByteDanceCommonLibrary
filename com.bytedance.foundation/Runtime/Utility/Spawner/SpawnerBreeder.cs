using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ByteDance.Foundation
{
    /// <summary>
    /// Handles spawning of game objects
    /// </summary>
    public class SpawnerBreeder : IDisposable
    {
        /// <summary>
        /// List of spawned objects
        /// </summary>
        private readonly Dictionary<string, SpawnerPool> SpawnPoolDict = new Dictionary<string, SpawnerPool>();

        /// <summary>
        /// Create a pool to hold a set of spawned objects
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="resObj"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public SpawnerPool Create(string poolName, Object resObj, GameObject parent)
        {
            SpawnerPool pool = new SpawnerPool(poolName, resObj, parent);
            SpawnPoolDict.Add(poolName, pool);
            return pool;
        }

        /// <summary>
        /// Remove the spawn pool
        /// </summary>
        /// <param name="poolName"></param>
        public void Remove(string poolName)
        {
            // remove, if it exists
            if (SpawnPoolDict.ContainsKey(poolName))
            {
                SpawnPoolDict[poolName].Dispose();
                SpawnPoolDict.Remove(poolName);
            }
        }

        /// <summary>
        /// Remove all spawn pools
        /// </summary>
        public void RemoveAll()
        {
            SpawnPoolDict.Clear();
        }

        /// <summary>
        /// Returns the pool of this name, if it exists
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public SpawnerPool Get(string poolName)
        {
            return SpawnPoolDict.ContainsKey(poolName) ? SpawnPoolDict[poolName] : null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var pair in SpawnPoolDict)
                DelegateX.InvokeSafely(pair.Value.Dispose);
            SpawnPoolDict.Clear();
        }
    }
}