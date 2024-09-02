using UnityEditor;

namespace ImpactCFX.Triggers.EditorScripts
{
    public abstract class ImpactCollisionTriggerEditor : ImpactTriggerBaseEditor
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
            base.drawAdvancedProperties();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(collisionVelocityMethodProperty);
            EditorGUILayout.PropertyField(cooldownProperty);
        }
    }

    [CustomEditor(typeof(ImpactCollisionTrigger3D))]
    [CanEditMultipleObjects]
    public class ImpactCollisionTrigger3DEditor : ImpactCollisionTriggerEditor { }

    [CustomEditor(typeof(ImpactCollisionTrigger2D))]
    [CanEditMultipleObjects]
    public class ImpactCollisionTrigger2DEditor : ImpactCollisionTriggerEditor { }
}

