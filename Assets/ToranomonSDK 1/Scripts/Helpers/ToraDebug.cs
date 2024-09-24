using System;
using UnityEngine;

namespace ToranomonSDK.Helpers
{
    internal static class ToraDebug
    {
        private const string SYMBOL = "TORANOMON_SDK_DEBUG";

        [System.Diagnostics.Conditional(SYMBOL)]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [System.Diagnostics.Conditional(SYMBOL)]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional(SYMBOL)]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        [System.Diagnostics.Conditional(SYMBOL)]
        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
