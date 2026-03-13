using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class PathSettingPage : SettingPageBase
    {
        public override string Name => "Paths";

        private const string SceneSearchPathKey = "UsefulTools.Path.SceneSearch";
        private const string EnumOutputPathKey = "UsefulTools.Path.EnumOutput";

        public static string SceneSearchPath
        {
            get => EditorPrefs.GetString(SceneSearchPathKey, "Assets/Level/Scenes");
            set => EditorPrefs.SetString(SceneSearchPathKey, value);
        }

        public static string EnumOutputPath
        {
            get => EditorPrefs.GetString(EnumOutputPathKey, "Assets/Code/AutoGenerate/SceneEnum.cs");
            set => EditorPrefs.SetString(EnumOutputPathKey, value);
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Path Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Search & Output Paths", EditorStyles.miniBoldLabel);
                
                DrawPathField("Scene Search Root", SceneSearchPath, path => SceneSearchPath = path, true);
                DrawPathField("Enum Output File", EnumOutputPath, path => EnumOutputPath = path, false);
            }
        }

        private void DrawPathField(string label, string currentPath, System.Action<string> onPathChanged, bool isFolder)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                string newPath = EditorGUILayout.TextField(label, currentPath);
                if (newPath != currentPath) onPathChanged(newPath);

                if (GUILayout.Button("Browse", GUILayout.Width(60)))
                {
                    string selectedPath = isFolder 
                        ? EditorUtility.OpenFolderPanel("Select Folder", "Assets", "")
                        : EditorUtility.SaveFilePanel("Select File Location", "Assets", "SceneEnum", "cs");

                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        // プロジェクト相対パスに変換
                        if (selectedPath.StartsWith(Application.dataPath))
                        {
                            onPathChanged("Assets" + selectedPath.Substring(Application.dataPath.Length));
                        }
                        else
                        {
                            Debug.LogWarning("Please select a path within the Assets folder.");
                        }
                    }
                }
            }
        }
    }
}
