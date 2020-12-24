using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ByteDance.Foundation
{
    public static class ByteDancePoolConst
    {
        public static MyLogger Logger = new MyLogger("Pool");
    }

    /// <summary> Pool 使用状态 </summary>
    public enum EPoolListState
    {
        Idle, Using
    }

    /// <summary>
    /// 对象池List, 包含使用中和可使用的所有PoolItem
    /// </summary>
    internal interface IByteDancePoolList<TItem> where TItem : ByteDancePoolItem
    {
        /// <summary>
        /// 尝试获取可用Item
        /// </summary>
        /// <returns>Pool Item</returns>
        [CanBeNull] TItem PeekToUse();
        
        /// <summary>
        /// 讲对象压进队列
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="state">The state.</param>
        void PushItem([NotNull] TItem item, EPoolListState state);
    }

    public interface IByteDancePoolListInterface
    {
        void Return(ByteDancePoolItem item);
    }

    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="TKey">Key类型</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface IByteDancePoolRoot<TKey, TItem> where TItem: ByteDancePoolItem
    {
        /// <summary>
        /// Initializes the specified function generate.
        /// </summary>
        /// <param name="funcGenerate">The function generate.</param>
        /// <param name="comparer">The comparer.</param>
        void Init(
            [NotNull] System.Func<TKey, TItem> funcGenerate,
            [CanBeNull] IEqualityComparer<TKey> comparer = null,
            [CanBeNull] System.Action<TItem> funcDispose = null);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Peeks to use.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [CanBeNull] TItem PeekToUse(TKey key);

        /// <summary>
        /// Caches the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="size">The size.</param>
        void CacheNew(TKey key, int size);
    }

    public interface IByteDancePoolManager
    {
        TItem PeekToUse<TItem>() where TItem : ByteDancePoolItem, new();
    }
}