using UnityEngine;

namespace ToranomonSDK
{
    public sealed class BuildingModel : MonoBehaviour
    {
        [SerializeField]
        private Material _occlusionMaterial;

        private void Start()
        {
            var occlusionMaterial = _occlusionMaterial;
            if (occlusionMaterial == null)
            {
                Debug.LogError("OcclusionMaterial がセットされていません");
                return;
            }
            if (Application.isEditor)
            {
                return;
            }

            foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>(true))
            {
                meshRenderer.material = occlusionMaterial;
            }
        }
    }
}
