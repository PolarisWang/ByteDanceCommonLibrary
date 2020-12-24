/**************************************************************
 * FileName:	TimerClockRoot.cs
 * Author:		王浩川
 * CreateTime:  2018-09-07 14:14
 * Copyright:   ByteDance
 * Description: 
 ***************************************************************/

using System;

namespace ByteDance.Foundation
{
    public class TimerClockRoot : ITimerClock
    {
        public void SetParentClock(ITimerClock parentTimerClock)
        {
            Assert.Fail("This is root of timer clock. Cannot be set parent clock.");
        }

        public float GetDeltaTime()
        {
            return ((int)(UnityEngine.Time.deltaTime*1000))*0.001f;
        }

        public float GetEngineTime()
        {
            return ((int)(UnityEngine.Time.realtimeSinceStartup * 1000)) * 0.001f;
        }

        public float Speed
        {
            get { return 1; }
        }

        public ITimerClock ParentTimerClock
        {
            get { return null; }
        }

        public void AddListenerSpeedChangedBefore(Action<ITimerClock> onChange)
        {
        }

        public void RemoveListenerSpeedChangedBefore(Action<ITimerClock> onChange)
        {
        }

        public void SetSpeed(float speed)
        {
            Assert.Fail("This is root of timer clock. Cannot be set speed.");
        }

        public Action<ITimerClock> E_OnChanged { get; set; }

        public void Dispose() { }
    }
}