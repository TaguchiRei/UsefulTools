using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UsefulTools.Editor
{
    [InitializeOnLoad]
    public class SceneEnumGenerator
    {
        private const string OutputPath = "Assets/Code/AutoGenerate/SceneEnum.cs";
        private const string TargetScenesPath = "Assets/Level/Scenes";

        static SceneEnumGenerator()
        {
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
        }

        private static void OnSceneListChanged()
        {
            if (SceneManagementPage.Timing == GenerateTiming.OnSceneListUpdate)
            {
                Generate();
            }
        }

        private static void OnNewSceneCreated(UnityEngine.SceneManagement.Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            if (SceneManagementPage.Timing == GenerateTiming.OnSceneListUpdate)
            {
                Generate();
            }
        }

        [MenuItem("UsefulTools/Generate/Scene Enum")]
        public static void Generate()
        {
            // Assets/Level/Scenes フォルダが存在するか確認
            if (!AssetDatabase.IsValidFolder(TargetScenesPath))
            {
                Debug.LogWarning($"[UsefulTools] Target scenes directory not found: {TargetScenesPath}");
                return;
            }

            // 指定ディレクトリ内の全シーンファイルを取得
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { TargetScenesPath });
            var scenePaths = sceneGuids.Select(AssetDatabase.GUIDToAssetPath).Distinct().ToArray();

            if (scenePaths.Length == 0)
            {
                Debug.Log($"[UsefulTools] No scenes found in {TargetScenesPath}");
                return;
            }

            // BuildSettings に登録されているかどうかの判定用
            var buildScenes = EditorBuildSettings.scenes.ToDictionary(s => s.path, s => s.enabled);

            var includedScenesList = new System.Collections.Generic.List<string>();
            var excludedScenesList = new System.Collections.Generic.List<string>();

            foreach (var path in scenePaths)
            {
                string sceneName = Path.GetFileNameWithoutExtension(path);
                // 名前を正規化
                string normalizedName = Regex.Replace(sceneName, @"[^a-zA-Z0-9_]", "_");

                if (buildScenes.TryGetValue(path, out bool enabled) && enabled)
                {
                    includedScenesList.Add(normalizedName);
                }
                else
                {
                    excludedScenesList.Add(normalizedName);
                }
            }

            // コード生成
            StringBuilder code = new StringBuilder();
            code.AppendLine("// 自動生成ファイルの為、手動での編集は上書きされます。");
            code.AppendLine("namespace UsefulTools.AutoGenerate");
            code.AppendLine("{");
            code.AppendLine("    public enum InListSceneName");
            code.AppendLine("    {");
            foreach (var scene in includedScenesList)
                code.AppendLine($"        {scene},");
            code.AppendLine("    }\n");

            code.AppendLine("    public enum OutListSceneName");
            code.AppendLine("    {");
            foreach (var scene in excludedScenesList)
                code.AppendLine($"        {scene},");
            code.AppendLine("    }");
            code.AppendLine("}");

            // 書き出し処理
            string dir = Path.GetDirectoryName(OutputPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(OutputPath, code.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
            Debug.Log($"[UsefulTools] SceneEnum generated from {TargetScenesPath}");
        }
    }
}