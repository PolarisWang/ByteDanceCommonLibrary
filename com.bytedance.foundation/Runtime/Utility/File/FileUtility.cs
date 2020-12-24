using JetBrains.Annotations;

namespace ByteDance.Foundation
{
    /// <summary>
    /// 文件管理类
    /// </summary>
    public static class FileUtility
    {
        private static FileUtilityBase _util;

        public static FileUtilityBase Util
        {
            get
            {
                if (_util == null)
                {
#if UNITY_EDITOR
                    _util = new FileUtilityStandalone();
#else
#if UNITY_IOS
                    _util = new FileUtilityIOS();
#elif UNITY_ANDROID
                    _util = new FileUtilityAndroid();
#else
                     _util = new FileUtilityStandalone();
#endif
#endif
                }
                return _util;
            }
        }

        //public static void SetMode(PlatformDefines platform, bool isEditorAndroid)
        //{
        //    if (platform == PlatformDefines.Editor)
        //        _util = new FileUtilityStandalone(isEditorAndroid);
        //    else  if (platform == PlatformDefines.Android)
        //        _util = new FileUtilityAndroid();
        //    else if (platform == PlatformDefines.IOS)
        //        _util = new FileUtilityIOS();
        //}

        public static void SetFileUtility(FileUtilityBase pNewFileUtility)
        {
            if (pNewFileUtility == null)
                return;
            _util = pNewFileUtility;
        }

    }

    /// <summary>
    /// 文件工具基类
    /// </summary>
    public abstract class FileUtilityBase
    {
        protected string ResourceUrl;

        public void RegistResourceUrl(string resourceUrl)
        {
            this.ResourceUrl = resourceUrl;
        }

        /// <summary>
        /// 获取可持久化路径
        /// </summary>
        /// <returns></returns>
        public abstract string GetPersistencePath();

        /// <summary>
        /// 获取StreamingAsset文件路径
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public abstract byte[] LoadBytesAtStreamingAssetsFolder(string relatedPath);

        /// <summary>
        /// 读取Preload地址
        /// </summary>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        [CanBeNull]
        public abstract byte[] LoadBytesAtPreloadFolder(string relatedPath);

        /// <summary>
        /// 读取固定的资源下载地址
        /// </summary>
        /// <returns></returns>
        public abstract string GetFixedResDownloadRootUrl();

        /// <summary>
        /// 获取StreamingAsset文件内容
        /// </summary>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        [CanBeNull]
        public string LoadTextAtStreamingAssetsFolder(string relatedPath)
        {            
            byte[] bytes = LoadBytesAtStreamingAssetsFolder(relatedPath);
            if (bytes != null)
                return System.Text.Encoding.UTF8.GetString(bytes);
            else
                return null;
        }

        /// <summary>
        /// Gets the preload path by WWW path.
        /// </summary>
        /// <param name="relatedPath">The related path.</param>
        /// <returns></returns>
        public abstract string GetPreloadPathByWWWPath(string relatedPath);
    }
}