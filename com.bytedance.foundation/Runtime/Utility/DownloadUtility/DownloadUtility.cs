using System;
using System.Collections;
using System.Collections.Generic;
using ByteDance.Foundation.Coroutine;
using UnityEngine;

namespace ByteDance.Foundation
{
    public static class DownloadUtility
    {
        private static MyLogger logger = new MyLogger("DownloadUtility");

        /// <summary>
        /// 下载结果美剧
        /// </summary>
        public enum EDownloadResult
        {
            Success = 0,
            Error = 1,
        }

        /// <summary>
        /// Downloads from WWW.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="E_OnCallback">The e on callback.</param>
        /// <param name="E_OnTimeout">The e on timeout.</param>
        /// <param name="E_OnProgress">The e on progress.</param>
        public static void DownloadFromWWW(string url, System.Action<EDownloadResult, WWW> E_OnCallback, System.Action<int> E_OnTimeout,System.Action<float> E_OnProgress)
        {
            Defer.RunCoroutine(DownloadFromWWWAsync(1, url, E_OnCallback, E_OnTimeout,E_OnProgress));
        }

        private static IEnumerator<float> DownloadFromWWWAsync(int tryCount, string url, System.Action<EDownloadResult, WWW> E_OnCallback, System.Action<int> E_OnTimeout, System.Action<float> E_OnProgress)
        {
            if (tryCount <= FoundationConst.MaxTryCountDownload)
            {
                UnityEngine.Debug.LogError("DownloadUtility::DownloadFromWWWAsync Start download url = " + url + " , TryCount = " + tryCount);
                //logger.LogInfo("DownloadUtility::DownloadFromWWWAsync Start download url = " + url + " , TryCount = " + tryCount);
                WWW www = new WWW(url);
                bool bTimeout = false;
                float waitingTime = 0.0f;
                float progress = 0.0f;

                while ((!www.isDone) && www.error == null)
                {
                    if (www.progress == progress)
                        waitingTime += Time.unscaledDeltaTime;
                    else
                    {
                        waitingTime = 0.0f;
                        progress = www.progress;
                        E_OnProgress.InvokeSafely(www.progress);
                    }

                    if (waitingTime >= FoundationConst.DownloadTimeout)
                    {
                        bTimeout = true;
                        E_OnTimeout.InvokeSafely(tryCount);
                        break;
                    }

                    yield return CoroutineManager.WaitForOneFrame;
                }

                if (www.error != null || bTimeout)
                {
                    Defer.RunCoroutine(DownloadFromWWWAsync(tryCount + 1, url, E_OnCallback, E_OnTimeout, E_OnProgress));
                }
                else
                {
                    E_OnCallback.InvokeSafely(EDownloadResult.Success, www);
                    www.Dispose();
                    www = null;
                }
            }
            else
            {
                logger.LogError("DownloadManager::DownloadFromWWWAsync download failed. url = " + url);
                E_OnCallback.InvokeSafely(EDownloadResult.Error, null);
            }
        }

        /// <summary>
        /// Post data from WWW
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pPostData"></param>
        /// <param name="E_OnCallback"></param>
        public static void BasePostFromWWW(string url, string pPostData, System.Action<WWW> E_OnCallback)
        {
            Defer.RunCoroutine(BasePostFromWWW(1, url, pPostData, E_OnCallback));
        }

        private static IEnumerator<float> BasePostFromWWW(int tryCount, string pUrl, string pPostData, System.Action<WWW> E_OnCallback)
        {
            logger.LogInfo("BasePostFromWWW Try Count = {0}, PostData = {1}, Url = {2}".F(tryCount, pPostData, pUrl));
            if (tryCount < FoundationConst.MaxTryCountPostData)
            {
                //将文本转为byte数组  
                byte[] bs = System.Text.UTF8Encoding.UTF8.GetBytes(pPostData);
                //向HTTP服务器提交Post数据  
                WWW getData = new WWW(pUrl, bs);
                bool bTimeout = false;
                float waitingTime = 0.0f;
                float progress = 0.0f;

                //yield return getData;
                while ((!getData.isDone) && getData.error == null && !bTimeout)
                {
                    if (getData.progress == progress)
                        waitingTime += Time.deltaTime;
                    else
                    {
                        progress = getData.progress;
                        waitingTime = 0.0f;
                    }

                    if (waitingTime >= FoundationConst.DownloadTimeout)
                    {
                        bTimeout = true;
                        break;
                    }
                    yield return CoroutineManager.WaitForOneFrame;
                }

                if (getData.error != null || bTimeout)
                {
                    logger.LogError(getData.error);
                    Defer.RunCoroutine(BasePostFromWWW(tryCount + 1, pUrl, pPostData, E_OnCallback));
                }
                else
                {
                    E_OnCallback.InvokeSafely(getData);
                    getData.Dispose();
                }
            }
            else
            {
                logger.LogError("DownloadManager::DownloadFromWWWAsync download failed.");
                E_OnCallback(null);
            }
        }
    }
}