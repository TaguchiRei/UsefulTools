using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace UsefulTools.Editor
{
    /// <summary>
    /// InputActionAssetからActionMapとActionのenumを自動生成するエディタ拡張
    /// </summary>
    public class InputActionEnumGenerator
    {
        public static void GenerateAll()
        {
            var guids = AssetDatabase.FindAssets("t:InputActionAsset");
            bool anyGenerated = false;

            // フィルタリング設定を取得
            string[] ignorePatterns = InputSupportTool.IgnorePatterns
                .Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                
                // 除外パターンに一致するか確認
                if (ignorePatterns.Any(pattern => path.Contains(pattern))) continue;

                var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
                if (asset != null)
                {
                    if (Generate(asset))
                    {
                        anyGenerated = true;
                    }
                }
            }

            if (anyGenerated)
            {
                AssetDatabase.Refresh();
            }
        }

        public static bool Generate(InputActionAsset asset)
        {
            if (asset == null) return false;

            string folderPath = InputSupportTool.OutputFolder;
            string ns = InputSupportTool.Namespace;

            string directoryPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", folderPath));
            string sanitizedAssetName = SanitizeName(asset.name);
            string filePath = Path.Combine(directoryPath, $"{sanitizedAssetName}Enum.cs");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var sb = new StringBuilder();

            sb.AppendLine("// 自動生成ファイルの為、手動での編集は上書きされます。");
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
            // アセット名のクラスで囲むことで、複数のアセットがあってもEnum名が衝突しないようにする
            sb.AppendLine($"    public static class {sanitizedAssetName}");
            sb.AppendLine("    {");

            // 1. ActionMapのenumを生成
            sb.AppendLine("        public enum ActionMaps");
            sb.AppendLine("        {");
            int i = 0;
            foreach (var map in asset.actionMaps)
            {
                sb.AppendLine($"            {SanitizeName(map.name)} = {i},");
                i++;
            }
            sb.AppendLine("        }");
            sb.AppendLine();

            // 2. 各ActionMapに対応するActionのenumを生成
            foreach (var map in asset.actionMaps)
            {
                string actionEnumName = $"{SanitizeName(map.name)}Actions";
                sb.AppendLine($"        public enum {actionEnumName}");
                sb.AppendLine("        {");

                i = 0;
                foreach (var action in map.actions)
                {
                    sb.AppendLine($"            {SanitizeName(action.name)} = {i},");
                    i++;
                }

                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            try
            {
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Debug.Log($"[UsefulTools] Input enums generated for '{asset.name}' at: {filePath}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[UsefulTools] Failed to generate input enums for '{asset.name}'. {e}");
                return false;
            }
        }

        private static string SanitizeName(string name)
        {
            string sanitized = Regex.Replace(name, @"[^a-zA-Z0-9_]", "");
            if (string.IsNullOrEmpty(sanitized))
            {
                return "_";
            }

            if (char.IsDigit(sanitized[0]))
            {
                return "_" + sanitized;
            }

            return sanitized;
        }
    }
}
