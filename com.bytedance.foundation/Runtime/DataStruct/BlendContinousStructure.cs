using System.Collections.Generic;
using UnityEngine;

namespace ByteDance.Foundation
{
    /// <summary>
    /// Blend的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBlendingLineData<TV>
    {
        /// <summary> 当前记录的时长 </summary>
        float TotalTime { get; }
        /// <summary> 最长时长 </summary>
        float MaxTime { get; }
        /// <summary> 获取当前时间的值 </summary>
        /// <param name="time">当前时间</param>
        /// <returns>Value</returns>
        TV GetValueByTime(float time);
        /// <summary> 调试方法 </summary>
        void DebugInfo();
        /// <summary> 驱动 </summary>
        /// <param name="deltaTime">时间差</param>
        /// <param name="value">值</param>
        void Update(float deltaTime, TV value);
    }

    /// <summary>
    /// 连续Blend的结构
    /// 包含缓存结构，
    /// </summary>
    /// <typeparam name="T">操作模板</typeparam>
    /// <typeparam name="TV">值模板</typeparam>
    public class BlendContinousStructure<T,TV> where T: IBlendingLineData<TV>
    {
        public float PreTime { get; private set; }
        public float AfterTime { get; private set; }
        public float MaxTime { get; private set; }
        public AnimationCurve Curve { get; private set; }
        //public bool IsLastBlendLoopMode { get; set; } = true;
        public bool IsLastBlendLoopMode  {  get { return _isLastBlendLoopMode; }  set { _isLastBlendLoopMode = value; } }
        private bool _isLastBlendLoopMode = true;
        public float CurTime { get { return _curTime; }  }

        private LinkedList<InternalData<T>> _runningData = new LinkedList<InternalData<T>>();
        private float _curTime;

        public class InternalData<T> where T : IBlendingLineData<TV>
        {
            public T Data { get; set; }
            public float StartTime { get; set; }
            public float BlendTime { get; set; }
        }

        public struct ResultData<T> where T : IBlendingLineData<TV>
        {
            public InternalData<T> Data;
            public float Ratio;
        }

        public void ClearAndSetValue(float maxTime, float preTime, float afterTime, AnimationCurve curve)
        {
            Assert.AssertTrue(afterTime >= preTime); // Ratio must bigger than zero.
            PreTime = preTime;
            AfterTime = afterTime;
            Curve = curve;
            MaxTime = maxTime;
        }

        /// <summary> 驱动 </summary>
        /// <param name="deltaTime">时间差</param>
        /// <param name="value">值</param>
        public ResultData<T>[] Update(float deltaTime, TV value)
        {
            _curTime += deltaTime;

            var iter = _runningData.First;
            while (iter != null)
            {
                var internalData = iter.Value;
                var blend = internalData.Data as IBlendingLineData<TV>;

                // 修正Blend的StartTime
                if (iter.Next == null)
                    _CorrectBlendStartTime(internalData);

                blend.Update(deltaTime, value);
                blend.DebugInfo();
                var src_iter = iter;
                iter = iter.Next;

                // 移除超时的Blend线
                if (!IsValid(src_iter.Value))
                {
                    //Debug.LogWarning("Remove timeout line.");
                    _runningData.Remove(src_iter);
                }
            }

            return GetCurrentValue();
        }

        public void AllocateNewBlend(T data)
        {
            var last = _runningData.Last;
            if (last != null)
                last.Value.BlendTime = _curTime;

            InternalData<T> item = new InternalData<T>();
            item.StartTime = _curTime;
            item.Data = data ;
            _runningData.AddLast(item);
        }

        public void DebugInfo()
        {
            foreach (var item in _runningData)
                item.Data.DebugInfo();
        }

        public void Reset()
        {
            _runningData.Clear();
        }

        private bool IsValid(InternalData<T> internalData)
        {
            var passedTime = _curTime - internalData.StartTime;
            return passedTime < internalData.Data.MaxTime + AfterTime;
        }

        /// <summary> 修正Blend的StartTime </summary>
        private void _CorrectBlendStartTime(InternalData<T> internalData)
        {
            if (!IsLastBlendLoopMode)
                return;

            // 只有Loop模式才需要修正StartTime
            var blend = internalData.Data as IBlendingLineData<TV>;
            var passedTime = _curTime - internalData.StartTime;
            if (passedTime > blend.MaxTime)
                internalData.StartTime = _curTime - blend.MaxTime;
        }

        private ResultData<T>[] GetCurrentValue()
        {
            var cnt = _runningData.Count;
            ResultData<T>[] result = new ResultData<T>[cnt];
            int index = 0;

            var ratio_left = 1.0f; // 当前分赃的百分比
            var iter = _runningData.First;
            while (iter != null)
            {
                InternalData<T> internal_data = iter.Value;
                IBlendingLineData<TV> blend = internal_data.Data as IBlendingLineData<TV>;

                // 最后一根Blend线，则要保持时间上的循环
                if (iter.Next == null)
                {
                    result[index] = new ResultData<T>() { Data = internal_data, Ratio = ratio_left };
                }
                // 若非最后一根Blend线，则自由完成生命周期
                else
                {
                    var blendPassedTime = internal_data.BlendTime - internal_data.StartTime;
                    var totalTime = blend.MaxTime - PreTime + AfterTime;
                    var full = totalTime - blendPassedTime;
                    var passedTime = _curTime - internal_data.BlendTime;
                    var ratio = Mathf.Clamp01(passedTime / full);
                    var _fRatioReal = ratio_left * (1 - ratio);
                    ratio_left = ratio_left - _fRatioReal;
                    result[index] = new ResultData<T>() { Data = internal_data, Ratio = _fRatioReal };
                }

                index++;
                iter = iter.Next;
            }
            return result;
        }
    }
}