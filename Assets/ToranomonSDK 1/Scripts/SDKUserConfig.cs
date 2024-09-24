using UnityEngine;

namespace ToranomonSDK
{
    [CreateAssetMenu(fileName = nameof(SDKUserConfig), menuName = "ToranomonSDK/SDKUserConfig")]
    public sealed class SDKUserConfig : ScriptableObject
    {
        public string Username;
        public string Password;
    }
}
