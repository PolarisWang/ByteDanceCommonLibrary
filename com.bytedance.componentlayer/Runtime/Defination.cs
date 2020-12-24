using System;
using ByteDance.Foundation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ByteDance.ComLayer
{
    /// <summary>
    /// 定义
    /// </summary>
    internal class Const
    {
        public static readonly MyLogger Log = new MyLogger("ByteDance.Settings",
            true, 
            Color.white, 
            Color.yellow,
            Color.yellow);

        #region Config

        public const String SETTINGS_FOLDER = "GameSettings";
        public const String SETTINGS_CONFIG_BUILDTARGET = "BuildTarget.json";
        public const String SETTINGS_CONFIG_GAMESETTINGS = "GameSettings.json";

        #endregion 

        #region Log Local Storage
        public const Int32  LOGLOCAL_FILE_LIMIT_AMOUNT = 10;              // 最大保存的日志数量
        public const String LOGLOCAL_LOG_FOLDER_NAME = "LOG";             // 日志文件夹名
        public const String LOGLOCAL_POSTFIX = ".log";                    // 日志文件后缀
        public const String LOGLOCAL_DATATIME_FORMAT = "yyyyMMdd_HHmmss"; // 文件名格式化
        public const Int32  LOGLOCAL_STORAGE_TIME_OUT = 100;              // 写入间隔 ms
        public const Int64  LOGLOCAL_MAX_FILE_SIZE = 15 * 1024 * 1024;    // 单文件大小上限15M，超过则拆分
        #endregion


    }

    /// <summary>
    /// 全局管理器接口
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IByteDanceGlobal : IDisposable
    {
        void Awake();
        void Update();
    }

    /// <summary>
    /// 管理器接口
    /// </summary>
    public interface IByteDanceComponents
    {
        T GetManager<T>() where T : IByteDanceGlobal;
        bool AddManager<T>(T globalComponent) where T : IByteDanceGlobal;
        bool RemoveManager(Type type);
    }

    /// <summary>
    /// Platform definition
    /// </summary>
    public enum PlatformDefines
    {
        None,
        Editor,
        IOS,
        Android
    }
}