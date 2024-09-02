using System.Text;
using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomPropertyDrawer(typeof(ImpactTagMask))]
    public class ImpactTagMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ImpactCFXSettings impactTagSettings = ImpactCFXSettings.instance;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            SerializedProperty valueProperty = property.FindPropertyRelative("Value");
            position.height = EditorGUIUtility.singleLineHeight;

            string selectedTags = getSelectedTags(valueProperty.intValue, impactTagSettings, false);
            string selectedTagsTooltip = getSelectedTags(valueProperty.intValue, impactTagSettings, true);

            if (GUI.Button(position, new GUIContent(selectedTags, selectedTagsTooltip), EditorStyles.layerMaskField))
            {
                ImpactTagSelectionDropdown tagMaskPopup = ScriptableObject.CreateInstance<ImpactTagSelectionDropdown>();

                tagMaskPopup.Initialize(valueProperty, impactTagSettings, true, (int pos, bool selected) =>
                {
                    if (selected)
                        valueProperty.intValue = valueProperty.intValue.SetBit(pos);
                    else
                        valueProperty.intValue = valueProperty.intValue.UnsetBit(pos);

                    property.serializedObject.ApplyModifiedProperties();
                });

                Rect buttonRect = position;
                Vector2 adjustedPosition = EditorGUIUtility.GUIToScreenPoint(buttonRect.position);
                buttonRect.position = adjustedPosition;

                tagMaskPopup.ShowAsDropDown(buttonRect, tagMaskPopup.GetWindowSize(position));
            }

            EditorGUI.EndProperty();
        }

        private string getSelectedTags(int tagMaskValue, ImpactCFXSettings impactTagSettings, bool newline)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < ImpactCFXSettings.TAG_COUNT; i++)
            {
                if (tagMaskValue.IsBitSet(i))
                {
                    if (impactTagSettings.IsTagDefined(i))
                        sb.Append(impactTagSettings[i]);
                    else
                        sb.Append("Undefined");

                    if (newline)
                        sb.Append("\n");
                    else
                        sb.Append(", ");
                }
            }

            if (sb.Length == 0)
                sb.Append("Nothing");
            else
                sb.Remove(sb.Length - (newline ? 1 : 2), newline ? 1 : 2);


            return sb.ToString();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}