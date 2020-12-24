using System.Collections.Generic;
using System.Security.Cryptography;
using ByteDance.Foundation;

namespace ByteDance.ComLayer
{
    using TimerId = System.Int32;
    using TimeInSecond = System.Single;
    using Counter = System.Int32;

    /// <summary> interface of TimerManager </summary>
    public interface ITimerManager
    {
        /// <summary> Resets the timer. </summary>
        /// <param name="timerId">The timer identifier.</param>
        void ResetTimer(TimerId timerId);

        /// <summary> Kills the timer. </summary>
        /// <param name="timerId">The timer identifier.</param>
        void KillTimer(TimerId timerId);

        /// <summary> 注册计时器. </summary>
        /// <param name="delayTime">等待时间.</param>
        /// <param name="onFinish">结束回调事件.</param>
        /// <returns> The timer identifier. </returns>
        TimerId Register(TimeInSecond delayTime, System.Action onFinish);

        /// <summary> 注册循环调用计时器. </summary>
        /// <param name="delayTime">等待事件.</param>
        /// <param name="repeatCount">重复次数 （无限次数用 -1）.</param>
        /// <param name="repeatIntervalTime">重复期间的间隔事件.</param>
        /// <param name="onPulse">触发事件回调.</param>
        /// <param name="onFinish">结束事件回调.</param>
        /// <returns> The timer identifier. </returns>
        TimerId RegisterRepeat(TimeInSecond delayTime, int repeatCount, TimeInSecond repeatIntervalTime,
            System.Action<TimerId, Counter> onPulse, System.Action onFinish);
        /// <summary> 获取剩余时间 </summary>
        /// <param name="timerId">The timer identifier.</param>
        /// <returns>剩余时间（单位：秒）</returns>
        TimeInSecond GetTimeLeftByTimerId(TimerId timerId);
    }

    public sealed class TimerManager : ITimerManager, IByteDanceGlobal
    {
        /// <summary> The invalid timer identifier </summary>
        public const TimerId InvalidTimerId = -1;
        /// <summary> 无限时长 </summary>
        public const TimeInSecond TimeLengthUnlimited = TimeInSecond.MaxValue;
        /// <summary> 非法时长 </summary>
        public const TimeInSecond TimeLengthInvalid = -1;

        /// <summary> The timer sorted by timerId </summary>
        private readonly LinkedList<TimerStruct> _linkedList;
        /// <summary> The timer structs </summary>
        private readonly Dictionary<TimerId, TimerStruct> _timerStructs;
        /// <summary> To remove list </summary>
        private readonly List<TimerId> _toRemoveList;
        /// <summary> 缓存池 </summary>
        private readonly ByteDancePoolList<TimerStruct> _poolList;
        /// <summary> The logger </summary>
        private static readonly MyLogger _logger = new MyLogger("TimerManager");

        /// <summary> The current time </summary>
        private TimeInSecond _currentTime = 0;
        /// <summary> The counter </summary>
        private TimerId _counter = 0;

        public TimerManager()
        {
            _linkedList = new LinkedList<TimerStruct>();
            _timerStructs = new Dictionary<TimerId, TimerStruct>();
            _toRemoveList = new List<TimerId>();
            _poolList = new ByteDancePoolList<TimerStruct>();
        }

        public void Update(TimeInSecond deltaTime)
        {
            _tick_remove();
            _currentTime += deltaTime;
            while (true)
            {
                var iter = _linkedList.First;
                if (iter == null)
                    break;

                if (_currentTime >= iter.Value.NextInvokeTime)
                {
                    if (_invoke_timer_(iter.Value))
                        _toRemoveList.RobustAdd(iter.Value.Key);
                    iter = iter.Next;
                }
                else
                    break;

                _tick_remove();
            }
        }

        public void Update() { Update(UnityEngine.Time.deltaTime); }
        public void Awake() { }
        public void Dispose() { }

        /// <summary>
        /// Gets the time left by timer identifier.
        /// </summary>
        /// <param name="timerId">The timer identifier.</param>
        /// <returns></returns>
        public TimeInSecond GetTimeLeftByTimerId(TimerId timerId)
        {
            var dataStruct = _timerStructs.GetOrDefault(timerId);
            if (dataStruct == null)
                return TimeLengthInvalid;

            if (dataStruct.TotalInvokeCount < 0)
                return TimeLengthUnlimited;

            var time = dataStruct.NextInvokeTime - _currentTime;
            time += dataStruct.RepeatIntervalTime * (dataStruct.TotalInvokeCount - dataStruct.InvokedCount);
            return time;
        }

        /// <summary> Kills the timer. </summary>
        /// <param name="timerId">The timer identifier.</param>
        public void KillTimer(TimerId timerId)
        {
            if (!_timerStructs.ContainsKey(timerId))
            {
                if (timerId <=0)
                    _logger.LogError("Kill timer failed. TimerId {0} not exist".F( timerId));
                return;
            }
            _toRemoveList.RobustAdd(timerId);
        }

        /// <summary> Resets the timer. </summary>
        /// <param name="timerId">The timer identifier.</param>
        public void ResetTimer(TimerId timerId)
        {
            if (!_timerStructs.ContainsKey(timerId))
            {
                _logger.LogError("Kill timer failed. TimerId {0} not exist".F(timerId));
                return;
            }

            var timerStruct = _timerStructs[timerId];
            timerStruct.NextInvokeTime = _currentTime + timerStruct.DelayTime;
            timerStruct.InvokedCount = 0;
        }

        /// <summary> Registers the specified delay time. </summary>
        /// <param name="delayTime">The delay time.</param>
        /// <param name="onFinish">The on finish.</param>
        /// <returns>TimerId</returns>
        public TimerId Register(TimeInSecond delayTime, System.Action onFinish)
        {
            if (delayTime <= 0)
            {
                if (delayTime < 0)
                    _logger.LogError("Register delayTime must bigger than zero.");
                onFinish();
                return InvalidTimerId;
            }

            var invokeTime = _currentTime + delayTime;
            TimerStruct content = _poolList.PeekToUse();
            if (content == null)
            {
                content = new TimerStruct();
                _poolList.PushItem(content, EPoolListState.Using);
            }
            content.Key = InvalidTimerId;
            content.NextInvokeTime = invokeTime;
            content.DelayTime = delayTime;
            content.OnFinish = onFinish;
            content.InvokedCount = 0;
            content.TotalInvokeCount = 1;
            content.RepeatIntervalTime = 0;

            _push_timer_(delayTime, content);
            //_debug_info();
            return content.Key;
        }

        /// <summary>
        /// Registers the repeat.
        /// </summary>
        /// <param name="delayTime">The delay time.</param>
        /// <param name="repeatCount">The repeat count.</param>
        /// <param name="repeatIntervalTime">The repeat interval time.</param>
        /// <param name="onPulse">The on pulse.</param>
        /// <param name="onFinish">The on finish.</param>
        /// <returns></returns>
        public TimerId RegisterRepeat(TimeInSecond delayTime, int repeatCount, TimeInSecond repeatIntervalTime, System.Action<TimerId, Counter> onPulse, System.Action onFinish)
        {
            if (repeatIntervalTime <= 0)
            {
                _logger.LogError("Register repeat timer failed. repeat interval time is 0.");
                return InvalidTimerId;
            }


            if (repeatCount == 0)
            {
                _logger.LogError("Register repeat timer failed. repeat count is 0.");
                return InvalidTimerId;
            }

            var invokeTime = _currentTime + delayTime;
            TimerStruct content = _poolList.PeekToUse();
            if (content == null)
            {
                content = new TimerStruct();
                _poolList.PushItem(content, EPoolListState.Using);
            }

            content.Key = InvalidTimerId;
            content.NextInvokeTime = invokeTime;
            content.DelayTime = delayTime;
            content.OnPulse = onPulse;
            content.OnFinish = onFinish;
            content.InvokedCount = 0;
            content.TotalInvokeCount = repeatCount;
            content.RepeatIntervalTime = repeatIntervalTime;

            if (delayTime <= 0 && repeatCount == 1)
            {
                if (delayTime < 0)
                    _logger.LogError("RegisterRepeat delayTime must bigger than zero.");
                _invoke_timer_(content);
            }
            else
                _push_timer_(delayTime, content);
            return content.Key;
        }

        private void _tick_remove()
        {
            for (int index = 0; index < _toRemoveList.Count; index++)
            {
                var timerId = _toRemoveList[index];
                _remove_timerid(timerId);
            }
            _toRemoveList.Clear();
        }

        private void _remove_timerid(int timerId)
        {
            if (_timerStructs.ContainsKey(timerId))
            {
                var timerStruct = _timerStructs[timerId];
                _timerStructs.Remove(timerId);
                _linkedList.Remove(timerStruct.Node);
                timerStruct.Return();
            }
            else
                _logger.LogError("Remove timer {0} failed, not exist in TimeStruct List".F(timerId));
        }

        private bool _invoke_timer_(TimerStruct timer)
        {
            timer.InvokedCount++;
            if (timer.TotalInvokeCount < 0)
            {
                timer.NextInvokeTime = timer.NextInvokeTime + timer.RepeatIntervalTime;
                _push_timer_(timer.RepeatIntervalTime, timer, false);
                timer.OnPulse.InvokeSafely(timer.Key, timer.InvokedCount);
                return false;
            }
            else
            {
                if (timer.InvokedCount < timer.TotalInvokeCount)
                {
                    timer.NextInvokeTime = timer.NextInvokeTime + timer.RepeatIntervalTime;
                    _push_timer_(timer.RepeatIntervalTime, timer, false);
                }

                timer.OnPulse.InvokeSafely(timer.Key, timer.InvokedCount);
                if (timer.InvokedCount == timer.TotalInvokeCount)
                {
                    timer.OnFinish.InvokeSafely();
                    return true;
                }

                return false;
            }
        }

        private void _push_timer_(float delayTime, TimerStruct timer, bool resetKey = true)
        {
            var invokeTime = timer.NextInvokeTime;
            if (_timerStructs.ContainsKey(timer.Key))
                _remove_timerid(timer.Key);

            var iter = _linkedList.First;
            while (iter != null)
            {
                if (invokeTime < iter.Value.NextInvokeTime)
                {
                    if (resetKey)
                        timer.Key = ++_counter;
                    // First time to push timer
                    if (timer.Node == null)
                        timer.Node = _linkedList.AddBefore(iter, timer);
                    else
                        _linkedList.AddBefore(iter, timer.Node); 
                    _timerStructs.RobustSave(timer.Key, timer);
                    break;
                }
                iter = iter.Next;
            }

            if (iter == null)
            {
                if (resetKey)
                    timer.Key = ++_counter;
                if (timer.Node == null)
                    timer.Node = _linkedList.AddLast(timer);
                else
                {
                    var last = _linkedList.Last;
                    if (last != null)
                        _linkedList.AddAfter(last, timer.Node);
                    else
                        _linkedList.AddLast(timer.Node);
                }

                _timerStructs.RobustSave(timer.Key, timer);
            }
        }

        private void _debug_info()
        {
            string result = "";
            var iter = _linkedList.First;
            while (iter != null)
            {
                result += "[{0}, {1}]".F(iter.Value.Key, iter.Value.NextInvokeTime) + ",";
                iter = iter.Next;
            }
            result += "\n,RemoveList:";
            foreach (var item in _toRemoveList)
            {
                result += item + ",";
            }
            _logger.LogDebug(result);
        }

        /// <summary> TimerStruct </summary>
        private class TimerStruct : ByteDancePoolItem
        {
            /// <summary> The key </summary>
            public TimerId Key;
            /// <summary> The next invoke time </summary>
            public TimeInSecond NextInvokeTime;
            /// <summary> The delay time </summary>
            public TimeInSecond DelayTime;    // To implement Reset function.            
            /// <summary> The on pulse </summary>
            public System.Action<TimerId, Counter> OnPulse;
            /// <summary> The on finish </summary>
            public System.Action OnFinish;
            /// <summary> The invoked count </summary>
            public Counter InvokedCount;
            /// <summary> The total invoke count </summary>
            public Counter TotalInvokeCount;
            /// <summary> The repeat interval time </summary>
            public TimeInSecond RepeatIntervalTime;
            /// <summary> The node </summary>
            public LinkedListNode<TimerStruct> Node;

            public override void OnReturnToPool()
            {
            }
        }
    }
}