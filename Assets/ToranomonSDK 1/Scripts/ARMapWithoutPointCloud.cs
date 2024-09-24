#if TORANOMON_SDK_IMMERSAL_INSTALLED
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using Immersal.AR;

namespace ToranomonSDK
{
    [ExecuteAlways]
    public class ARMapWithoutPointCloud : ARMap
    {
        public override async Task<int> LoadMap(byte[] mapBytes = null, int mapId = -1)
        {
            if (mapBytes == null)
            {
                mapBytes = (mapFile != null) ? mapFile.bytes : null;
            }

            if (mapBytes != null)
            {
                var handle = await Task.Run(() => Immersal.Core.LoadMap(mapBytes));
                SetMapHandle(handle);

                if (this == null)
                {
                    FreeMap();
                    return -1;
                }
            }

            if (mapId > 0)
            {
                SetMapId(mapId);
            }
            else
            {
                ParseMapIdAndName();
            }

            if (mapHandle >= 0)
            {
                mapHandleToMap[mapHandle] = this;
            }

            if (this.mapId > 0 && m_ARSpace != null)
            {
                root = m_ARSpace.transform;
                ARSpace.RegisterSpace(root, this, transform.localPosition, transform.localRotation, transform.localScale);
            }
            return mapHandle;
        }

        public override void FreeMap(bool destroy = false)
        {
            if (mapHandle >= 0)
            {
                var handle = mapHandle;
                // fire and forget
                Task.Run(() => Immersal.Core.FreeMap(handle));
            }
            SetMapHandle(-1);
            ClearMesh();
            Reset();
            if (this.mapId > 0)
            {
                ARSpace.UnregisterSpace(root, this.mapId);
                SetMapId(-1);
            }
            if (destroy)
            {
                GameObject.Destroy(gameObject);
            }
        }

        private void SetMapId(int value)
        {
            // access private property of ARMap by reflection
            typeof(ARMap).GetProperty("mapId").SetValue(this, value);
        }

        private void SetMapHandle(int value)
        {
            // access private property of ARMap by reflection
            typeof(ARMap).GetProperty("mapHandle").SetValue(this, value);
        }

        private void ClearMesh()
        {
            // access private method of ARMap by reflection
            typeof(ARMap).GetMethod("ClearMesh", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null);
        }

        private void ParseMapIdAndName()
        {
            // access private method of ARMap by reflection
            typeof(ARMap).GetMethod("ParseMapIdAndName", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null);
        }
    }
}
#endif
