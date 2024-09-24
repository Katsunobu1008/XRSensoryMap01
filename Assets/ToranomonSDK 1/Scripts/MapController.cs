using UnityEngine;
using System;
using ToranomonSDK.Helpers;
using System.Threading.Tasks;

#if TORANOMON_SDK_IMMERSAL_INSTALLED
using Immersal.AR;
#endif

namespace ToranomonSDK
{
#if TORANOMON_SDK_IMMERSAL_INSTALLED
    [RequireComponent(typeof(ARSpace))]
#endif
    public sealed class MapController : MonoBehaviour
    {
        [SerializeField]
        private MapData[] _mapList;

        [SerializeField]
        private Transform _contentsRoot;

        [SerializeField]
        private string _areaName;
        public string AreaName => _areaName;

        public event Action<int> OnFirstLocalization;

        private static MapController _instance;
        public static MapController Instance
        {
            get => _instance;
            private set
            {
                if (_instance != null)
                {
                    Debug.LogError("MapController はシングルトンです。複数同時に存在してはいけません");
                }
                _instance = value;
            }
        }

#if TORANOMON_SDK_IMMERSAL_INSTALLED
        private async void Awake()
        {
            Instance = this;
            _contentsRoot.gameObject.SetActive(false);
            try
            {
                await Load();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private async Task Load()
        {
            var mapList = _mapList ?? new MapData[0];
            foreach (var mapData in mapList)
            {
                var arMap = mapData.ARMap;
                arMap.OnFirstLocalization ??= new MapLocalizedEvent();
                arMap.OnFirstLocalization.AddListener(mapId =>
                {
                    ToraDebug.Log("on first localization");
                    _contentsRoot.gameObject.SetActive(true);
                    OnFirstLocalization?.Invoke(mapId);
                });

                var mapBytes = mapData.MapFile.bytes;
                var mapId = int.Parse(mapData.MapFile.name.Split("-")[0]);
                // ARMap のインスペクターにマップファイルを設定して読み込ませると
                // Assets/Map Data から metadata を探してくる挙動をするが、そこにないのでエラーになる。
                // そのため、ここで動的にコードから読み込む
                await mapData.ARMap.LoadMap(mapBytes, mapId);
                Debug.Log($"immersal map (mapId: {arMap.mapId}) is loaded");
            }
        }
#endif
    }

    [Serializable]
    public sealed class MapData
    {
        [SerializeField]
        public TextAsset MapFile;
#if TORANOMON_SDK_IMMERSAL_INSTALLED
        [SerializeField]
        public ARMapWithoutPointCloud ARMap;
#endif
    }
}
