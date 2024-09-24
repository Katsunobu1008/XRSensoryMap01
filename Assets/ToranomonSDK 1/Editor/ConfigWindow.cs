#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToranomonSDK.Editor
{
    public sealed class ConfigWindow : EditorWindow
    {
        private const string UserConfigPath = "Assets/ToranomonSDK/SDKUserConfig.asset";
        private TextField _apiUsername;
        private TextField _apiPassword;
        private Button _saveButton;
        private Button _downloadImmersalButton;
        private TextField _newProjNameText;
        private Button _createScenesButton;

        [MenuItem("Toranomon SDK/Toranomon SDK 設定...")]
        private static void Open()
        {
            GetWindow<ConfigWindow>("Toranomon SDK 設定");
        }

#if TORANOMON_SDK_DEBUG
        [MenuItem("Toranomon SDK/Toranomon SDK を unitypackage にエクスポート...")]
        private static void StartExportPackage()
        {
            const string PathKey = "ToranomonSDKExportDir";
            var suggestedDir = PlayerPrefs.GetString(PathKey, Application.dataPath);
            if (!Directory.Exists(suggestedDir))
            {
                suggestedDir = Application.dataPath;
            }

            var path = EditorUtility.SaveFilePanel("ToranomonSDK をエクスポート", suggestedDir, "ToranomonSDK.unitypackage", "unitypackage");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

            var assets = Directory.GetFileSystemEntries(Path.Combine(Application.dataPath, "ToranomonSDK"))
                .Select(n => "Assets/" + Path.GetRelativePath(Application.dataPath, n).Replace("\\", "/"))
                .Where(n => n.EndsWith(".meta") == false)
                .Where(n => n != UserConfigPath)
                .ToArray();
            foreach (var asset in assets)
            {
                Debug.Log($"export: '{asset}'");
            }
            PlayerPrefs.SetString(PathKey, dir);
            PlayerPrefs.Save();
            AssetDatabase.ExportPackage(assets, path, ExportPackageOptions.Recurse);
            System.Diagnostics.Process.Start(dir);
        }
#endif

        private void CreateGUI()
        {
            try
            {
                var root = rootVisualElement;
                root.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/ToranomonSDK/Editor/ConfigWindow.uxml").Instantiate());

                _apiUsername = root.Q<TextField>(name: "api-username");
                _apiPassword = root.Q<TextField>(name: "api-password");
                _saveButton = root.Q<Button>(name: "save-button");
                _downloadImmersalButton = root.Q<Button>(name: "download-immersal");
                _newProjNameText = root.Q<TextField>(name: "new-proj-name");
                _createScenesButton = root.Q<Button>(name: "create-scenes");
                _downloadImmersalButton.clicked += DownloadImmersalClicked;
                _saveButton.clicked += SaveClicked;
                _createScenesButton.clicked += CreateScenesClicked;

                // init input fields
                var config = AssetDatabase.LoadAssetAtPath<SDKUserConfig>(UserConfigPath);
                if (config != null)
                {
                    _apiUsername.value = config.Username;
                    _apiPassword.value = config.Password;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Close();
            }
        }

        private void CreateScenesClicked()
        {
            var newProjName = _newProjNameText.value.Trim();
            if (string.IsNullOrEmpty(newProjName))
            {
                Debug.LogError("シーンを作成できませんでした。プロジェクト名を入力してください。");
                return;
            }

            var projFolderPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder($"Assets", newProjName));
            var scenesFolderPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(projFolderPath, "Scenes"));
            var templateScenes = Directory.GetFiles(Application.dataPath + $"/ToranomonSDK/TemplateScenes")
                .Where(x => x.EndsWith(".unity"))
                .Select(x =>
                {
                    var assetPath = "Assets/" + Path.GetRelativePath(Application.dataPath, x).Replace('\\', '/');
                    return (AssetPath: assetPath, Name: Path.GetFileName(assetPath));
                })
                .ToArray();

            var sceneConfigPath = $"{scenesFolderPath}/SDKSceneConfig.asset";
            AssetDatabase.CopyAsset("Assets/ToranomonSDK/TemplateScenes/SDKSceneConfigTemplate.asset", sceneConfigPath);
            var sceneConfig = AssetDatabase.LoadAssetAtPath<SDKSceneConfig>(sceneConfigPath);
            var message = new StringBuilder();
            message.AppendLine($"シーンの作成に成功しました。");
            message.AppendLine($"シーンの作成場所: {scenesFolderPath}");

            var buildScenes = new List<EditorBuildSettingsScene>();
            buildScenes.AddRange(EditorBuildSettings.scenes);
            var newScenes = new List<SceneAsset>();
            var templateRegex = new Regex(@"Template\.unity$");
            foreach (var (assetPath, templateName) in templateScenes)
            {
                var newName = templateRegex.Replace(templateName, ".unity");
                var newPath = $"{scenesFolderPath}/{newName}";
                if (AssetDatabase.CopyAsset(assetPath, newPath))
                {
                    newScenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(newPath));
                    if (newName == "MainScene.unity")
                    {
                        buildScenes.Insert(0, new EditorBuildSettingsScene(newPath, true));
                    }
                    else
                    {
                        buildScenes.Add(new EditorBuildSettingsScene(newPath, true));
                    }
                    message.AppendLine($"  {newName}");
                }
            }
            EditorBuildSettings.scenes = buildScenes.ToArray();
            sceneConfig.SetScenes(sceneConfig.Scenes.Select(x =>
            {
                var asset = x.Scene.SceneAsset;
                var templateScenePath = AssetDatabase.GetAssetPath(asset);
                if (templateRegex.IsMatch(templateScenePath))
                {
                    var newSceneName = Path.GetFileNameWithoutExtension(templateRegex.Replace(templateScenePath, ".unity"));
                    var newScene = newScenes.Find(x => x.name == newSceneName);
                    var newScenePath = AssetDatabase.GetAssetPath(newScene);
                    return new ContentsSceneInfo(new SceneObject(newScenePath), x.MinHeight, x.MaxHeight);
                }
                else
                {
                    Debug.LogError($"シーン {asset.name} はテンプレートシーンではありません。");
                    return new ContentsSceneInfo(null, x.MinHeight, x.MaxHeight);
                }
            }).ToArray());
            EditorUtility.SetDirty(sceneConfig);
            AssetDatabase.SaveAssetIfDirty(sceneConfig);
            Debug.Log(message.ToString());
        }

        private void SaveClicked()
        {
            var username = _apiUsername.value;
            var password = _apiPassword.value;
            var config = AssetDatabase.LoadAssetAtPath<SDKUserConfig>(UserConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<SDKUserConfig>();
                AssetDatabase.CreateAsset(config, UserConfigPath);
            }
            config.Username = username;
            config.Password = password;
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssetIfDirty(config);
            Debug.Log("Toranomon SDK の設定を保存しました。");
        }

        private void DownloadImmersalClicked()
        {
            DownloadImmersalWindow.OpenWindow();
        }
    }
}
#endif
