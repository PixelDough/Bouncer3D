using UnityEditor;

namespace ImpactCFX.Audio.EditorScripts
{
    [CustomEditor(typeof(ImpactMaterialAuthoring))]
    public class ImpactMaterialAuthoringEditor : Editor
    {
        private SerializedProperty materialTagsProperty;
        private SerializedProperty effectSetsProperty;
        private SerializedProperty fallbackTagsProperty;
        private SerializedProperty materialIDProperty;

        private void OnEnable()
        {
            materialTagsProperty = serializedObject.FindProperty("materialTags");
            effectSetsProperty = serializedObject.FindProperty("effectSets");
            fallbackTagsProperty = serializedObject.FindProperty("fallbackTags");
            materialIDProperty = serializedObject.FindProperty("materialID");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(materialTagsProperty);
            EditorGUILayout.PropertyField(effectSetsProperty);
            EditorGUILayout.PropertyField(fallbackTagsProperty);
            EditorGUILayout.PropertyField(materialIDProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

