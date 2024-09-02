using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomPropertyDrawer(typeof(ImpactTagMaskFilter))]
    public class ImpactTagFilterPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.x -= 15;
            position.width += 15;

            SerializedProperty tagMaskProperty = property.FindPropertyRelative("TagMask");
            SerializedProperty exactProperty = property.FindPropertyRelative("Exact");

            position.width -= 65;
            EditorGUI.PropertyField(position, tagMaskProperty, new GUIContent());

            position.x = position.max.x - 10;
            position.width = 65;
            exactProperty.boolValue = EditorGUI.ToggleLeft(position, new GUIContent("Exact", "Should the tags exactly match the specified tags?"), exactProperty.boolValue);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}