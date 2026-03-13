using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class SceneManagementPage : SettingPageBase
    {
        public override string Name => "Scene Management";

        private const string SceneEnumGeneratorTimingKey = "SceneEnumGenerator.GenerateTiming";
        private const string SceneEnumNamespaceKey = "UsefulTools.Path.EnumNamespace";
        private const string InListEnumNameKey = "UsefulTools.Path.InListEnumName";
        private const string OutListEnumNameKey = "UsefulTools.Path.OutListEnumName";
        private const string IgnorePatternsKey = "UsefulTools.Path.IgnorePatterns";

        public static GenerateTiming Timing
        {
            get => (GenerateTiming)EditorPrefs.GetInt(SceneEnumGeneratorTimingKey, (int)GenerateTiming.OnSceneListUpdate);
            private set => EditorPrefs.SetInt(SceneEnumGeneratorTimingKey, (int)value);
        }

        public static string Namespace
        {
            get => EditorPrefs.GetString(SceneEnumNamespaceKey, "UsefulTools.AutoGenerate");
            set => EditorPrefs.SetString(SceneEnumNamespaceKey, value);
        }

        public static string InListEnumName
        {
            get => EditorPrefs.GetString(InListEnumNameKey, "InListSceneName");
            set => EditorPrefs.SetString(InListEnumNameKey, value);
        }

        public static string OutListEnumName
        {
            get => EditorPrefs.GetString(OutListEnumNameKey, "OutListSceneName");
            set => EditorPrefs.SetString(OutListEnumNameKey, value);
        }

        public static string IgnorePatterns
        {
            get => EditorPrefs.GetString(IgnorePatternsKey, "");
            set => EditorPrefs.SetString(IgnorePatternsKey, value);
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Scene Enum Generator Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Auto Generation Timing", EditorStyles.miniBoldLabel);
                Timing = (GenerateTiming)EditorGUILayout.EnumPopup("Generate Timing", Timing);

                string helpText = "Select when to automatically generate the SceneEnum.\n" +
                                 "- None: Manual only.\n" +
                                 "- OnSceneListUpdate: Automatically update when scenes are added/removed in the project or Build Settings.\n" +
                                 "- OnSceneLoaderUpdate: Same as above, and also triggers immediate refresh of SceneLoader window.";
                EditorGUILayout.HelpBox(helpText, MessageType.Info);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Code Structure Settings", EditorStyles.miniBoldLabel);
                Namespace = EditorGUILayout.TextField("Namespace", Namespace);
                InListEnumName = EditorGUILayout.TextField("In-List Enum Name", InListEnumName);
                OutListEnumName = EditorGUILayout.TextField("Out-List Enum Name", OutListEnumName);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Filter Settings", EditorStyles.miniBoldLabel);
                IgnorePatterns = EditorGUILayout.TextField("Ignore Patterns", IgnorePatterns);
                EditorGUILayout.HelpBox("Comma separated (e.g., 'Test, _Temp'). Scenes containing these strings will be excluded.", MessageType.Info);
            }
        }
    }

    public enum GenerateTiming
    {
        None = 0,
        OnSceneListUpdate = 1,
        OnSceneLoaderUpdate = 2,
    }
}