using UnityEditor;

namespace ImpactCFX.Triggers.EditorScripts
{
    public abstract class ImpactSimpleCollisionTriggerEditor : ImpactTriggerBaseEditor
    {
        protected SerializedProperty cooldownProperty;
        protected SerializedProperty collisionVelocityMethodProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            cooldownProperty = serializedObject.FindProperty("cooldown");
            collisionVelocityMethodProperty = serializedObject.FindProperty("collisionVelocityMethod");
        }

        protected override void drawAdvancedProperties()
        {
            EditorGUILayout.PropertyField(collisionVelocityMethodProperty);
            EditorGUILayout.PropertyField(cooldownProperty);
        }

        protected override void showImpactObjectNotAssignedMessage()
        {
            EditorGUILayout.HelpBox("An Impact Object must be assigned for Simple Collision Triggers.", MessageType.Error);
        }
    }

    [CustomEditor(typeof(ImpactSimpleCollisionTrigger3D))]
    [CanEditMultipleObjects]
    public class ImpactSimpleCollisionTrigger3DEditor : ImpactSimpleCollisionTriggerEditor { }

    [CustomEditor(typeof(ImpactSimpleCollisionTrigger2D))]
    [CanEditMultipleObjects]
    public class ImpactSimpleCollisionTrigger2DEditor : ImpactSimpleCollisionTriggerEditor { }
}

