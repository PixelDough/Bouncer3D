using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    public abstract class OverrideValuePropertyDrawerBase : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            SerializedProperty overrideProperty = property.FindPropertyRelative("Override");


            position.width = EditorGUIUtility.labelWidth;
            overrideProperty.boolValue = EditorGUI.ToggleLeft(position, new GUIContent(label.text, label.tooltip + " Click the checkbox to override this value."), overrideProperty.boolValue);

            GUI.enabled = overrideProperty.boolValue;

            position.x += EditorGUIUtility.labelWidth;
            position.width = EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth;
            EditorGUI.PropertyField(position, valueProperty, new GUIContent(""));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

    }

    [CustomPropertyDrawer(typeof(OverrideValueInt))]
    public class OverrideValueIntPropertyDrawer : OverrideValuePropertyDrawerBase
    {

    }
}
