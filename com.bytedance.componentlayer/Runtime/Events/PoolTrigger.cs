using System;
using System.Collections.Generic;
using ByteDance.Foundation;
using IDisposable = ByteDance.Foundation.Annotation.IDisposable;

namespace ByteDance.ComLayer
{
    /// <summary>
    /// Event Message 的基类
    /// 注意：不许在这里私自定义任何变量！
    /// </summary>
    /// <seealso cref="ByteDance.Foundation.ByteDancePoolItem" />
    public abstract class TriggerMessage : ByteDancePoolItem { }

    /// <summary>
    /// 事件分发器 （带缓存池功能）
    /// 事件注册方法：
    /// 1. 注册： AddTriggerEvent
    /// 2. 反注册：RemoveTriggerEvent
    /// 分发方式：
    /// 1. Allocate 消息后赋值
    /// 2. Invoke事件后会自动回收Message  ****注意：逻辑层使用完Message后必须释放指针！****
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <seealso cref="ByteDance.ComLayer.PoolTriggerBase{TKey, System.Action{T1}}" />
    public sealed class PoolTrigger<TKey, T1> : PoolTriggerBase<TKey, Action<T1>> where T1 : TriggerMessage
    {
        readonly ByteDancePoolDictionary<Type, T1> _poolDictionary = new ByteDancePoolDictionary<Type, T1>();

        public PoolTrigger() : base()
        {
            _poolDictionary.Init(key=>null, null, null);
        }

        public PoolTrigger(IEqualityComparer<TKey> comparer) : base(comparer)
        {
            _poolDictionary.Init(key => null, null, null);
        }

        protected override void Combine(Event e, Action<T1> trigger)
        {
            e.Trigger += trigger;
        }

        protected override void Remove(Event e, Action<T1> trigger)
        {
            e.Trigger -= trigger;
        }

        public T AllocateFromPool<T>() where T : T1, new()
        {
            var key = typeof(T);
            var result = _poolDictionary.PeekToUse(key) as T;
            if (result == null)
            {
                result = new T();
                _poolDictionary.PushItem(key, result, EPoolListState.Using);
            }

            return result;
        }

        public void InvokeTriggerEventAndReturn(TKey type, T1 t1 = default(T1)) 
        {
            using (BeginInvoke(type, out var t))
            {
                t?.Invoke(t1);
                t1.Return();
            }
        }
    }

    public abstract class PoolTriggerBase<Key, Action>
    {
        protected sealed class Event : ByteDancePoolItem, System.IDisposable
        {
            private const int kOnceMaxInvokeCount = 80;
            private int onceInvokeCount_;
            public Action Trigger;

            public void Dispose()
            {
                onceInvokeCount_ = 0;
            }

            public bool CheckInvokeCount()
            {
                if (onceInvokeCount_ > kOnceMaxInvokeCount)
                {
                    throw new ArgumentOutOfRangeException("Trigger invoke too many at once");
                }
                ++onceInvokeCount_;
                return onceInvokeCount_ == 1;
            }

            public override void OnReturnToPool()
            {
                onceInvokeCount_ = 0;
            }
        }
        private readonly PoolDictionary<Key, Event> triggers_;

        public PoolTriggerBase() { triggers_ = new PoolDictionary<Key, Event>(); }
        public PoolTriggerBase(IEqualityComparer<Key> comparer) { triggers_ = new PoolDictionary<Key, Event>(comparer); }

        public void AddTriggerEvent(Key type, Action trigger)
        {
            Event e;
            if (triggers_.TryGetValue(type, out e))
            {
                Combine(e, trigger);
            }
            else
            {
                var eve =  triggers_.Add(type);
                eve.Trigger = trigger;
            }
        }

        protected abstract void Combine(Event e, Action trigger);

        public void RemoveTriggerEvent(Key type, Action trigger)
        {
            Event e;
            if (triggers_.TryGetValue(type, out e))
            {
                Remove(e, trigger);
                if (e.Trigger == null)
                {
                    triggers_.Remove(type);
                }
            }
        }

        protected abstract void Remove(Event e, Action trigger);

        protected System.IDisposable BeginInvoke(Key type, out Action action)
        {
            Event e;
            if (triggers_.TryGetValue(type, out e))
            {
                bool isFirst = e.CheckInvokeCount();
                action = e.Trigger;
                return isFirst ? e : null;
            }
            else
            {
                action = default(Action);
                return null;
            }
        }

        public void Dispose()
        {
            triggers_.Clear();
        }
    }
}