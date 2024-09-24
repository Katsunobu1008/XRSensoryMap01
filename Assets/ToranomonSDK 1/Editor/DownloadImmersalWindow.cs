#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace ToranomonSDK.Editor
{
    public sealed class DownloadImmersalWindow : EditorWindow
    {
        private const string NoImmersalMessage = "プロジェクト内に Immersal SDK がありません。\nImmersal Developer Portal に記載された Developer Token を入力してインポートしてください。";
        private const string DownloadingMessage = "Immersal SDK をダウンロード中...";
        private const string ImmersalAlreadyExistsMessage = "Immersal SDK は既にプロジェクト内にインポートされています。";

        private const string BasePath = "Assets/ToranomonSDK/Editor";
        private const string ImmersalSDKUrl = "https://api.immersal.com/download";
        private const string ImmersalSDKName = "ImmersalSDKv1_19_1.unitypackage";
        private Label _statusLabel;
        private TextField _immersalTokenField;
        private Button _downloadButton;

        public static void OpenWindow()
        {
            GetWindow<DownloadImmersalWindow>("Immersal SDK のインポート");
        }

        private void CreateGUI()
        {
            try
            {
                var root = rootVisualElement;
                root.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{BasePath}/DownloadImmersalWindow.uxml").Instantiate());

                _statusLabel = root.Q<Label>(name: "status-label");
                _immersalTokenField = root.Q<TextField>(name: "immersal-token");
                _downloadButton = root.Q<Button>(name: "download-button");
                _downloadButton.clicked += DownloadImmersalClicked;

                var immersalAsmdef = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/ImmersalSDK/ImmersalSDK.asmdef");
                if (immersalAsmdef == null)
                {
                    _statusLabel.text = NoImmersalMessage;
                }
                else
                {
                    _statusLabel.text = ImmersalAlreadyExistsMessage;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Close();
            }
        }

        private async void DownloadImmersalClicked()
        {
            var token = _immersalTokenField.value;
            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("Immersal Developer Token が入力されていません");
                return;
            }
            _downloadButton.SetEnabled(false);
            var text = _statusLabel.text;
            _statusLabel.text = DownloadingMessage;
            try
            {
                var uri = $"{ImmersalSDKUrl}?token={token}&name={ImmersalSDKName}";
                var data = await UnityWebRequest.Get(uri).RequestBytes(progress =>
                {
                    _statusLabel.text = $"{DownloadingMessage} ({progress * 100:N0}%)";
                });
                var tmpfile = Path.GetTempFileName();
                try
                {
                    File.WriteAllBytes(tmpfile, data);
                    AssetDatabase.ImportPackage(tmpfile, false);
                }
                finally
                {
                    File.Delete(tmpfile);
                }

                SetSymbol();

                _statusLabel.text = ImmersalAlreadyExistsMessage;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                _statusLabel.text = text;
            }
            finally
            {
                _downloadButton.SetEnabled(true);
            }
        }

        private void SetSymbol()
        {
            const string SYMBOL = "TORANOMON_SDK_IMMERSAL_INSTALLED";

            var buildTargets = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
            foreach (var target in buildTargets)
            {
                var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Split(";", StringSplitOptions.RemoveEmptyEntries);
                if (symbols.Contains(SYMBOL) == false)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(target, symbols.Append(SYMBOL).ToArray());
                }
            }
        }
    }
}
#endif
