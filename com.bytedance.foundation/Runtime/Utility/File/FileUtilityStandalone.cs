using UnityEngine;

namespace ByteDance.Foundation
{
    public class FileUtilityStandalone : FileUtilityBase
    {
      private bool isEditorAndroid
      {
        get
        {
#if UNITY_ANDROID
        return true;
#else
          return false;
#endif
        }
      }
        public FileUtilityStandalone()
        {
        }

        public override string GetPersistencePath()
        {
            return FileHelper.CombinePath(FileHelper.EditorProjectDir, "Downloaded");
        }

        public override byte[] LoadBytesAtStreamingAssetsFolder(string relatedPath)
        {
            var path = FileHelper.CombinePath(Application.streamingAssetsPath, relatedPath);
            if (FileHelper.FileExist(path))
                return FileHelper.FileReadAllBytes(path);
            else
                return null;
        }

        public override byte[] LoadBytesAtPreloadFolder(string relatedPath)
        {
            //string path = FileHelper.CombinePath(
            //    Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/build/",
            //    isEditorAndroid ? FoundationConst.EditorPreloadPathAndroid : FoundationConst.EditorPreloadPathIOS);
            //var fileName = FileHelper.CombinePath(path, relatedPath);
            var fileName = GetPreloadPathByWWWPath(relatedPath);
            if (FileHelper.FileExist(fileName))
                return FileHelper.FileReadAllBytes(fileName);
            else
                return null;
        }

        public override string GetFixedResDownloadRootUrl()
        {
            var path = FileHelper.CombinePath(Application.dataPath.Substring(0, Application.dataPath.Length - 7),
                FoundationConst.EditorBuildFolder);
            return isEditorAndroid
                ? FileHelper.CombinePath(path, FoundationConst.EditorNetResPathAndroid)
                : FileHelper.CombinePath(path, FoundationConst.EditorNetResPathIOS);
        }

        public override string GetPreloadPathByWWWPath(string relatedPath)
        {
            string path = FileHelper.CombinePath(
                Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/build/",
                isEditorAndroid ? FoundationConst.EditorPreloadPathAndroid : FoundationConst.EditorPreloadPathIOS);
            return FileHelper.CombinePath(path, relatedPath);
        }
    }
}