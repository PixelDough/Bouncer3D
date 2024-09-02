using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    public static class ImpactEditorUtilities
    {
        public static void Separator()
        {
            EditorGUILayout.Space();
            GUILayout.Box("", GUILayout.MaxWidth(Screen.width - 25f), GUILayout.Height(2));
        }
    }
}
