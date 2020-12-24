/**************************************************************
 * FileName:	NewTimerClock.cs
 * Author:		王浩川
 * CreateTime:  2018-09-07 14:03
 * Copyright:   ByteDance
 * Description: 
 ***************************************************************/

using System;

namespace ByteDance.Foundation
{
    public class TimerClock : ITimerClock
    {

        public enum EStateTag
        {
           New,
           DisPospose,
        }

        private EStateTag ClockTag = EStateTag.New;
        private ITimerClock _parentTimerClock;    // 父TimerClock
        private int _lastRefreshFrameCount = -1;    // 上次刷新帧数
        private float _lastRefreshParentEngineTime = -1;    // 上次刷新父TimerClock引擎时间 
        private float _myEngineTime = 0.00001f;    // 当前引擎的时间
        private bool isDisposed = false;
        private float _speed = 1.0f;

        private int _dependenceCount = 0;   //從屬
        public int DependenceCount => _dependenceCount;

        /// <inheritdoc />
        public Action<ITimerClock> E_OnChanged { get; set; }

        public TimerClock()
        {
            _parentTimerClock = TimerClockManager.Ins.TimerClockRoot;    // Root 不会改变速度，无需监听改变
            _dependenceCount = 0;
            ClockTag = EStateTag.New;
        }
        
        public void SetParentClock(ITimerClock parentTimerClock)
        {
            Assert.AssertTrue(!isDisposed,"TimerClock is disposed");
            Assert.AssertTrue(parentTimerClock != null && parentTimerClock != this,
                "Cannot set a null parent timer clock");
            Assert.AssertTrue(_check_parent_valid_(parentTimerClock), "Set parent timer clock invalid.");

            if (_parentTimerClock != null)
            {
                _parentTimerClock.RemoveListenerSpeedChangedBefore(OnSpeedChangedBefore);
            }
            _refresh_current_();
            
            _parentTimerClock = parentTimerClock;

            TimerClock t = _parentTimerClock as TimerClock;
            if (t!=null)
            {
                t._dependenceCount++;
            }
            _parentTimerClock.AddListenerSpeedChangedBefore(OnSpeedChangedBefore);
            _lastRefreshParentEngineTime = _parentTimerClock.GetEngineTime();    // 刷新父Timer的当前时间
        }

        private void _refresh_current_()
        {
            if (_lastRefreshFrameCount == UnityEngine.Time.frameCount)
                return;

            _lastRefreshFrameCount = UnityEngine.Time.frameCount;
            var curParentEngineTime = _parentTimerClock.GetEngineTime();

            _myEngineTime += (curParentEngineTime - _lastRefreshParentEngineTime) * Speed;
            _lastRefreshParentEngineTime = curParentEngineTime;
        }

        void OnSpeedChangedBefore(ITimerClock timerClock)
        {
            _refresh_current_();
            if (E_OnChanged != null)
                E_OnChanged.Invoke(this);
        }

        public float GetDeltaTime()
        {
            Assert.AssertTrue(!isDisposed, "TimerClock is disposed");
            return _parentTimerClock.GetDeltaTime() * Speed;
        }

        public float GetEngineTime()
        {
            Assert.AssertTrue(!isDisposed, "TimerClock is disposed");
            _refresh_current_();
            return _myEngineTime;
        }

        public ITimerClock ParentTimerClock
        {
            get { return _parentTimerClock; }
        }

        public void AddListenerSpeedChangedBefore(Action<ITimerClock> onChange)
        {
            Assert.AssertTrue(!isDisposed, "TimerClock is disposed");
            E_OnChanged += onChange;
        }

        public void RemoveListenerSpeedChangedBefore(Action<ITimerClock> onChange)
        {
            Assert.AssertTrue(!isDisposed, "TimerClock is disposed");
            E_OnChanged -= onChange;
        }

        public void SetSpeed(float speed)
        {
            Assert.AssertTrue(!isDisposed, "TimerClock is disposed");
            _refresh_current_();
            _speed = speed;
            if (E_OnChanged != null)
                E_OnChanged.Invoke(this);
        }


        public float Speed
        {
            get { return ParentTimerClock.Speed * _speed; }
        }

        private bool _check_parent_valid_(ITimerClock parent)
        {
            int safeLoopTime = 0;
            ITimerClock current = parent;
            while (++safeLoopTime <= 100)
            {
                if (current == null)
                    return true;
                current = current.ParentTimerClock;
            }
            return false;
        }

        public void Dispose()
        {
            if (DependenceCount==0 || ClockTag==EStateTag.DisPospose) 
            {
                _speed = 1.0f;
                _lastRefreshFrameCount = -1;
                _lastRefreshParentEngineTime = -1;
                _myEngineTime = 0.00001f;
                isDisposed = true;
                E_OnChanged = null; 
            }
            else
            {
                ClockTag = EStateTag.DisPospose;
                TimerClock pc = _parentTimerClock as TimerClock;
                if (pc!=null)
                {
                    pc._dependenceCount--;
                    if (pc.ClockTag == EStateTag.DisPospose && pc._dependenceCount <= 0)
                    {
                        if (_parentTimerClock != null)
                            _parentTimerClock.RemoveListenerSpeedChangedBefore(OnSpeedChangedBefore);
                        _parentTimerClock.Dispose();
                        _parentTimerClock = null;
                    }
                }
            }
        }
    }
}