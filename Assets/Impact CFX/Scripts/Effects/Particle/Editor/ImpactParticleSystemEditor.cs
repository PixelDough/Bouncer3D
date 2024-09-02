using ImpactCFX.EditorScripts;
using UnityEditor;

namespace ImpactCFX.Particles.EditorScripts
{
    [CustomEditor(typeof(ImpactParticleSystem))]
    [CanEditMultipleObjects]
    public class ImpactParticleSystemEditor : PooledEffectObjectBaseEditor
    {
        private SerializedProperty particleSystemsProperty;
        private SerializedProperty rotationModeProperty;
        private SerializedProperty axisProperty;

        private SerializedProperty scaleSizeWithVelocityProperty;
        private SerializedProperty scaleSpeedWithVelocityProperty;
        private SerializedProperty scaleLifetimeWithVelocityProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            particleSystemsProperty = serializedObject.FindProperty("particleSystems");
            rotationModeProperty = serializedObject.FindProperty("rotationMode");
            axisProperty = serializedObject.FindProperty("axis");

            scaleSizeWithVelocityProperty = serializedObject.FindProperty("scaleSizeWithVelocity");
            scaleSpeedWithVelocityProperty = serializedObject.FindProperty("scaleSpeedWithVelocity");
            scaleLifetimeWithVelocityProperty = serializedObject.FindProperty("scaleLifetimeWithVelocity");
        }

        protected override void drawAdditionalProperties()
        {
            EditorGUILayout.PropertyField(particleSystemsProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(axisProperty);
            EditorGUILayout.PropertyField(rotationModeProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(scaleSizeWithVelocityProperty);
            EditorGUILayout.PropertyField(scaleSpeedWithVelocityProperty);
            EditorGUILayout.PropertyField(scaleLifetimeWithVelocityProperty);
        }

    }
}

