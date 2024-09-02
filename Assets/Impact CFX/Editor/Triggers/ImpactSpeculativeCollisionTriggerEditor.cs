using UnityEditor;

namespace ImpactCFX.Triggers.EditorScripts
{
    public abstract class ImpactSpeculativeCollisionTriggerEditor : ImpactCollisionTriggerEditor
    {
        private SerializedProperty maxCollisionsPerFrameProperty;
        private SerializedProperty contactPointComparisonThresholdProperty;
        private SerializedProperty contactPointLifetimeProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            maxCollisionsPerFrameProperty = serializedObject.FindProperty("maxCollisionsPerFrame");
            contactPointComparisonThresholdProperty = serializedObject.FindProperty("contactPointComparisonThreshold");
            contactPointLifetimeProperty = serializedObject.FindProperty("contactPointLifetime");
        }

        protected override void drawAdvancedProperties()
        {
            base.drawAdvancedProperties();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(maxCollisionsPerFrameProperty);
            EditorGUILayout.PropertyField(contactPointComparisonThresholdProperty);
            EditorGUILayout.PropertyField(contactPointLifetimeProperty);
        }
    }

    [CustomEditor(typeof(ImpactSpeculativeCollisionTrigger3D))]
    [CanEditMultipleObjects]
    public class ImpactSpeculativeCollisionTrigger3DEditor : ImpactSpeculativeCollisionTriggerEditor { }

    [CustomEditor(typeof(ImpactSpeculativeCollisionTrigger2D))]
    [CanEditMultipleObjects]
    public class ImpactSpeculativeCollisionTrigger2DEditor : ImpactSpeculativeCollisionTriggerEditor { }
}

