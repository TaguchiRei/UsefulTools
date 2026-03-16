using UnityEngine;
using UnityEditor;

public class AudioImportTab : EditorWindow
{
    private AudioClip _targetClip;
    private string _clipPath;
    private string _fileName;

    public static void ShowWindow(AudioClip clip, string path)
    {
        var window = GetWindow<AudioImportTab>("AudioImportTab");

        window._clipPath = path;

        window._targetClip = clip;

        if (clip != null)
        {
            window._fileName = clip.name;
        }
    }

    private void OnGUI()
    {
        if (_targetClip == null)
        {
            EditorGUILayout.LabelField("AudioClip が設定されていません");
            return;
        }

        EditorGUILayout.LabelField("対象AudioClip", _targetClip.name);

        EditorGUILayout.Space();

        _fileName = EditorGUILayout.TextField("File Name", _fileName);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Apply"))
        {
            ApplyRename();
            Close();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ApplyRename()
    {
        AssetDatabase.RenameAsset(_clipPath, _fileName);
        AssetDatabase.Refresh();
    }
}