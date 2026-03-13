using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class SceneManagementPage : SettingPageBase
    {
        public override string Name => "SceneEnumGenerator";

        private const string SceneEnumGeneratorTimingKey = "SceneEnumGenerator.GenerateTiming";

        public static GenerateTiming Timing
        {
            get => (GenerateTiming)EditorPrefs.GetInt(SceneEnumGeneratorTimingKey, (int)GenerateTiming.OnSceneListUpdate);
            private set => EditorPrefs.SetInt(SceneEnumGeneratorTimingKey, (int)value);
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Scene Enum Generator Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Auto Generation Settings", EditorStyles.miniBoldLabel);
                
                Timing = (GenerateTiming)EditorGUILayout.EnumPopup("Generate Timing", Timing);

                string helpText = "Select when to automatically generate the SceneEnum.\n" +
                                 "- None: Manual only.\n" +
                                 "- OnSceneListUpdate: Automatically update when scenes are added/removed in the project or Build Settings.";
                EditorGUILayout.HelpBox(helpText, MessageType.Info);
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