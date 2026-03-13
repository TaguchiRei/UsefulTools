using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class DebugSupportTool : SettingPageBase
    {
        public override string Name => "Debug Support";

        // 設定キー
        private const string LogCaptureEnabledKey = "UsefulTools.Debug.LogCaptureEnabled";
        private const string DebugFontSizeKey = "UsefulTools.Debug.FontSize";

        public static bool LogCaptureEnabled
        {
            get => EditorPrefs.GetBool(LogCaptureEnabledKey, false);
            set => EditorPrefs.SetBool(LogCaptureEnabledKey, value);
        }

        public static float FontSize
        {
            get => EditorPrefs.GetFloat(DebugFontSizeKey, 12f);
            set => EditorPrefs.SetFloat(DebugFontSizeKey, value);
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Debug GUI & Log Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // ログ取得設定
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Log Management", EditorStyles.miniBoldLabel);
                LogCaptureEnabled = EditorGUILayout.Toggle("Capture Application Logs", LogCaptureEnabled);
                EditorGUILayout.HelpBox("If enabled, DebugGUI will capture and display logs from Application.logMessageReceived.", MessageType.Info);
            }

            EditorGUILayout.Space();

            // 表示設定
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Appearance Settings", EditorStyles.miniBoldLabel);
                FontSize = EditorGUILayout.Slider("Debug GUI Font Size", FontSize, 8f, 24f);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Debug GUI in Scene", GUILayout.Height(30)))
            {
                DebugGuiGenereater.GenerateDebugGUI();
            }
        }
    }
}
