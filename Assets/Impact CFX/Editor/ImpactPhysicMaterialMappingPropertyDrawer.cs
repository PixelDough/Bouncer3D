using UnityEditor;
using UnityEngine;


namespace ImpactCFX.EditorScripts
{
    [CustomPropertyDrawer(typeof(ImpactPhysicMaterialMapping))]
    public class ImpactPhysicMaterialMappingPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const float arrowWidth = 18;
            const float arrowWidthHalf = arrowWidth / 2;

            SerializedProperty physicMaterial = property.FindPropertyRelative("PhysicMaterial");
            SerializedProperty impactMaterial = property.FindPropertyRelative("ImpactMaterial");

            EditorGUI.BeginProperty(position, label, property);

            float totalWidth = position.width;

            position.height = EditorGUIUtility.singleLineHeight;
            position.width = (totalWidth / 2) - arrowWidth;
            EditorGUI.PropertyField(position, physicMaterial, new GUIContent());

            position.x += position.width + arrowWidthHalf;
            position.width = arrowWidth;
            EditorGUI.LabelField(position, ">>");

            position.x += position.width + arrowWidthHalf;
            position.width = (totalWidth / 2) - arrowWidth;
            EditorGUI.PropertyField(position, impactMaterial, new GUIContent());

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
