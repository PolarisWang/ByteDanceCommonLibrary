using System;

namespace ByteDance.Foundation
{
    public abstract class ByteDancePoolItem
    {
        [NonSerialized]
        internal IByteDancePoolListInterface _poolList;
        public abstract void OnReturnToPool();
        public virtual void Return() {
            _poolList.Return(this);
            _poolList = null;
        }
    }
}