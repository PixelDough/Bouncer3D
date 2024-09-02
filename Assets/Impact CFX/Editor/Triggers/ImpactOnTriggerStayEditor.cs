using UnityEditor;

namespace ImpactCFX.Triggers.EditorScripts
{
    public abstract class ImpactOnTriggerStayEditor : Editor
    {
        private SerializedProperty triggerEnabledProperty;
        private SerializedProperty impactObjectProperty;

        private void OnEnable()
        {
            triggerEnabledProperty = serializedObject.FindProperty("triggerEnabled");
            impactObjectProperty = serializedObject.FindProperty("impactObject");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(triggerEnabledProperty);
            EditorGUILayout.PropertyField(impactObjectProperty);

            if (impactObjectProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("An Impact Object must be assigned for Impact On Trigger Stay.", MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(ImpactOnTriggerStay3D))]
    [CanEditMultipleObjects]
    public class ImpactOnTriggerStay3DEditor : ImpactOnTriggerStayEditor { }

    [CustomEditor(typeof(ImpactOnTriggerStay2D))]
    [CanEditMultipleObjects]
    public class ImpactOnTriggerStay2DEditor : ImpactOnTriggerStayEditor { }
}

