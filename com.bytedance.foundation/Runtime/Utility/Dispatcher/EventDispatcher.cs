// 打开这里记录异步消息 Invoke 堆栈
//#define STACKTRACE_LOG  

using System;
using System.Collections.Generic;
#if STACKTRACE_LOG
using System.Diagnostics;
#endif

namespace ByteDance.Foundation
{
    public interface IEventDispatcherBase<TK, TV1, TV2>
    {
        void AddTriggerEvent(TK type, System.Action<TV1, TV2> action);
        void RemoveTriggerEvent(TK type, System.Action<TV1, TV2> action);
        void InvokeTriggerEvent(TK type, TV1 arg1 = default(TV1), TV2 arg2 = default(TV2), bool isAsync = true);
    }

    public sealed class EventDispatcher<TKey, TValue1, TValue2>
        : EventDispatcherBase<TKey, Action<TValue1, TValue2>>
        , IEventDispatcherBase<TKey, TValue1, TValue2>
    {
        private readonly ExposedList<TriggerData> toInvoke = new ExposedList<TriggerData>(10);
        private readonly ExposedList<TriggerData> invoking = new ExposedList<TriggerData>(10);
        private readonly MyLogger _logger = new MyLogger("EventDispatcher");

        protected struct TriggerData
        {
            public TKey Key;
            public TValue1 T1;
            public TValue2 T2;
#if STACKTRACE_LOG
            public StackTrace StackTrace;
#endif
        }

        public EventDispatcher(IEqualityComparer<TKey> comparer):base(comparer) { }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>True: No updates.  False: have updates. </returns>
        public bool Update()
        {
            if (toInvoke.Count == 0)
                return true;

            int counter = 0;
            // Internal looper safe protect.
            while (++counter <= 100)
            {
                UpdateInternal();
                if (toInvoke.Count == 0)
                    break;
            }
            return false;
        }

        public void InvokeTriggerEvent(TKey key, TValue1 t1 = default(TValue1), TValue2 t2 = default(TValue2), bool isAsync = true)
        {
            if (isAsync)
            {
                TriggerData data = new TriggerData() {Key = key, T1 = t1, T2 = t2};
#if STACKTRACE_LOG
                data.StackTrace = new StackTrace(2, true);
#endif
                toInvoke.Add(data);
            }
            else
                InvokeTriggerEventInternal(key, t1, t2);
        }

        protected override void Combine(Event e, Action<TValue1, TValue2> trigger)
        {
            e.Trigger += trigger;
        }

        protected override void Remove(Event e, Action<TValue1, TValue2> trigger)
        {
            e.Trigger -= trigger;
        }

        private void UpdateInternal()
        {
            // Copy to process list
            for (int index = 0; index < toInvoke.Count; index++)
                invoking.Add(toInvoke[index]);

            toInvoke.Clear(false);
            for (int index = 0; index < invoking.Count; index++)
            {
                var data = invoking[index];
#if STACKTRACE_LOG
                _logger.LogInfo($"Invoke <color=yellow>{data.Key.ToString()}</color>, call stack is below.\n {data.StackTrace.ToString()}"); 
#endif
                InvokeTriggerEventInternal(data.Key, data.T1, data.T2);
            }
            invoking.Clear(false);
        }

        private void InvokeTriggerEventInternal(TKey type, TValue1 t1 = default(TValue1), TValue2 t2 = default(TValue2))
        {
            Action<TValue1, TValue2> t = null;
            using (BeginInvoke(type, out t))
            {
                t?.Invoke(t1, t2);
            }
        }
    }

    public abstract class EventDispatcherBase<Key, Action>
    {
        protected class Event : IDisposable
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
        }

        private Dictionary<Key, Event> triggers_ = new Dictionary<Key, Event>();

        public EventDispatcherBase(IEqualityComparer<Key> comparer)
        {
            triggers_ = new Dictionary<Key, Event>(comparer);
        }

        public void AddTriggerEvent(Key type, Action trigger)
        {
            if (triggers_.ContainsKey(type))
            {
                var e = triggers_[type];
                Combine(e, trigger);
            }
            else
                triggers_.Add(type, new Event() { Trigger = trigger });
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

        protected IDisposable BeginInvoke(Key type, out Action action)
        {
            Event e;
            if (triggers_.TryGetValue(type, out e))
            {
                bool isFirst = e.CheckInvokeCount();
                action = e.Trigger;
                if (isFirst)
                    return e;
                else
                    return null;
            }
            else
            {
                action = default(Action);
                return null;
            }
        }

        public virtual void Dispose()
        {
            triggers_.Clear();
        }
    }
}