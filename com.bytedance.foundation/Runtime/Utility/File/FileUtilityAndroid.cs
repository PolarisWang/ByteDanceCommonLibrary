using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

namespace ByteDance.Foundation
{
    public class FileUtilityAndroid: FileUtilityBase
    {
        //[DllImport("dream")]
        //private static extern bool dream_isFileExistsC(string path);

        //[DllImport("dream")]
        //private static extern int dream_getStringC(string path, ref IntPtr data);

        public override string GetPersistencePath()
        {
            return Application.persistentDataPath;
        }

        public override byte[] LoadBytesAtStreamingAssetsFolder(string relatedPath) {
            //Debug.LogError("LoadBytesAtStreamingAssetsFolder relatedPath Is :" + relatedPath);
            //IntPtr unmanaged_ptr = IntPtr.Zero; //定义这个c#中用来接收c++返回数据的指针类型
            //int length = dream_getStringC(relatedPath, ref unmanaged_ptr);  //调用c++的函数，使unmanaged_ptr指向c++里分配的内存，注意这里用out ，才能与c++里面的**匹配。

            //byte[] managed_data = new byte[length];
            //Marshal.Copy(unmanaged_ptr, managed_data, 0, length);//将非托管内存拷贝成托管内存，才能在c#里面使用
            //Marshal.FreeHGlobal(unmanaged_ptr); //释放非托管的内存
            //Debug.LogError("LoadBytesAtStreamingAssetsFolder length  Is :" + length);
            //return managed_data;

            var loadingRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, relatedPath));
            loadingRequest.SendWebRequest();
            while (!loadingRequest.isDone)
            {
                if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
                {
                    break;
                }
            }
            if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
            {
                Debug.LogError("LoadBytesAtStreamingAssetsFolder failed, relatedPath Is : " + relatedPath);
            }
            else
            {
                return loadingRequest.downloadHandler.data;
            }

            return null;
        }

        public override byte[] LoadBytesAtPreloadFolder(string relatedPath)
        {
            return LoadBytesAtStreamingAssetsFolder(relatedPath);
        }
        
        public override string GetFixedResDownloadRootUrl()
        {
            Assert.AssertStringNotNull(ResourceUrl);
            return ResourceUrl;
        }

        public override string GetPreloadPathByWWWPath(string relatedPath)
        {
            return FileHelper.CombineRelatePath("jar:file://" + Application.dataPath + "!/assets/", relatedPath);
        }
    }
}