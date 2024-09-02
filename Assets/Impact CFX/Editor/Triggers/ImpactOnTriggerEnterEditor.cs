using UnityEditor;

namespace ImpactCFX.Triggers.EditorScripts
{
    public abstract class ImpactOnTriggerEnterEditor : Editor
    {
        private SerializedProperty triggerEnabledProperty;
        private SerializedProperty impactObjectProperty;
        private SerializedProperty cooldownProperty;

        private bool advancedFoldout;

        private void OnEnable()
        {
            triggerEnabledProperty = serializedObject.FindProperty("triggerEnabled");
            impactObjectProperty = serializedObject.FindProperty("impactObject");
            cooldownProperty = serializedObject.FindProperty("cooldown");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(triggerEnabledProperty);
            EditorGUILayout.PropertyField(impactObjectProperty);

            if (impactObjectProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("An Impact Object must be assigned for Impact On Trigger Enter.", MessageType.Error);
            }

            EditorGUILayout.Separator();

            advancedFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(advancedFoldout, "Advanced");
            if (advancedFoldout)
            {
                EditorGUILayout.PropertyField(cooldownProperty);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(ImpactOnTriggerEnter3D))]
    [CanEditMultipleObjects]
    public class ImpactImpactOnTriggerEnter3DEditor : ImpactOnTriggerEnterEditor { }

    [CustomEditor(typeof(ImpactOnTriggerEnter2D))]
    [CanEditMultipleObjects]
    public class ImpactImpactOnTriggerEnter2DEditor : ImpactOnTriggerEnterEditor { }
}

