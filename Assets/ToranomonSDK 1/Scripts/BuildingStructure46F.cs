using System.Collections.Generic;
using UnityEngine;

namespace ToranomonSDK
{
    [RequireComponent(typeof(BuildingModel))]
    internal sealed class BuildingStructure46F : MonoBehaviour
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
        [UnityEditor.CustomEditor(typeof(BuildingStructure46F))]
        private sealed class BuildingStructure46FEditor : BuildingStructureEditor
        {
            protected override IReadOnlyDictionary<string, string[]> GroupDefs => new Dictionary<string, string[]>
            {
                ["天井"] = new string[]
                {
                    "金属 - 鉄 ダークグレー塗装",
                    "Rhino - 011",
                    "FM 光膜フレーム",
                    "Rhino - 004",
                    "FM 光膜",
                    "金属 - メッキ",
                    "金属 - 銅",
                    "FM 光膜フレーム 小口3",
                    "FM 光膜フレーム 小口1",
                    "Rhino - 001",
                    "プラスチック - 光沢 白",
                    "金属 - ビジ発バトン 道具",
                    "金属 - ビジ発バトン 照明",
                },
                ["壁"] = new string[]
                {
                    "Rhino - 002",
                    "金属 - アルミニウム",
                    "漆喰 - 白 ファイン",
                    "断熱材 - グラスウール 黒",
                    "LGS",
                    "ガラス - シンプル 青 ライト",
                    "金属 - ビジ発バトン 暗幕",
                    "ペイント - サンドベージュ",
                    "金属 - 鋼製床(W250)",
                    "金属 - 鋼製床 小口",
                    "C-99",
                    "金属 - 亜鉛",
                    "耐火被覆 マキエベ",
                },
                ["椅子・窓枠"] = new string[]
                {
                    "Rhino - 006",
                },
            };
        }
#endif
    }
}
