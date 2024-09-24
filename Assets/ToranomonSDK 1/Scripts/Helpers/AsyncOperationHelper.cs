using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace ToranomonSDK.Helpers
{
    internal static class AsyncOperationHelper
    {
        public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation self)
        {
            return new AsyncOperationAwaiter(self);
        }

        public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation self)
        {
            return new UnityWebRequestAsyncOperationAwaiter(self);
        }
    }

    internal sealed class AsyncOperationAwaiter : INotifyCompletion
    {
        private readonly AsyncOperation _op;
        private Action _continuation;

        public AsyncOperationAwaiter(AsyncOperation op)
        {
            _op = op;
            op.completed += OnRequestCompleted;
        }

        public bool IsCompleted => _op.isDone;

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        private void OnRequestCompleted(AsyncOperation obj)
        {
            _continuation();
        }
    }

    internal sealed class UnityWebRequestAsyncOperationAwaiter : INotifyCompletion
    {
        private readonly UnityWebRequestAsyncOperation _op;
        private Action _continuation;

        public UnityWebRequestAsyncOperationAwaiter(UnityWebRequestAsyncOperation op)
        {
            _op = op;
            op.completed += OnRequestCompleted;
        }

        public bool IsCompleted => _op.isDone;

        public void GetResult()
        {
            var result = _op.webRequest.result;
            if (result == UnityWebRequest.Result.ConnectionError || result == UnityWebRequest.Result.ProtocolError || result == UnityWebRequest.Result.DataProcessingError)
            {
                throw new Exception(_op.webRequest.error);
            }
        }

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        private void OnRequestCompleted(AsyncOperation obj)
        {
            _continuation();
        }
    }
}
