using UnityEditor;

namespace ImpactCFX.EditorScripts
{
    public abstract class ImpactEffectAuthoringBaseEditor : Editor
    {
        private SerializedProperty effectIDProperty;

        protected virtual void OnEnable()
        {
            effectIDProperty = serializedObject.FindProperty("effectID");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            drawEffectProperties();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(effectIDProperty);

            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void drawEffectProperties();
    }
}