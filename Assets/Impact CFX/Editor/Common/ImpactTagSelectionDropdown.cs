using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    public class ImpactTagSelectionDropdown : EditorWindow
    {
        private const int maxTagEntries = 10;

        private ImpactCFXSettings tagSettings;
        private SerializedProperty tagValueProperty;
        private bool isMask;

        private string search;
        private SearchField searchField;
        private Vector2 scrollPos;

        private System.Action<int, bool> tagSelectedChangedCallback;

        public void Initialize(SerializedProperty tagMaskValueProperty, ImpactCFXSettings tagSettings, bool isMask, System.Action<int, bool> tagSelectedChangedCallback)
        {
            this.isMask = isMask;
            this.tagValueProperty = tagMaskValueProperty;
            this.tagSettings = tagSettings;
            this.tagSelectedChangedCallback = tagSelectedChangedCallback;

            search = "";
            searchField = new SearchField();
        }

        public Vector2 GetWindowSize(Rect controlRect)
        {
            int h = Mathf.Min(tagSettings.GetDefinedTagCount(), maxTagEntries);

            float height = EditorGUIUtility.singleLineHeight * h + EditorGUIUtility.singleLineHeight + 10;
            return new Vector2(controlRect.width, height);
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            search = searchField.OnGUI(search);
            string searchLower = search.ToLower();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            int tagValue = tagValueProperty.intValue;

            for (int i = 0; i < ImpactCFXSettings.TAG_COUNT; i++)
            {
                bool isTagNameDefined = tagSettings.IsTagDefined(i);

                if (isTagNameDefined)
                {
                    string tagName = tagSettings[i];
                    string tagNameLower = tagName.ToLower();

                    bool boolValue = isMask ? tagValue.IsBitSet(i) : tagValue == i;
                    bool show = isTagNameDefined || boolValue;

                    if (search.Length > 0)
                        show &= searchLower.Contains(tagNameLower) || tagNameLower.Contains(searchLower);

                    if (show)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.BeginHorizontal();

                        bool toggleValue = GUILayout.Toggle(boolValue, new GUIContent(i + " : " + tagName));

                        if (!isTagNameDefined)
                        {
                            GUILayout.FlexibleSpace();
                            GUIContent warning = EditorGUIUtility.IconContent("console.warnicon.sml");
                            warning.tooltip = "Undefined tag.";
                            GUILayout.Label(warning, GUILayout.Width(20));
                        }

                        EditorGUILayout.EndHorizontal();

                        if (EditorGUI.EndChangeCheck())
                            tagSelectedChangedCallback.Invoke(i, toggleValue);
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }
    }
}
