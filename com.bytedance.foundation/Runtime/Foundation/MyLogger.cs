using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace ByteDance.Foundation
{
    /// <summary> Logger类 </summary>
    public partial class MyLogger : ILoggable
    {
        #region field capsule        
        /// <summary> Logger Key String </summary>
        private readonly string _loggableString;
        /// <summary> Is Enable </summary>
        private bool _isEnable = true;

#if LOG_NORMAL_ENABLE || UNITY_EDITOR        
        /// <summary> Title Color </summary>
        private Color? _titleColor;
        /// <summary> The Info log color </summary>
        private Color? _infoColor;
        /// <summary> The Debug log color </summary>
        private Color? _debugColor;
        /// <summary> The Warning log color </summary>
        private Color? _warningColor;
        /// <summary> The StringBuilder to combine String </summary>
        private static StringBuilder _sb = new StringBuilder();
#endif        
        /// <summary> The logger instance </summary>
        public static readonly MyLogger Log = new MyLogger("Logger");

        public static bool IsDebugEnable = true;
        public static bool IsInfoEnable = true;
        public static bool IsWarningEnable = true;
        public static bool IsErrorEnable = true;
        public static bool IsFatalEnable = true;

        //public bool IsEnable { get; set; } = true;
        public static System.Func<String, String> ExtraActionProcDebug,
            ExtraActionProcInfo,
            ExtraActionProcWarning,
            ExtraActionProcError,
            ExtraActionProcFatal;

        public bool IsEnable { get { return _isEnable; } set { _isEnable = value; } }
        public string ToLoggableString() { return _loggableString; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MyLogger"/> class.
        /// </summary>
        /// <param name="loggableString">The loggable string.</param>
        public MyLogger(string loggableString)
        {
            _loggableString = loggableString;
        }

        /// <summary>
        /// MyLogger Constructor
        /// </summary>
        /// <param name="loggableString">Logger 分类</param>
        /// <param name="defEnabled">默认的激活标记位</param>
        public MyLogger(string loggableString, bool defEnabled)
        {
            _loggableString = loggableString;
            _isEnable = defEnabled;
        }

        public MyLogger(string loggableString, bool defEnabled,
            Color? titleColor = null,
            Color? debugColor = null,
            Color? infoColor = null,
            Color? warningColor = null)
        {
            _loggableString = loggableString;
            _isEnable = defEnabled;
#if LOG_NORMAL_ENABLE || UNITY_EDITOR
            _infoColor = infoColor;
            _titleColor = titleColor;
            _debugColor = debugColor;
            _warningColor = warningColor;
#endif
        }

#region Public Field Log Info
        [Conditional("LOG_NORMAL_ENABLE"), Conditional("UNITY_EDITOR")]
        public void LogInfo(string message)
        {
            if (IsInfoEnable && IsEnable)
            {
                if (ExtraActionProcInfo != null) 
                    message = ExtraActionProcInfo(message);
                string final = _concat_(_loggableString, message,
#if LOG_NORMAL_ENABLE || UNITY_EDITOR
                    _titleColor, _infoColor
#else
                    null,null
#endif
                    );
                //UnityEngine.Debug.Log(final);
                _output_string_(UnityEngine.Debug.Log, final);
            }
        }
            
        [Conditional("LOG_NORMAL_ENABLE"), Conditional("UNITY_EDITOR")]
        public void LogDebug(string message)
        {
            if (IsDebugEnable && IsEnable)
            {
                if (ExtraActionProcDebug != null)
                    message = ExtraActionProcDebug(message);
                string final = _concat_(_loggableString, message,
#if LOG_NORMAL_ENABLE || UNITY_EDITOR
                    _titleColor, _infoColor
#else
                    null, null
#endif
                    );
                //UnityEngine.Debug.Log(final);
                _output_string_(UnityEngine.Debug.Log, final);
            }
        }
            
        [Conditional("LOG_NORMAL_ENABLE"), Conditional("UNITY_EDITOR")]
        public void LogWarning(string message)
        {
            if (IsWarningEnable && IsEnable)
            {
                if (ExtraActionProcWarning != null)
                    message = ExtraActionProcWarning(message);
                string final = _concat_(_loggableString, message,
#if LOG_NORMAL_ENABLE || UNITY_EDITOR
                    _titleColor, _infoColor
#else
                    null, null
#endif
                    );
                //UnityEngine.Debug.LogWarning(final);
                _output_string_(UnityEngine.Debug.LogWarning, final);
            }
        }
            
        [Conditional("LOG_ERROR_ENABLE"), Conditional("UNITY_EDITOR")]
        public void LogError(string message)
        {
            if (IsErrorEnable && IsEnable)
            {
                if (ExtraActionProcError != null)
                    message = ExtraActionProcError(message);
                string final = _concat_(_loggableString, message,
#if LOG_NORMAL_ENABLE || UNITY_EDITOR
                    _titleColor
#else
                    null
#endif
                    );
                //UnityEngine.Debug.LogError(final);
                _output_string_(UnityEngine.Debug.LogError, final);
            }
        }
            
        [Conditional("LOG_ERROR_ENABLE"), Conditional("UNITY_EDITOR")]
        public void LogFatal(string message)
        {
            if (IsFatalEnable&& IsEnable)
            {
                if (ExtraActionProcFatal != null)
                    message = ExtraActionProcFatal(message);
                string final = _concat_(_loggableString, message,
#if LOG_NORMAL_ENABLE || UNITY_EDITOR
                    _titleColor
#else
                    null
#endif
                    );
                //UnityEngine.Debug.LogError(final);
                _output_string_(UnityEngine.Debug.LogError, final);
            }
        }
        
        [Conditional("LOG_ERROR_ENABLE"), Conditional("UNITY_EDITOR")]
        public void LogException(Exception e, string title = null)
        {
            if (IsFatalEnable&& IsEnable)
            {
                var message = string.IsNullOrEmpty(title)
                    ? $"{e}\n=========END======="
                    : $"{title}\n{e}\n=========END=======";
                if (ExtraActionProcError != null)
                    message = ExtraActionProcError(message);
                string final = _concat_(_loggableString, message,
#if LOG_NORMAL_ENABLE || UNITY_EDITOR
                    _titleColor
#else
                    null
#endif
                    );
                _output_string_(UnityEngine.Debug.LogError, final);
            }
        }
#endregion

#region Public static log
        [Conditional("LOG_NORMAL_ENABLE"), Conditional("UNITY_EDITOR")]
        public static void LogInfo_(string message)
        {
            Log.LogInfo(message);
        }

        [Conditional("LOG_NORMAL_ENABLE"), Conditional("UNITY_EDITOR")]
        public static void LogDebug_(string message)
        {
            Log.LogDebug(message);
        }

        [Conditional("LOG_NORMAL_ENABLE"), Conditional("UNITY_EDITOR")]
        public static void LogWarning_(string message)
        {
            Log.LogWarning(message);
        }

        [Conditional("LOG_ERROR_ENABLE"), Conditional("UNITY_EDITOR")]
        public static void LogError_(string message)
        {
            Log.LogError(message);
        }

        [Conditional("LOG_ERROR_ENABLE"), Conditional("UNITY_EDITOR")]
        public static void LogFatal_(string message, params object[] param)
        {
            Log.LogFatal(message);
        }

        [Conditional("LOG_ERROR_ENABLE"), Conditional("UNITY_EDITOR")]
        public void LogException_(Exception e)
        {
            Log.LogException(e);
        }
#endregion
        
#region internal methods

        private String _concat_(String loggableString, String message, Color? loggableColor = null,  Color? consoleColor = null)
        {
#if LOG_NORMAL_ENABLE || UNITY_EDITOR
            _sb.Clear();
            var titleColor = _titleColor ?? loggableColor;
            var titleString = titleColor == null ? $"[{loggableString}]" : $"<color=#{titleColor.Value.ColorToHex()}><b>[{loggableString}]</b></color> ";
            _sb.Append(titleString);
            _sb.Append($"-[{Time.frameCount}]-");

            //String result = null;
            if (consoleColor != null)
            {
                var index = message.IndexOf('\n');
                if (index < 0)
                    _sb.Append($"<color=#{consoleColor.Value.ColorToHex()}>{message}</color>");
                else
                {
                    var firstLine = message.Substring(0, index - 1);
                    var otherLine = message.Substring(index);
                    _sb.Append( $"<color=#{consoleColor.Value.ColorToHex()}>{firstLine}</color>{otherLine}");
                }
            }
            else
                _sb.Append(message);

            return _sb.ToString();
#else
            return $"[{loggableString}] {message}";
#endif
        }

        private static void _output_string_(System.Action<object> action, string logs)
        {
            action(logs);
        }
    }
#endregion

    /// <summary>
    /// Logger接口
    /// </summary>
    internal interface ILoggable
    {
        string ToLoggableString();
    }
}
