using ImpactCFX.Audio.EditorScripts;
using ImpactCFX.EditorScripts;
using UnityEditor;
using UnityEngine;

namespace ImpactCFX.Particles.EditorScripts
{
    [CustomEditor(typeof(ImpactParticleEffectAuthoring))]
    public class ImpactParticleEffectAuthoringEditor : ImpactEffectAuthoringBaseEditor
    {
        private SerializedProperty minimumVelocityProperty;
        private SerializedProperty collisionNormalInfluenceProperty;

        private SerializedProperty particlePrefabProperty;
        private SerializedProperty particleEffectTypeProperty;

        private SerializedProperty emitOnSlideProperty;
        private SerializedProperty emitOnRollProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            minimumVelocityProperty = serializedObject.FindProperty("minimumVelocity");
            collisionNormalInfluenceProperty = serializedObject.FindProperty("collisionNormalInfluence");

            particlePrefabProperty = serializedObject.FindProperty("particlePrefab");
            particleEffectTypeProperty = serializedObject.FindProperty("particleEffectType");

            emitOnSlideProperty = serializedObject.FindProperty("emitOnSlide");
            emitOnRollProperty = serializedObject.FindProperty("emitOnRoll");
        }

        protected override void drawEffectProperties()
        {
            EditorGUILayout.PropertyField(particlePrefabProperty);
            EditorGUILayout.PropertyField(particleEffectTypeProperty);

            ParticleEffectType particleEffectType = (ParticleEffectType)particleEffectTypeProperty.enumValueIndex;

            GUI.enabled = particleEffectType == ParticleEffectType.Looped;

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(emitOnSlideProperty);
            EditorGUILayout.PropertyField(emitOnRollProperty);

            EditorGUI.indentLevel--;

            GUI.enabled = true;

            ImpactEditorUtilities.Separator();

            EditorGUILayout.PropertyField(minimumVelocityProperty);
            EditorGUILayout.Slider(collisionNormalInfluenceProperty, 0, 1);
        }
    }
}