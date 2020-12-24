namespace ByteDance.Foundation
{
  /// <summary>
  /// 常量
  /// </summary>
  public sealed class FoundationConst
  {
    public const int FileStreamMaxSize = 16192 * 1024; // Log文件大小

    public const float DownloadTimeout = 15.0f; // 下载超时时间
    public const int MaxTryCountDownload = 3;
    public const int MaxTryCountPostData = 3;

    // UpdateManager
    public const string EditorBuildFolder = "build";
    public const string EditorSrcResPathAndroid = "ByteDance_Android_Source";
    public const string EditorSrcResPathIOS = "ByteDance_IOS_Source";
    public const string EditorNetResPathAndroid = "ByteDance_Android_NetRes";
    public const string EditorNetResPathIOS = "ByteDance_IOS_NetRes";
    public const string EditorPreloadPathAndroid = "ByteDance_Android_Preload";

    public const string EditorPreloadPathIOS = "ByteDance_IOS_Preload";

    // Name
    public const string NameCoroutineManager = "__ByteDanceCoroutines__";

    public const string NameGlobals = "__ByteDanceGlobals__";
  }
}