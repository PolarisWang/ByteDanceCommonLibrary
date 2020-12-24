using System.Collections.Generic;

namespace ByteDance.Foundation
{
#pragma warning disable 649
    public class StopWatch
    {
        private bool _bStart = false;
        private string _name;
        private System.DateTime _startTime, _stopTime;
        private List<Tuple<System.DateTime, string>> _data;

        public StopWatch(string name, bool autoStart = false)
        {
            _name = name;
            if (autoStart)
                Start();
        }

        public void Start()
        {
            if (_bStart)
            {
                MyLogger.Log.LogError("Stop watch [{0}] already started.".F(_name));
                return;
            }
            _bStart = true;
            _startTime = System.DateTime.Now;
        }

        public void Record(string log)
        {
            _data.AddSafely(new Tuple<System.DateTime, string>(System.DateTime.Now, log));
        }

        public void Stop()
        {
            if (!_bStart)
            {
                MyLogger.Log.LogError("Stop watch [{0}] not start.".F(_name));
                return;
            }
            _bStart = false;
            _stopTime = System.DateTime.Now;
        }

        public override string ToString()
        {
            string result = "StopWath:[{0}] started\n".F(_name);
            if (_data != null)
                foreach (var content in _data)
                {
                    var ms = (content.Item1 - _startTime).TotalMilliseconds;
                    result += "{0} \tms: {1}".F(ms, content.Item2);
                }
            var ms2 = (_stopTime - _startTime).TotalMilliseconds;
            result += "{0} \tms: stopped ".F(ms2);
            return result;
        }
    }
}
