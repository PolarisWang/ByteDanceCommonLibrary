namespace ByteDance.Foundation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ByteDancePoolManager" />.
    /// </summary>
    public class ByteDancePoolManager : IByteDancePoolManager, IDisposable
    {
        /// <summary> Defines the _pools. </summary>
        private readonly Dictionary<Type, ByteDancePoolList<ByteDancePoolItem>> _pools =
            new Dictionary<Type, ByteDancePoolList<ByteDancePoolItem>>();

        /// <summary> The PeekToUse. </summary>
        public TItem PeekToUse<TItem>() where TItem : ByteDancePoolItem, new()
        {
            Type t = typeof(TItem);
            return PeekToUse(t) as TItem;
        }

        /// <summary> Peeks to use. </summary>
        public ByteDancePoolItem PeekToUse(Type t)
        {
            ByteDancePoolList<ByteDancePoolItem> list = null;
            list = _pools.GetOrDefault(t, null);
            if (list == null)
            {
                list = new ByteDancePoolList<ByteDancePoolItem>();
                _pools.Add(t, list);
            }

            var item = list.PeekToUse();
            if (item == null)
            {
                item = System.Activator.CreateInstance(t) as ByteDancePoolItem;
                item._poolList = list;
                list.PushItem(item, EPoolListState.Using);
            }

            return item;
        }

        /// <summary>
        /// The Dispose.
        /// </summary>
        public void Dispose()
        {
            foreach (var pair in _pools)
                pair.Value.Dispose();
            _pools.Clear();
        }
    }
}
