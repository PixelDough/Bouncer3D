using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomPropertyDrawer(typeof(ImpactCFXManager.CollisionTypeLimit))]
    public class CollisionTypeLimitPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty limitProperty = property.FindPropertyRelative("Limit");
            SerializedProperty enabledProperty = property.FindPropertyRelative("Enabled");

            position.width = EditorGUIUtility.labelWidth;
            enabledProperty.boolValue = EditorGUI.ToggleLeft(position, label, enabledProperty.boolValue);

            GUI.enabled = enabledProperty.boolValue;

            position.x += EditorGUIUtility.labelWidth;
            position.width = EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth;
            EditorGUI.PropertyField(position, limitProperty, new GUIContent(""));

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

    }
}
