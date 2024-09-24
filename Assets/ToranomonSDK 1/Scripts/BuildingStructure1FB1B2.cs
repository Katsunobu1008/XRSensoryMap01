using System.Collections.Generic;
using UnityEngine;

namespace ToranomonSDK
{
    [RequireComponent(typeof(BuildingModel))]
    internal sealed class BuildingStructure1FB1B2 : MonoBehaviour
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
        [UnityEditor.CustomEditor(typeof(BuildingStructure1FB1B2))]
        private sealed class BuildingStructure1FB1B2Editor : BuildingStructureEditor
        {
            protected override IReadOnlyDictionary<string, string[]> GroupDefs => new Dictionary<string, string[]>
            {
                ["天井"] = new[]
                {
                    "C-22",
                    "金属 - アルミニウム",
                    "C-01",
                },
                ["壁"] = new[]
                {
                    "漆喰 - 白 ファイン",
                    "Rhino - 013",
                    "ガラス - シンプル 青 ライト",
                    "ペイント - ペールブルー",
                    "コンクリート",
                    "ペイント - サンドベージュ",
                    "LGS",
                },
            };
        }
#endif
    }
}
