using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UsefulTools.Editor;

namespace UsefulTools.Editor
{
    [InitializeOnLoad]
    public class SceneEnumGenerator
    {
        /// <summary>
        /// Enum生成完了時に発行されるイベント
        /// </summary>
        public static event Action OnGenerated;

        static SceneEnumGenerator()
        {
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
        }

        private static void OnSceneListChanged()
        {
            if (SceneManagementPage.Timing != GenerateTiming.None)
            {
                Generate();
            }
        }

        private static void OnNewSceneCreated(UnityEngine.SceneManagement.Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            if (SceneManagementPage.Timing != GenerateTiming.None)
            {
                Generate();
            }
        }

        [MenuItem("UsefulTools/Generate/Scene Enum")]
        public static void Generate()
        {
            string targetPath = PathSettingPage.SceneSearchPath;
            string outputPath = PathSettingPage.EnumOutputPath;

            // ターゲットフォルダが存在するか確認
            if (!AssetDatabase.IsValidFolder(targetPath))
            {
                Debug.LogWarning($"[UsefulTools] Target scenes directory not found: {targetPath}");
                return;
            }

            // 指定ディレクトリ内の全シーンファイルを取得
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { targetPath });
            var scenePaths = sceneGuids.Select(AssetDatabase.GUIDToAssetPath).Distinct().ToArray();

            // フィルタリング
            string[] ignorePatterns = SceneManagementPage.IgnorePatterns
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            if (ignorePatterns.Length > 0)
            {
                scenePaths = scenePaths.Where(path => 
                    !ignorePatterns.Any(pattern => path.Contains(pattern))
                ).ToArray();
            }

            if (scenePaths.Length == 0)
            {
                Debug.Log($"[UsefulTools] No scenes found in {targetPath}");
                return;
            }

            // BuildSettings に登録されているかどうかの判定用
            var buildScenes = EditorBuildSettings.scenes.ToDictionary(s => s.path, s => s.enabled);

            var includedScenesList = new System.Collections.Generic.List<string>();
            var excludedScenesList = new System.Collections.Generic.List<string>();

            foreach (var path in scenePaths)
            {
                string sceneName = Path.GetFileNameWithoutExtension(path);
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
            code.AppendLine($"namespace {SceneManagementPage.Namespace}");
            code.AppendLine("{");
            code.AppendLine($"    public enum {SceneManagementPage.InListEnumName}");
            code.AppendLine("    {");
            foreach (var scene in includedScenesList)
                code.AppendLine($"        {scene},");
            code.AppendLine("    }\n");

            code.AppendLine($"    public enum {SceneManagementPage.OutListEnumName}");
            code.AppendLine("    {");
            foreach (var scene in excludedScenesList)
                code.AppendLine($"        {scene},");
            code.AppendLine("    }");
            code.AppendLine("}");

            // 書き出し処理
            string dir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(outputPath, code.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
            
            Debug.Log($"[UsefulTools] SceneEnum generated to {outputPath}");

            // イベント発行
            OnGenerated?.Invoke();
        }
    }
}
