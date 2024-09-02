using UnityEditor;

namespace ImpactCFX.Audio.EditorScripts
{
    [CustomEditor(typeof(ImpactAudioEffectProcessor))]
    public class ImpactAudioEffectProcessorEditor : Editor
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
                EditorGUILayout.HelpBox("Queue Capacity is automatically determined from the largest audio source pool size.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
