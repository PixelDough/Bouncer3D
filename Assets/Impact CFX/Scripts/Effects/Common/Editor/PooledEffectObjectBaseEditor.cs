using UnityEditor;

namespace ImpactCFX.EditorScripts
{
    public abstract class PooledEffectObjectBaseEditor : Editor
    {
        private SerializedProperty objectPoolConfigProperty;
        private SerializedProperty effectAttachModeProperty;
        private SerializedProperty poolIDProperty;

        protected virtual void OnEnable()
        {
            effectAttachModeProperty = serializedObject.FindProperty("effectAttachMode");
            objectPoolConfigProperty = serializedObject.FindProperty("objectPoolConfig");
            poolIDProperty = serializedObject.FindProperty("poolID");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            drawAdditionalProperties();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(objectPoolConfigProperty);
            EditorGUILayout.PropertyField(effectAttachModeProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(poolIDProperty);

            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void drawAdditionalProperties();
    }
}

