#if UNITY_EDITOR
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;

namespace ToranomonSDK.Editor
{
    internal static class EditorWebRequestHelper
    {
        public static Task<byte[]> RequestBytes(this UnityWebRequest self, Action<float> onProgress = null)
        {
            var tcs = new TaskCompletionSource<byte[]>();
            SendWebRequest(self, onProgress,
                handler =>
                {
                    tcs.SetResult(handler.data);
                },
                error =>
                {
                    tcs.SetException(new Exception(error));
                });
            return tcs.Task;
        }

        private static void SendWebRequest(UnityWebRequest req, Action<float> onProgress, Action<DownloadHandler> onSuccess, Action<string> onError)
        {
            req.SendWebRequest();
            EditorApplication.CallbackFunction updateFunc = null;
            updateFunc = () =>
            {
                if (req.isDone)
                {
                    var result = req.result;
                    if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError || !string.IsNullOrEmpty(req.error))
                    {
                        onError?.Invoke(req.error);
                    }
                    else
                    {
                        onSuccess?.Invoke(req.downloadHandler);
                    }
                    EditorApplication.update -= updateFunc;
                }
                else
                {
                    onProgress?.Invoke(req.downloadProgress);
                }
            };
            EditorApplication.update += updateFunc;
        }
    }
}
#endif
