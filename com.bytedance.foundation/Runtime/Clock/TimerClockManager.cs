/**************************************************************
 * FileName:	NewTimerClockManager.cs
 * Author:		王浩川
 * CreateTime:  2018-09-07 14:11
 * Copyright:   ByteDance
 * Description: 
 ***************************************************************/

namespace ByteDance.Foundation
{
    public class TimerClockManager : Singleton<TimerClockManager>
    {
        private ITimerClock _timerClockRoot;

        public ITimerClock TimerClockRoot
        {
            get
            {
                if (_timerClockRoot == null)
                    _timerClockRoot = new TimerClockRoot();
                return _timerClockRoot;
            }
        }
    }
}