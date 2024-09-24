using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace ToranomonSDK
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    internal sealed class CustomLabelAttribute : PropertyAttribute
    {
        public string Label { get; }

        public CustomLabelAttribute(string label)
        {
            Label = label;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MonoBehaviour), true)]
    internal class CustomInspectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(
                "Script",
                MonoScript.FromMonoBehaviour(target as MonoBehaviour),
                typeof(MonoScript),
                false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            serializedObject.Update();

            var fields = target
                .GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.IsPublic || field.GetCustomAttribute<SerializeField>(true) != null)
                .Select(field =>
                {
                    var serializedProp = serializedObject.FindProperty(field.Name);
                    var labelAttr = field.GetCustomAttribute<CustomLabelAttribute>(true);
                    return (
                        SerializedProp: serializedProp,
                        Name: labelAttr?.Label ?? serializedProp?.displayName
                        );
                })
                .Where(x => x.SerializedProp != null);
            foreach (var (prop, n) in fields)
            {
                EditorGUILayout.PropertyField(prop, new GUIContent(n), true);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(ScriptableObject), true)]
    internal class CustomInspectorEditorForScriptableObject : CustomInspectorEditor { }

#endif
}
