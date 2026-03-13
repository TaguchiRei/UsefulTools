using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class SceneLoader : EditorWindow
    {
        private string[] _onListScenes;
        private string[] _outListScenes;
        private Vector2 _onListScroll;
        private Vector2 _outListScroll;

        private const string TargetScenesPath = "Assets/Level/Scenes";

        [MenuItem("UsefulTools/Scene Loader")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneLoader>("Scene Loader");
            window.Initialize();
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _onListScenes = Enum.GetNames(typeof(InListSceneName));
                _outListScenes = Enum.GetNames(typeof(OutListSceneName));
            }
            catch
            {
                // Enumがまだ生成されていない場合など
                _onListScenes = Array.Empty<string>();
                _outListScenes = Array.Empty<string>();
            }
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("シーンリストを更新", EditorStyles.toolbarButton))
                {
                    Initialize();
                }
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space(5);

            using (new EditorGUILayout.HorizontalScope())
            {
                // On List Scenes
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Build Settings (Included)", EditorStyles.boldLabel);
                    using (var scroll = new EditorGUILayout.ScrollViewScope(_onListScroll, EditorStyles.helpBox))
                    {
                        _onListScroll = scroll.scrollPosition;
                        DrawSceneButtons(_onListScenes);
                    }
                }

                // Out List Scenes
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Project (Excluded)", EditorStyles.boldLabel);
                    using (var scroll = new EditorGUILayout.ScrollViewScope(_outListScroll, EditorStyles.helpBox))
                    {
                        _outListScroll = scroll.scrollPosition;
                        DrawSceneButtons(_outListScenes);
                    }
                }
            }
        }

        private void DrawSceneButtons(string[] sceneNames)
        {
            if (sceneNames == null || sceneNames.Length == 0)
            {
                EditorGUILayout.LabelField("No scenes found.");
                return;
            }

            foreach (var sceneName in sceneNames)
            {
                if (GUILayout.Button(sceneName, GUILayout.Height(25)))
                {
                    LoadSceneByName(sceneName);
                }
            }
        }

        private void LoadSceneByName(string sceneName)
        {
            // Assets/Level/Scenes 以下からシーン名を元にパスを検索
            string[] guids = AssetDatabase.FindAssets($"{sceneName} t:Scene", new[] { TargetScenesPath });
            
            // 完全に一致する名前のシーンを探す
            string scenePath = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(path => System.IO.Path.GetFileNameWithoutExtension(path) == sceneName);

            if (!string.IsNullOrEmpty(scenePath))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }
            else
            {
                Debug.LogError($"[UsefulTools] Scene not found in {TargetScenesPath}: {sceneName}");
            }
        }
    }
}