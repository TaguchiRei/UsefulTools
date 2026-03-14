using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DebugGUI : MonoBehaviour
{
#if UNITY_EDITOR
    private Vector2 Position => new Vector2(UnityEditor.EditorPrefs.GetFloat("UsefulTools.Debug.PosX", 10f), UnityEditor.EditorPrefs.GetFloat("UsefulTools.Debug.PosY", 10f));
    private int FontSize => UnityEditor.EditorPrefs.GetInt("UsefulTools.Debug.FontSize", 20);
    private int FPSSampling => UnityEditor.EditorPrefs.GetInt("UsefulTools.Debug.FPSSampling", 10);
    private bool RemoveMissingReferences => UnityEditor.EditorPrefs.GetBool("UsefulTools.Debug.RemoveMissing", true);
#endif

    private readonly List<(string, Func<string>)> _getValueFunc = new();
    private readonly List<(string, Func<string>)> _setValueFunc = new();
    private readonly List<float> _averageFps = new();
    private readonly List<int> _notFindIndexes = new();

    private static DebugGUI _instance;

    private GUIStyle _debugStyle;
    private GUIStyle _errorStyle;
    private Rect _rect;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (Event.current.type == EventType.Layout)
        {
            _debugStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = FontSize
            };

            _errorStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = FontSize,
                normal =
                {
                    textColor = Color.red
                }
            };
        }

        if (!Mathf.Approximately(_rect.width, Screen.width) || !Mathf.Approximately(_rect.height, Screen.height))
        {
            var pos = Position;
            _rect = new Rect(pos.x, pos.y, Screen.width, Screen.height);
        }

        GUILayout.BeginVertical();
        GUI.BeginGroup(_rect);


        GUILayout.Label($"FPS : {(1.0f / Time.deltaTime):000.0}", _debugStyle);
        GUILayout.Label($"Average FPS : {GetAverageFPS():000.0}", _debugStyle);

        int index = 0;
        foreach (var refAndName in _getValueFunc)
        {
            try
            {
                GUILayout.Label($"{refAndName.Item1} : {refAndName.Item2.Invoke()}", _debugStyle);
            }
            catch
            {
                GUILayout.Label($"{refAndName.Item1} : 値が見つかりません。インスタンスはすでに破棄された可能性があります", _errorStyle);

                if (RemoveMissingReferences)
                {
                    _notFindIndexes.Add(index);
                }
            }

            index++;
        }

        if (RemoveMissingReferences)
        {
            _notFindIndexes.Sort();

            _notFindIndexes.Reverse();

            foreach (var t in _notFindIndexes)
            {
                _getValueFunc.RemoveAt(t);
            }

            _notFindIndexes.Clear();
        }

        GUI.EndGroup();
        GUILayout.EndVertical();
    }

    private void Update()
    {
        _averageFps.Add(Time.deltaTime);
        if (_averageFps.Count > FPSSampling)
        {
            _averageFps.RemoveAt(0);
        }
    }
#endif

    [Conditional("UNITY_EDITOR")]
    public static void ObserveVariable(string name, Func<string> debugValue)
    {
#if UNITY_EDITOR
        if (_instance == null)
        {
            Debug.LogWarning("DebugGUIの初期化前に登録メソッドが呼ばれました");
            return;
        }

        _instance._getValueFunc.Add((name, debugValue));
#endif
    }

    public static void Log(string message)
    {
#if UNITY_EDITOR
        if (_instance == null)
        {
            Debug.LogWarning("DebugGUIの初期化前に登録メソッドが呼ばれました");
            return;
        }
        
        
#endif
    }

    private float GetAverageFPS()
    {
        if (_averageFps.Count == 0) return 0f;
        float average = 0;
        foreach (var delta in _averageFps)
        {
            average += delta;
        }

        average /= _averageFps.Count;
        return 1f / average;
    }
}