using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class ProjectInitTool : EditorWindow
{
    private static AddRequest _request;

    [MenuItem("UsefulTools/ProjectInitTool")]
    public static void ShowWindow()
    {
        GetWindow<ProjectInitTool>("Project Init Tool");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("UniTaskを導入"))
        {
            InstallURL("https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");
        }

        if (GUILayout.Button("VContainerを導入"))
        {
            InstallURL("https://github.com/hadashiA/VContainer.git");
        }

        if (GUILayout.Button("Addressablesを導入"))
        {
            InstallPackageManager("com.unity.addressables");
        }

        if (GUILayout.Button("ディレクトリ整理"))
        {
            DirectoryInit();
        }
    }

    #region ディレクトリ整理

    private void DirectoryInit()
    {
        CreateFolderUnderAssets("Assets", "Art");
        CreateFolderUnderAssets("Assets/Art", "Materials");
        CreateFolderUnderAssets("Assets/Art", "Models");
        CreateFolderUnderAssets("Assets/Art", "Textures");

        CreateFolderUnderAssets("Assets", "Audio");
        CreateFolderUnderAssets("Assets/Audio", "Music");
        CreateFolderUnderAssets("Assets/Audio", "Sound");

        CreateFolderUnderAssets("Assets", "Code");
        CreateFolderUnderAssets("Assets/Code", "Scripts");
        CreateFolderUnderAssets("Assets/Code", "Shaders");
        CreateFolderUnderAssets("Assets/Code", "Editor");
        CreateFolderUnderAssets("Assets/Code", "Attribute");

        CreateFolderUnderAssets("Assets", "Docs");

        CreateFolderUnderAssets("Assets", "Level");
        CreateFolderUnderAssets("Assets/Level", "Prefabs");
        CreateFolderUnderAssets("Assets/Level", "UI");

        CreateFolderUnderAssets("Assets", "Data");
        CreateFolderUnderAssets("Assets/Data", "InputSystemAsset");

        CreateFolderUnderAssets("Assets", "LocalAssets");

        MoveFolderAndAsset("Assets/Scenes", "Assets/Level/Scenes");
        MoveFolderAndAsset("Assets/Settings", "Assets/Art/Settings");
        MoveFolderAndAsset("Assets/TutorialInfo", "Assets/Docs/TutorialInfo");
        MoveFolderAndAsset("Assets/Readme.asset", "Assets/Docs/Readme.asset");
        MoveFolderAndAsset("Assets/InputSystem_Actions.inputactions", "Assets/Data/InputSystemAsset/InputSystem_Actions.inputactions");

        if (!AssetDatabase.IsValidFolder("Assets/Level/Scenes"))
        {
            CreateFolderUnderAssets("Assets/Level", "Scenes");
        }

        AssetDatabase.Refresh();
    }

    private static void CreateFolderUnderAssets(string folderPath, string folderName)
    {
        if (!AssetDatabase.IsValidFolder(folderPath + "/" + folderName))
        {
            AssetDatabase.CreateFolder(folderPath, folderName);
            Debug.Log($"フォルダを作成しました: {folderPath}/{folderName}");
        }
    }

    private static void MoveFolderAndAsset(string oldPath, string newPath)
    {
        if (AssetDatabase.IsValidFolder(oldPath) || AssetDatabase.LoadAssetAtPath<Object>(oldPath) != null)
        {
            if (AssetDatabase.IsValidFolder(newPath) || AssetDatabase.LoadAssetAtPath<Object>(newPath) != null)
            {
                Debug.LogWarning($"移動先が既に存在するため、{oldPath} の移動をスキップしました: {newPath}");
                return;
            }

            string error = AssetDatabase.MoveAsset(oldPath, newPath);

            if (string.IsNullOrEmpty(error))
            {
                Debug.Log($"アセットを移動しました: {oldPath} -> {newPath}");
            }
            else
            {
                Debug.LogError($"アセットの移動に失敗しました: {oldPath} -> {newPath} (Error: {error})");
            }
        }
    }

    #endregion

    #region Asset導入

    private void InstallURL(string url)
    {
        _request = Client.Add(url);
        EditorApplication.update += Progress;
    }

    private void InstallPackageManager(string packageName)
    {
        _request = Client.Add(packageName);
        EditorApplication.update += Progress;
    }

    private static void Progress()
    {
        if (_request == null)
        {
            EditorApplication.update -= Progress;
            return;
        }

        if (_request.IsCompleted)
        {
            if (_request.Status == StatusCode.Success)
            {
                Debug.Log("Packageの導入が完了しました");
            }
            else if (_request.Status >= StatusCode.Failure)
            {
                Debug.LogError($"Package導入失敗: {_request.Error.message}");
            }

            EditorApplication.update -= Progress;
            _request = null;
        }
    }

    #endregion
}