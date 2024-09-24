using System;
using UnityEngine;

namespace ToranomonSDK
{
    internal static class SDKInternalSettings
    {
        public static string PinnacleDeviceId
        {
            get
            {
                const string Key = "tora_sdk_device_Id";
                var deviceId = PlayerPrefs.GetString(Key);
                if (string.IsNullOrEmpty(deviceId))
                {
                    deviceId = Guid.NewGuid().ToString();
                    PlayerPrefs.SetString(Key, deviceId);
                    PlayerPrefs.Save();
                }
                return deviceId;
            }
        }

        public static string PinnacleAppId => "tokyonode_toranomon";
        public static string PinnacleAPIKey => "qC298s7Lsh0GKgyU1of389FLrBhCJn182EI5I5je";
    }
}
