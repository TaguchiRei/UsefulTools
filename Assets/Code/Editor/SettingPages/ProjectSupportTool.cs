using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace UsefulTools.Editor
{
    /// <summary>
    /// フォルダ階層を表現するクラス
    /// </summary>
    [System.Serializable]
    public class DirectoryHierarchy
    {
        public string folderName;
        public string folderPath;
        public List<DirectoryHierarchy> children = new List<DirectoryHierarchy>();
        public bool isExpanded = false;

        public DirectoryHierarchy(string name, string path)
        {
            folderName = name;
            folderPath = path;
        }
    }

    public class ProjectSupportTool : SettingPageBase
    {
        public override string Name => "Project Support";

        // --- Package Keys ---
        private const string UniTaskUrlKey = "UsefulTools.Code.UniTaskUrl";
        private const string VContainerUrlKey = "UsefulTools.Code.VContainerUrl";
        private const string AddressablesPkgKey = "UsefulTools.Code.AddressablesPkg";

        private const string DefaultUniTaskUrl = "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";
        private const string DefaultVContainerUrl = "https://github.com/hadashiA/VContainer.git";
        private const string DefaultAddressablesPkg = "com.unity.addressables";

        public static string UniTaskUrl { get => EditorPrefs.GetString(UniTaskUrlKey, DefaultUniTaskUrl); set => EditorPrefs.SetString(UniTaskUrlKey, value); }
        public static string VContainerUrl { get => EditorPrefs.GetString(VContainerUrlKey, DefaultVContainerUrl); set => EditorPrefs.SetString(VContainerUrlKey, value); }
        public static string AddressablesPkg { get => EditorPrefs.GetString(AddressablesPkgKey, DefaultAddressablesPkg); set => EditorPrefs.SetString(AddressablesPkgKey, value); }

        private static AddRequest _request;

        // --- Hierarchy Fields ---
        private static List<DirectoryHierarchy> _intendedStructure;
        private List<DirectoryHierarchy> _currentStructure;
        private bool _showIntended = true;
        private bool _showCurrent = true;

        public override void Initialize()
        {
            RefreshIntendedStructure();
        }

        private void RefreshIntendedStructure()
        {
            _intendedStructure = new List<DirectoryHierarchy>();
            var art = new DirectoryHierarchy("Art", "Assets/Art");
            art.children.Add(new DirectoryHierarchy("Materials", "Assets/Art/Materials"));
            art.children.Add(new DirectoryHierarchy("Models", "Assets/Art/Models"));
            art.children.Add(new DirectoryHierarchy("Textures", "Assets/Art/Textures"));

            var code = new DirectoryHierarchy("Code", "Assets/Code");
            code.children.Add(new DirectoryHierarchy("Scripts", "Assets/Code/Scripts"));
            code.children.Add(new DirectoryHierarchy("Editor", "Assets/Code/Editor"));

            _intendedStructure.Add(art);
            _intendedStructure.Add(code);
            _intendedStructure.Add(new DirectoryHierarchy("Audio", "Assets/Audio"));
            _intendedStructure.Add(new DirectoryHierarchy("Docs", "Assets/Docs"));
            _intendedStructure.Add(new DirectoryHierarchy("Level", "Assets/Level"));
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Project Setup & Hierarchy Support", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 1. Package Import Settings
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Package Import URLs", EditorStyles.miniBoldLabel);
                UniTaskUrl = EditorGUILayout.TextField("UniTask URL", UniTaskUrl);
                VContainerUrl = EditorGUILayout.TextField("VContainer URL", VContainerUrl);
                AddressablesPkg = EditorGUILayout.TextField("Addressables Package", AddressablesPkg);

                EditorGUILayout.Space(5);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Import UniTask")) Install(UniTaskUrl);
                    if (GUILayout.Button("Import VContainer")) Install(VContainerUrl);
                    if (GUILayout.Button("Import Addressables")) Install(AddressablesPkg);
                }
                if (GUILayout.Button("Reset to Default URLs"))
                {
                    UniTaskUrl = DefaultUniTaskUrl;
                    VContainerUrl = DefaultVContainerUrl;
                    AddressablesPkg = DefaultAddressablesPkg;
                }
            }

            EditorGUILayout.Space();

            // 2. Intended Hierarchy
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _showIntended = EditorGUILayout.Foldout(_showIntended, "Intended Structure Preview (Standard)", true);
                if (_showIntended)
                {
                    DrawHierarchyList(_intendedStructure, 0);
                    if (GUILayout.Button("Apply Intended Structure (Create Folders)", GUILayout.Height(30)))
                    {
                        ApplyStructure(_intendedStructure);
                        ScanCurrentStructure();
                    }
                }
            }

            EditorGUILayout.Space();

            // 3. Current Hierarchy
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    _showCurrent = EditorGUILayout.Foldout(_showCurrent, "Current Folder Structure (Actual)", true);
                    if (GUILayout.Button("Scan", GUILayout.Width(60))) ScanCurrentStructure();
                }

                if (_showCurrent)
                {
                    if (_currentStructure == null || _currentStructure.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Press 'Scan' to visualize current project structure.", MessageType.Info);
                    }
                    else
                    {
                        DrawHierarchyList(_currentStructure, 0);
                    }
                }
            }
        }

        private void DrawHierarchyList(List<DirectoryHierarchy> list, int depth)
        {
            if (list == null) return;
            foreach (var item in list)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(depth * 16);
                    bool hasChildren = item.children != null && item.children.Count > 0;
                    Rect rect = GUILayoutUtility.GetRect(16, 16, GUILayout.ExpandWidth(false));
                    if (hasChildren) item.isExpanded = EditorGUI.Foldout(rect, item.isExpanded, "", true);

                    bool exists = AssetDatabase.IsValidFolder(item.folderPath);
                    GUILayout.Label(exists ? "📁" : "⚪", GUILayout.Width(18));
                    EditorGUILayout.LabelField(item.folderName, EditorStyles.miniLabel);
                    GUILayout.FlexibleSpace();
                }
                if (item.isExpanded && item.children != null && item.children.Count > 0) DrawHierarchyList(item.children, depth + 1);
            }
        }

        private void ScanCurrentStructure()
        {
            _currentStructure = new List<DirectoryHierarchy>();
            string[] dirs = Directory.GetDirectories(Application.dataPath, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                string folderName = Path.GetFileName(dir);
                _currentStructure.Add(BuildHierarchyRecursive("Assets/" + folderName));
            }
        }

        private DirectoryHierarchy BuildHierarchyRecursive(string relativePath)
        {
            var hierarchy = new DirectoryHierarchy(Path.GetFileName(relativePath), relativePath);
            string systemPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6), relativePath);
            if (Directory.Exists(systemPath))
            {
                foreach (var subDir in Directory.GetDirectories(systemPath))
                {
                    hierarchy.children.Add(BuildHierarchyRecursive(relativePath + "/" + Path.GetFileName(subDir)));
                }
            }
            return hierarchy;
        }

        private void ApplyStructure(List<DirectoryHierarchy> list)
        {
            foreach (var item in list)
            {
                if (!AssetDatabase.IsValidFolder(item.folderPath))
                {
                    AssetDatabase.CreateFolder(Path.GetDirectoryName(item.folderPath).Replace("\\", "/"), Path.GetFileName(item.folderPath));
                }
                if (item.children.Count > 0) ApplyStructure(item.children);
            }
            AssetDatabase.Refresh();
        }

        private void Install(string identifier)
        {
            if (string.IsNullOrEmpty(identifier)) return;
            _request = Client.Add(identifier);
            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            if (_request == null) { EditorApplication.update -= Progress; return; }
            if (_request.IsCompleted)
            {
                if (_request.Status == StatusCode.Success) Debug.Log("[UsefulTools] Package installed successfully.");
                else if (_request.Status >= StatusCode.Failure) Debug.LogError($"[UsefulTools] Package install failed: {_request.Error.message}");
                EditorApplication.update -= Progress;
                _request = null;
            }
        }
    }
}
