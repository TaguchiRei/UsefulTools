using System;
using UnityEditor;
using UnityEngine;

public class AudioImportWatcher : AssetPostprocessor
{
    private void OnPostprocessAudio(AudioClip clip)
    {
        AudioImportTab.ShowWindow(clip, assetPath);
    }
}