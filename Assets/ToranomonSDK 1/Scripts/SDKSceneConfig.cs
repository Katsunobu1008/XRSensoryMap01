using System;
using UnityEngine;
using System.Linq;
using System.Collections.ObjectModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ToranomonSDK
{
    [CreateAssetMenu(fileName = nameof(SDKSceneConfig), menuName = "ToranomonSDK/SDKSceneConfig")]
    public sealed class SDKSceneConfig : ScriptableObject
    {
        [SerializeField]
        private ContentsSceneInfo[] _scenes;

        public ReadOnlyCollection<ContentsSceneInfo> Scenes =>
            new ReadOnlyCollection<ContentsSceneInfo>(_scenes ?? new ContentsSceneInfo[0]);

        public void SetScenes(ContentsSceneInfo[] scenes)
        {
            _scenes = scenes;
        }
    }

    [Serializable]
    public sealed class ContentsSceneInfo
    {
        [SerializeField]
        private SceneObject _scene;
        [SerializeField]
        private float _minHeight;
        [SerializeField]
        private float _maxHeight;

        public SceneObject Scene => _scene;
        public float MinHeight => _minHeight;
        public float MaxHeight => _maxHeight;

        public ContentsSceneInfo(SceneObject scene, float minHeight, float maxHeight)
        {
            _scene = scene;
            _minHeight = minHeight;
            _maxHeight = maxHeight;
        }
    }

    [Serializable]
    public sealed class SceneObject
    {
        [SerializeField]
        private string _scenePath;

        public string ScenePath => _scenePath;

        internal const string ScenePathProperty = nameof(_scenePath);

#if UNITY_EDITOR

        public SceneAsset SceneAsset
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<SceneAsset>(_scenePath);
            }
        }

        public SceneObject(string scenePath)
        {
            _scenePath = scenePath;
        }
#else
        private SceneObject()
        {
        }
#endif
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneObject))]
    internal sealed class SceneObjectEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var scenePathProp = property.FindPropertyRelative(SceneObject.ScenePathProperty);
            var currentPath = scenePathProp.stringValue;
            var currentSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(currentPath);
            var newSceneValue = EditorGUI.ObjectField(position, label, currentSceneAsset, typeof(SceneAsset), false) as SceneAsset;

            var newPath = AssetDatabase.GetAssetOrScenePath(newSceneValue);
            if (newSceneValue == null || string.IsNullOrEmpty(newPath))
            {
                scenePathProp.stringValue = "";
                return;
            }

            if (newPath != currentPath)
            {
                // value changed
                if (EditorBuildSettings.scenes.Any(x => x.path == newPath) == false)
                {
                    Debug.LogError($"シーン {newSceneValue.name} はビルドに含まれていません。このシーンをプロジェクトのビルド設定に追加してください。");
                    return;
                }
                scenePathProp.stringValue = newPath;
            }
        }
    }
#endif
}
