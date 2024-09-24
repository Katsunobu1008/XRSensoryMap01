using System;
using System.Collections;
using UnityEngine;

namespace ToranomonSDK.Helpers
{
    internal sealed class CoroutineDispatcher : MonoBehaviour
    {
        private static CoroutineDispatcher _instance;

        public static void Dispatch(IEnumerator coroutine, Action onEnd)
        {
            if (Application.isPlaying == false)
            {
                return;
            }
            var instance = GetInstance();
            instance.StartCoroutine(Merge(coroutine, onEnd));

            static IEnumerator Merge(IEnumerator target, Action onEnd)
            {
                yield return target;
                onEnd();
            }
        }

        private static CoroutineDispatcher GetInstance()
        {
            if (Application.isPlaying == false)
            {
                Debug.LogError("invalid operation");
                return null;
            }
            if (_instance == null)
            {
                var instance = FindFirstObjectByType<CoroutineDispatcher>();
                if (instance == null)
                {
                    instance = new GameObject(nameof(CoroutineDispatcher)).AddComponent<CoroutineDispatcher>();
                }
                _instance = instance;
            }
            return _instance;
        }
    }
}
