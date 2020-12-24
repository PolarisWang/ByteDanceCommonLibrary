using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ByteDance.ComLayer
{
    public class LogLocalStorage : IByteDanceGlobal
    {
        private struct LogInfo
        {
            public string Condition;
            public string StackTrace;
            public LogType Type;
        }

#if LOG_STORAGE || UNITY_EDITOR
        public bool IS_STORAGE = true;
#else
        public bool IS_STORAGE = false;
#endif

        private Thread _thread;        
        private ConcurrentQueue<LogInfo> _logQueue = new ConcurrentQueue<LogInfo>();
        private bool _stop;                                         // 标记退出

        private StringBuilder _stringBuilder = new StringBuilder();
        private StreamWriter _streamWriter;
        private int _subVersion = 1;                                // 日志文件拆分序号
        private int _msgIndex = 0;                                  // 日志条目序号
        private string _logFilePrefix;                              // 日志文件名前缀（200610124035）
        private string _curLogFilePath;                             // 当前日志的路径
        private string _PersistentDataPath;

        public void Awake()
        {
            if (!IS_STORAGE)
                return;

            _logFilePrefix = DateTime.Now.ToString(Const.LOGLOCAL_DATATIME_FORMAT);

            _PersistentDataPath = Application.persistentDataPath;
            string logFolderPath = string.Format("{0}/{1}", _PersistentDataPath, Const.LOGLOCAL_LOG_FOLDER_NAME);
            if (!Directory.Exists(logFolderPath))
                Directory.CreateDirectory(logFolderPath);

            TryCleanLogFiles(logFolderPath);
            CreateNewLogFile();

            Application.logMessageReceivedThreaded += OnLogCallBack;
            _thread = new Thread(() => StorageLogs());
            _thread.Start();
        }

        public void Update()
        {
        }

        public void Dispose()
        {
            if (!IS_STORAGE)
                return;

            Application.logMessageReceivedThreaded -= OnLogCallBack;
            _stop = true;
        }

        private void OnLogCallBack(string condition, string stackTrace, LogType type)
        {
            LogInfo logInfo = new LogInfo()
            {
                Condition = condition,
                StackTrace = stackTrace,
                Type = type,
            };

            _logQueue.Enqueue(logInfo);
        }

        private void StorageLogs()
        {
            while (true)
            {
                LogInfo logInfo;
                bool isWrite = false;
                while (!_logQueue.IsEmpty)
                {
                    if (_logQueue.TryDequeue(out logInfo))
                    {
                        _streamWriter.WriteLine(FormatLog(logInfo));
                        isWrite = true;
                    }
                    else
                    {
                        break;
                    }    
                }

                if (isWrite)
                {
                    _streamWriter.Flush();
                    if(!_stop)
                    {
                        FileInfo fileInfo = new FileInfo(_curLogFilePath);
                        if (fileInfo.Length >= Const.LOGLOCAL_MAX_FILE_SIZE)
                            CreateNewLogFile();
                    }
                }

                if (_stop && _logQueue.IsEmpty)
                {
                    _streamWriter.Close();
                    break;
                }

                Thread.Sleep(Const.LOGLOCAL_STORAGE_TIME_OUT);
            }
        }

        private void CreateNewLogFile()
        {
            string logFolderPath = string.Format("{0}/{1}", _PersistentDataPath, Const.LOGLOCAL_LOG_FOLDER_NAME);
            _curLogFilePath = string.Format("{0}/{1}_{2}{3}", logFolderPath, _logFilePrefix, _subVersion++, Const.LOGLOCAL_POSTFIX); //文件路径
            FileStream kFileStream = new FileStream(_curLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            _streamWriter = new StreamWriter(kFileStream);
        }

        private string FormatLog(LogInfo logInfo)
        {
            _stringBuilder.Clear();
            _stringBuilder.AppendFormat("[{0} {1}]", DateTime.Now.ToString(), logInfo.Type);
            _stringBuilder.AppendFormat("[{0:00000000}]{1}\n", _msgIndex++, logInfo.Condition);
            _stringBuilder.AppendLine(logInfo.StackTrace);
            return _stringBuilder.ToString();
        }

        private void TryCleanLogFiles(string logFolderPath)
        {
            List<string> allFiles = Directory.GetFiles(logFolderPath).ToList();
            if (allFiles.Count > Const.LOGLOCAL_FILE_LIMIT_AMOUNT)
            {
                allFiles.Sort((a, b) => String.Compare(Path.GetFileName(b), Path.GetFileName(a), StringComparison.Ordinal));

                for (int i = Const.LOGLOCAL_FILE_LIMIT_AMOUNT; i < allFiles.Count; i++)
                    File.Delete(allFiles[i]);
            }
        }
    }
}

