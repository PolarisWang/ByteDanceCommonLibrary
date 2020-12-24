/**************************************************************
 * FileName:	ITimerClock.cs
 * Author:		王浩川
 * CreateTime:  2018-09-07 13:35
 * Copyright:   ByteDance
 * Description: 
 ***************************************************************/

namespace ByteDance.Foundation
{
    /// <summary>
    /// Defines the <see cref="ITimerClock" />.
    /// </summary>
    public interface ITimerClock : System.IDisposable
    {
        /// <summary>
        /// 设置时钟节点依附哪个TimerClock.
        /// </summary>
        /// <param name="parentTimerClock">.</param>
        void SetParentClock(ITimerClock parentTimerClock);

        /// <summary>
        /// 获取DeltaTime.
        /// </summary>
        /// <returns>.</returns>
        float GetDeltaTime();

        /// <summary>
        /// 获取当前生存时间.
        /// </summary>
        /// <returns>.</returns>
        float GetEngineTime();

        /// <summary>
        /// Gets the Speed
        /// 播放速度.
        /// </summary>
        float Speed { get; }

        /// <summary>
        /// Gets the ParentTimerClock
        /// 获取父节点TimerClock.
        /// </summary>
        ITimerClock ParentTimerClock { get; }

        /// <summary>
        /// 添加监听Speed改变时间.
        /// </summary>
        /// <param name="onChange">.</param>
        void AddListenerSpeedChangedBefore(System.Action<ITimerClock> onChange);

        /// <summary>
        /// 移除监听Speed改变时间.
        /// </summary>
        /// <param name="onChange">.</param>
        void RemoveListenerSpeedChangedBefore(System.Action<ITimerClock> onChange);

        /// <summary>
        /// 设置速度.
        /// </summary>
        /// <param name="speed">.</param>
        void SetSpeed(float speed);

        /// <summary>
        /// Gets or sets the E_OnChanged
        /// TimerClock 改变事件.
        /// </summary>
        System.Action<ITimerClock> E_OnChanged { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ITimerClockCtrl" />.
    /// </summary>
    public interface ITimerClockCtrl
    {
        /// <summary> Gets the timer clock. </summary>
        ITimerClock TimerClock { get; }

        /// <summary> 设置播放速度 </summary>
        /// <param name="speed">播放速度.</param>
        void SetPlaySpeed(float speed);

        /// <summary> 暂停TimerClock </summary>
        void PauseTimeClock();

        /// <summary> 恢复TimerClock </summary>
        void ResumeTimeClock();

        /// <summary> 设置TimerClock的父子结构 </summary>
        /// <param name="parent">The parent<see cref="ITimerClock"/>.</param>
        void SetParentTimerClock(ITimerClock parent);
    }
}
