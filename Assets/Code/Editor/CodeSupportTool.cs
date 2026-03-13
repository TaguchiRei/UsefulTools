using UnityEditor;
using UnityEngine;

namespace UsefulTools.Editor
{
    public class CodeSupportTool : SettingPageBase
    {
        public override string Name => "Code Support";

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Code Support Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Code support settings will be added here in the future.", MessageType.Info);
        }
    }
}
