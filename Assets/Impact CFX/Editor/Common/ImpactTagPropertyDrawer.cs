using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomPropertyDrawer(typeof(ImpactTag))]
    public class ImpactTagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ImpactCFXSettings impactTagSettings = ImpactCFXSettings.instance;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            position.height = EditorGUIUtility.singleLineHeight;

            string selectedTagName = "Undefined";
            int tagValue = valueProperty.intValue;

            if (tagValue >= 0 && tagValue < ImpactCFXSettings.TAG_COUNT)
                selectedTagName = impactTagSettings[tagValue];

            if (GUI.Button(position, selectedTagName, EditorStyles.layerMaskField))
            {
                ImpactTagSelectionDropdown tagMaskPopup = ScriptableObject.CreateInstance<ImpactTagSelectionDropdown>();

                tagMaskPopup.Initialize(valueProperty, impactTagSettings, false, (int pos, bool selected) =>
                {
                    valueProperty.intValue = pos;
                    property.serializedObject.ApplyModifiedProperties();
                });

                Rect buttonRect = position;
                Vector2 adjustedPosition = EditorGUIUtility.GUIToScreenPoint(buttonRect.position);
                buttonRect.position = adjustedPosition;

                tagMaskPopup.ShowAsDropDown(buttonRect, tagMaskPopup.GetWindowSize(position));
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}