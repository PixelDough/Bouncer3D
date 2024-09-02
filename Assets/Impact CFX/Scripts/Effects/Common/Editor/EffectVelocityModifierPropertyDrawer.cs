using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomPropertyDrawer(typeof(EffectVelocityModifier))]
    public class EffectVelocityModifierPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty enabledProperty = property.FindPropertyRelative("Enabled");
            SerializedProperty velocityRangeProperty = property.FindPropertyRelative("VelocityRange");
            SerializedProperty curveProperty = property.FindPropertyRelative("Curve");

            position.height = EditorGUIUtility.singleLineHeight;
            enabledProperty.boolValue = EditorGUI.ToggleLeft(position, new GUIContent(property.displayName, property.tooltip), enabledProperty.boolValue);


            if (enabledProperty.boolValue)
            {
                EditorGUI.indentLevel++;

                position.y += EditorGUIUtility.singleLineHeight + 1;
                EditorGUI.PropertyField(position, velocityRangeProperty);

                position.y += EditorGUIUtility.singleLineHeight + 1;
                EditorGUI.PropertyField(position, curveProperty);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty enabledProperty = property.FindPropertyRelative("Enabled");

            if (enabledProperty.boolValue)
                return (EditorGUIUtility.singleLineHeight + 1) * 3;
            else
                return EditorGUIUtility.singleLineHeight;
        }

    }
}
