using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomPropertyDrawer(typeof(Range))]
    public class RangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty minProperty = property.FindPropertyRelative("Min");
            SerializedProperty maxProperty = property.FindPropertyRelative("Max");

            float width = position.width;

            position.width = 28;
            EditorGUI.LabelField(position, "Min");

            position.x = position.max.x;
            position.width = (width - 70) / 2;
            EditorGUI.PropertyField(position, minProperty, new GUIContent());

            position.x = position.max.x + 10;
            position.width = 30;
            EditorGUI.LabelField(position, "Max");

            position.x = position.max.x;
            position.width = (width - 70) / 2;
            EditorGUI.PropertyField(position, maxProperty, new GUIContent());

            EditorGUI.EndProperty();

            EditorGUI.indentLevel = indent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
