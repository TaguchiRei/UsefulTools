using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace UsefulTools.Editor
{
    /// <summary>
    /// Project-wide Actionsに設定されたInputActionAssetから
    /// ActionMapとActionのenumを自動生成するエディタ拡張
    /// </summary>
    public class InputActionEnumGenerator
    {
        public static void GenerateAllEnums()
        {
            var asset = InputSystem.actions;

            if (asset == null)
            {
                Debug.LogError("[UsefulTools] Project-wide Actions が設定されていません。\nProject Settings > Input System Package > Project-wide Actions を設定してください。");
                return;
            }

            if (Generate(asset))
            {
                AssetDatabase.Refresh();
            }
        }

        public static bool Generate(InputActionAsset asset)
        {
            if (asset == null) return false;

            string folderPath = InputSupportTool.OutputFolder;

            string directoryPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", folderPath));
            string sanitizedAssetName = SanitizeName(asset.name);
            string filePath = Path.Combine(directoryPath, $"{sanitizedAssetName}Enum.cs");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var sb = new StringBuilder();

            sb.AppendLine("// 自動生成ファイルの為、手動での編集は上書きされます。");

            // ActionMap enum
            sb.AppendLine("public enum ActionMaps");
            sb.AppendLine("{");

            int i = 0;
            foreach (var map in asset.actionMaps)
            {
                sb.AppendLine($"    {SanitizeName(map.name)} = {i},");
                i++;
            }

            sb.AppendLine("}");
            sb.AppendLine();

            // Action enum per ActionMap
            foreach (var map in asset.actionMaps)
            {
                string actionEnumName = $"{SanitizeName(map.name)}Actions";

                sb.AppendLine($"public enum {actionEnumName}");
                sb.AppendLine("{");

                i = 0;
                foreach (var action in map.actions)
                {
                    sb.AppendLine($"    {SanitizeName(action.name)} = {i},");
                    i++;
                }

                sb.AppendLine("}");
                sb.AppendLine();
            }

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