#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace ToranomonSDK
{
    internal abstract class BuildingStructureEditor : UnityEditor.Editor
    {
        private sealed class ObjectGroup
        {
            public Transform[] Objects { get; }
            public bool IsVisible { get; set; }
            public ObjectGroup(Transform[] objects, bool isVisible)
            {
                Objects = objects;
                IsVisible = isVisible;
            }
        }

        private readonly Dictionary<string, ObjectGroup> _groups = new Dictionary<string, ObjectGroup>();

        protected abstract IReadOnlyDictionary<string, string[]> GroupDefs { get; }

        private void OnEnable()
        {
            var instance = target as MonoBehaviour;
            if (instance == null) { return; }
            var transform = instance.transform;
            foreach (var (groupName, nameList) in GroupDefs)
            {
                var objects = nameList.Select(n => transform.Find(n)).ToArray();
                var isVisible = objects.Length > 0 ? objects[0].gameObject.activeSelf : false;
                _groups[groupName] = new ObjectGroup(objects, isVisible);
            }
        }

        public override void OnInspectorGUI()
        {
            var instance = target as MonoBehaviour;
            if (instance == null)
            {
                return;
            }
            foreach (var (groupName, group) in _groups)
            {
                var oldValue = group.IsVisible;
                group.IsVisible = EditorGUILayout.Toggle(groupName, oldValue);
                if (oldValue != group.IsVisible)
                {
                    foreach (var obj in group.Objects)
                    {
                        obj.gameObject.SetActive(group.IsVisible);
                    }
                }
            }
        }
    }
}
#endif
