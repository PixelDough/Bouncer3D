using UnityEditor;

namespace ImpactCFX.FMOD.EditorScripts
{
    [CustomEditor(typeof(ImpactFMODAudioEffectProcessor))]
    public class ImpactFMODAudioEffectProcessorEditor : Editor
    {
        private SerializedProperty queueCapacityProperty;
        private SerializedProperty objectPoolConfigProperty;
        private SerializedProperty effectAttachModeProperty;
        private SerializedProperty audioFadeOutTimeProperty;

        private void OnEnable()
        {
            queueCapacityProperty = serializedObject.FindProperty("queueCapacity");
            objectPoolConfigProperty = serializedObject.FindProperty("proxyObjectPoolConfig");
            effectAttachModeProperty = serializedObject.FindProperty("effectAttachMode");
            audioFadeOutTimeProperty = serializedObject.FindProperty("audioFadeOutTime");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(queueCapacityProperty);

            SerializedProperty capacityOverride = queueCapacityProperty.FindPropertyRelative("Override");
            bool o = capacityOverride.boolValue;

            if (!o)
            {
                SerializedProperty capacityValue = queueCapacityProperty.FindPropertyRelative("Value");
                SerializedProperty poolSizeProperty = objectPoolConfigProperty.FindPropertyRelative("PoolSize");

                capacityValue.intValue = poolSizeProperty.intValue;
                EditorGUILayout.HelpBox("Queue Capacity defaults to the Proxy Object Pool Config Pool Size.", MessageType.Info);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(objectPoolConfigProperty);
            EditorGUILayout.PropertyField(effectAttachModeProperty);
            EditorGUILayout.PropertyField(audioFadeOutTimeProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
