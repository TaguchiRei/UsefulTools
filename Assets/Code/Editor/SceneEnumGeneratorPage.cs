using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class SceneEnumGeneratorPage : SettingPageBase
    {
        public override string Name => "SceneEnumGenerator";

        private const string SceneEnumGeneratorGenerateOnCreateKey = "SceneEnumGenerator.GenerateOnCreate";

        public static bool GenerateOnCreate
        {
            get => EditorPrefs.GetBool(SceneEnumGeneratorGenerateOnCreateKey, false);
            private set => EditorPrefs.SetBool(SceneEnumGeneratorGenerateOnCreateKey, value);
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Auto Generation Settings", EditorStyles.miniBoldLabel);
                GenerateOnCreate = EditorGUILayout.ToggleLeft("Scene Enum: Generate on Create", GenerateOnCreate);
                EditorGUILayout.HelpBox(
                    "If enabled, SceneEnum will be automatically updated when a new Scene is created.",
                    MessageType.Info);
            }
        }
    }
}