using ImpactCFX.EditorScripts;
using UnityEditor;

namespace ImpactCFX.Audio.EditorScripts
{
    [CustomEditor(typeof(ImpactAudioEffectAuthoring))]
    public class ImpactAudioEffectAuthoringEditor : ImpactEffectAuthoringBaseEditor
    {
        private SerializedProperty collisionAudioModeProperty;
        private SerializedProperty collisionAudioClipsProperty;
        private SerializedProperty slideAudioProperty;
        private SerializedProperty rollAudioProperty;
        private SerializedProperty audioSourceTemplateProperty;

        private SerializedProperty velocityReferenceRangeProperty;
        private SerializedProperty collisionNormalInfluenceProperty;
        private SerializedProperty scaleVolumeWithVelocityProperty;
        private SerializedProperty randomPitchRangeProperty;
        private SerializedProperty randomVolumeRangeProperty;
        private SerializedProperty velocityPitchInfluenceProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            velocityReferenceRangeProperty = serializedObject.FindProperty("velocityReferenceRange");
            randomPitchRangeProperty = serializedObject.FindProperty("randomPitchRange");
            randomVolumeRangeProperty = serializedObject.FindProperty("randomVolumeRange");
            scaleVolumeWithVelocityProperty = serializedObject.FindProperty("scaleVolumeWithVelocity");
            collisionNormalInfluenceProperty = serializedObject.FindProperty("collisionNormalInfluence");
            velocityPitchInfluenceProperty = serializedObject.FindProperty("velocityPitchInfluence");

            collisionAudioModeProperty = serializedObject.FindProperty("collisionClipSelectionMode");
            collisionAudioClipsProperty = serializedObject.FindProperty("collisionAudioClips");
            slideAudioProperty = serializedObject.FindProperty("slideAudioClip");
            rollAudioProperty = serializedObject.FindProperty("rollAudioClip");
            audioSourceTemplateProperty = serializedObject.FindProperty("audioSourceTemplate");
        }

        protected override void drawEffectProperties()
        {
            EditorGUILayout.PropertyField(collisionAudioClipsProperty);
            EditorGUILayout.PropertyField(collisionAudioModeProperty);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(slideAudioProperty);
            EditorGUILayout.PropertyField(rollAudioProperty);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(audioSourceTemplateProperty);

            if (audioSourceTemplateProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("You must assign an Audio Source Template for sounds to play for this effect.", MessageType.Error);
            }

            ImpactEditorUtilities.Separator();

            EditorGUILayout.PropertyField(velocityReferenceRangeProperty);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(scaleVolumeWithVelocityProperty);

            EditorGUILayout.Slider(collisionNormalInfluenceProperty, 0, 1);
            EditorGUILayout.Slider(velocityPitchInfluenceProperty, 0, 0.1f);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(randomPitchRangeProperty);
            EditorGUILayout.PropertyField(randomVolumeRangeProperty);
        }
    }
}