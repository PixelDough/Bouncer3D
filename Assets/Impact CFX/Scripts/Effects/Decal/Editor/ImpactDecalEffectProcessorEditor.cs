using UnityEditor;

namespace ImpactCFX.Decals.EditorScripts
{
    [CustomEditor(typeof(ImpactDecalEffectProcessor))]
    public class ImpactDecalEffectProcessorEditor : Editor
    {
        private SerializedProperty capacityProperty;

        private void OnEnable()
        {
            capacityProperty = serializedObject.FindProperty("queueCapacity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(capacityProperty);

            SerializedProperty capacityOverride = capacityProperty.FindPropertyRelative("Override");
            bool o = capacityOverride.boolValue;

            if (!o)
            {
                EditorGUILayout.HelpBox("Queue Capacity is automatically determined from the largest decal pool size.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
