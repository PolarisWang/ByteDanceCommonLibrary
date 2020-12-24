using UnityEngine;

#if UNITY_EDITOR
//#define POOL_DEBUG
#endif

namespace ByteDance.Foundation
{
    using System.Collections.Generic;

    using System;

    /// <summary>
    /// Defines the <see cref="ByteDancePoolList" />.
    /// </summary>
    [Serializable]
    public sealed class ByteDancePoolList<TItem> : IDisposable, IByteDancePoolListInterface, IByteDancePoolList<TItem> where TItem : ByteDancePoolItem
    {
#if POOL_DEBUG
        [SerializeField] internal readonly List<ByteDancePoolItem> _allItems = new List<ByteDancePoolItem>();
#endif
        /// <summary>
        /// Defines the _usableList.
        /// </summary>
        [SerializeField] internal readonly Stack<ByteDancePoolItem> _usableList = new Stack<ByteDancePoolItem>();

        /// <summary>
        /// The PeekToUse.
        /// </summary>
        /// <returns>The <see cref="IByteDancePoolItem"/>.</returns>
        public TItem PeekToUse()
        {
            if (_usableList.Count > 0)
            {
                var result = _usableList.Pop();
                result._poolList = this;
                return result as TItem;
            }

            return default(TItem);
        }
        
        /// <summary>
        /// The PushItem.
        /// </summary>
        /// <param name="item">The item<see cref="IByteDancePoolItem"/>.</param>
        /// <param name="state">The state<see cref="EPoolListState"/>.</param>
        public void PushItem(TItem item, EPoolListState state)
        {
#if POOL_DEBUG
            if (_allItems.Contains(item))
                ByteDancePoolConst.Logger.LogError("PushItem failed, all item already contain this item");
            else
                _allItems.Add(item);
#endif
            if (state == EPoolListState.Idle)
                _usableList.Push(item);

            item._poolList = this;
        }

        public void Dispose()
        {
#if POOL_DEBUG
            _allItems.Clear();
#endif
            _usableList.Clear();
        }

        public void Return(ByteDancePoolItem item)
        {
#if POOL_DEBUG
            if (!_allItems.Contains(item))
                ByteDancePoolConst.Logger.LogError("Return failed, all item not contain this item");
#endif
            _usableList.Push(item);
            item.OnReturnToPool();
        }
    }
}
