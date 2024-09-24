using System.Collections.Generic;
using UnityEngine;

namespace ToranomonSDK
{
    [RequireComponent(typeof(BuildingModel))]
    internal sealed class BuildingStructure8F : MonoBehaviour
    {
        private void Start()
        {
            if (Application.isEditor == false)
            {
                // 実機実行時にはオクルージョン用に使うため、
                // エディタ上での表示/非表示にかかわらず子要素をすべて表示する
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(BuildingStructure8F))]
        private sealed class BuildingStructure8FEditor : BuildingStructureEditor
        {
            protected override IReadOnlyDictionary<string, string[]> GroupDefs => new Dictionary<string, string[]>
            {
                ["天井"] = new string[]
                {
                    "ペイント - アントラシート",
                    "漆喰 - 白 ファイン",
                    "金属 - 銅",
                },
                ["壁"] = new string[]
                {
                    "Rhino - 002",
                    "金属 - アルミニウム",
                    "ガラス - シンプル 青 ライト",
                    "金属 - 亜鉛",
                },
            };
        }
#endif
    }
}
