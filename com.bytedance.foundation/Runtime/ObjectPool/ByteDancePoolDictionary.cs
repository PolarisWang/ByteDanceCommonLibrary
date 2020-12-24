using System;
using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public sealed class ByteDancePoolDictionary<TKey, TItem> : IByteDancePoolRoot<TKey,TItem>
        where TItem : ByteDancePoolItem
    {
        private Func<TKey, TItem> _funcGenerate;
        private Action<TItem> _funcDispose;
        private Dictionary<TKey, ByteDancePoolList<TItem>> _poolList;
        private Dictionary<TItem, TKey> _itemDic;

        #region interface

        public void Init(Func<TKey, TItem> funcGenerate, IEqualityComparer<TKey> comparer = null,
            System.Action<TItem> funcDispose = null)
        {
            _funcGenerate = funcGenerate;
            _poolList = new Dictionary<TKey, ByteDancePoolList<TItem>>(comparer);
            _itemDic = new Dictionary<TItem, TKey>();
            _funcDispose = funcDispose;
        }

        public void Dispose()
        {
            if (_poolList != null && _funcDispose != null)
            {
                try
                {
                    foreach (var pair in _poolList)
                    {
                        var list = pair.Value;
                        foreach (var item in list._usableList)
                            _funcDispose(item as TItem);
                        list.Dispose();
                    }
                }
                catch (Exception e)
                {
                    MyLogger.Log.LogError(e.ToString());
                }
            }

            _funcDispose = null;
            _funcGenerate = null;
            _poolList?.Clear();
            _itemDic?.Clear();
        }

        public TItem PeekToUse(TKey key)
        {
            // Get pool list.
            ByteDancePoolList<TItem> list = _poolList.GetOrDefault(key);
            if (list == null)
            {
                list = new ByteDancePoolList<TItem>();
                _poolList.Add(key, list);
            }

            // Spawn.
            var item = list.PeekToUse();
            if (item != null)
                return item;

            // Create new.
            item = _generate_item_(list, key, EPoolListState.Using);
            return item;
        }

        public void PushItem(TKey key, TItem item, EPoolListState state)
        {
            var list = _poolList.GetOrDefault(key);
            if (list == null)
            {
                list = new ByteDancePoolList<TItem>();
                _poolList.Add(key, list);
            }

            list.PushItem(item, state);
        }

        public void CacheNew(TKey key, int size)
        {
            // Get pool list.
            ByteDancePoolList<TItem> list = null;
            if (_poolList.ContainsKey(key))
                list = _poolList[key];
            else
            {
                list = new ByteDancePoolList<TItem>();
                _poolList.Add(key, list);
            }

            // Create new.
            for (int index = 0; index < size; index++)
                _generate_item_(list, key, EPoolListState.Idle);
        }
        #endregion

        private TItem _generate_item_(IByteDancePoolList<TItem> list, TKey key, EPoolListState state)
        {
            var item = _funcGenerate(key);
            if (item != null)
            {
                list.PushItem(item, state);
                _itemDic.Add(item, key);
            }

            return item;
        }
    }
}