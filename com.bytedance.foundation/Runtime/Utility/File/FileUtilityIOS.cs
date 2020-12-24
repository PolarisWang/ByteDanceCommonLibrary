using UnityEngine;

namespace ByteDance.Foundation
{
    public class FileUtilityIOS : FileUtilityBase
    {
        public override string GetPersistencePath()
        {
            string desPath = Application.persistentDataPath;
            desPath = desPath.Substring(0, desPath.Length - 10) + "/Library/Caches/";
            return desPath;
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
            var path = FileHelper.CombinePath(Application.dataPath, relatedPath);
            if (FileHelper.FileExist(path))
                return FileHelper.FileReadAllBytes(path);
            else
                return null;
        }

        public override string GetFixedResDownloadRootUrl()
        {
            Assert.AssertStringNotNull(ResourceUrl);
            return ResourceUrl;
        }

        public override string GetPreloadPathByWWWPath(string relatedPath)
        {
            return FileHelper.CombinePath(Application.dataPath, relatedPath).AddWWWFilePrefix();
        }
    }
}