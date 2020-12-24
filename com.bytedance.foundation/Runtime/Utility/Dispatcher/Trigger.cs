using System;
using System.Collections.Generic;

namespace ByteDance.Foundation
{
    public sealed class Trigger<Key, T1, T2> : TriggerBase<Key, Action<T1, T2>>
    {
        public Trigger(IEqualityComparer<Key> comparer = null) : base(comparer) { }
        protected override void Combine(Event e, Action<T1, T2> trigger)
        {
            e.Trigger += trigger;
        }

        protected override void Remove(Event e, Action<T1, T2> trigger)
        {
            e.Trigger -= trigger;
        }

        public void InvokeTriggerEvent(Key type, T1 t1 = default(T1), T2 t2 = default(T2))
        {
//            Helper.LogWarning("Trigger InvokeTriggerEvent: Key:{0}, T1:{1}, T2:{2}".F(type.ToString(),
//                t1 == null ? "null" : t1.ToString(), t2 == null ? "null" : t2.ToString()));
            Action<T1, T2> t = null;
            using (BeginInvoke(type, out t))
            {
                t?.Invoke(t1, t2);
            }
        }
    }

    public sealed class Trigger<Key, T1, T2, T3> : TriggerBase<Key, Action<T1, T2, T3>>
    {
        public Trigger(IEqualityComparer<Key> comparer = null) : base(comparer) { }
        protected override void Combine(Event e, Action<T1, T2, T3> trigger)
        {
            e.Trigger += trigger;
        }

        protected override void Remove(Event e, Action<T1, T2, T3> trigger)
        {
            e.Trigger -= trigger;
        }

        public void InvokeTriggerEvent(Key type, T1 t1 = default(T1), T2 t2 = default(T2), T3 t3 = default(T3))
        {
            Action<T1, T2, T3> t = null;
            using (BeginInvoke(type, out t))
            {
                t?.Invoke(t1, t2, t3);
            }
        }
    }

    public sealed class Trigger<Key, T1, T2, T3, T4> : TriggerBase<Key, Action<T1, T2, T3, T4>>
    {

        public Trigger(IEqualityComparer<Key> comparer = null):base(comparer) { }
        protected override void Combine(Event e, Action<T1, T2, T3, T4> trigger)
        {
            e.Trigger += trigger;
        }

        protected override void Remove(Event e, Action<T1, T2, T3, T4> trigger)
        {
            e.Trigger -= trigger;
        }

        public void InvokeTriggerEvent(Key type, T1 t1 = default(T1), T2 t2 = default(T2), T3 t3 = default(T3), T4 t4 = default(T4))
        {
            Action<T1, T2, T3, T4> t = null;
            using (BeginInvoke(type, out t))
            {
                t?.Invoke(t1, t2, t3, t4);
            }
        }
    }

    public abstract class TriggerBase<Key, Action>
    {
        protected sealed class Event : IDisposable
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


        public TriggerBase(IEqualityComparer<Key> comparer) { triggers_ = new Dictionary<Key, Event>(comparer); }
        public void AddTriggerEvent(Key type, Action trigger)
        {
            Event e;
            if (triggers_.TryGetValue(type, out e))
            {
                Combine(e, trigger);
            }
            else
            {
                triggers_.Add(type, new Event() { Trigger = trigger });
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

        protected IDisposable BeginInvoke(Key type, out Action action)
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
