using UnityEditor;

namespace ImpactCFX.Audio.EditorScripts
{
    [CustomEditor(typeof(ImpactAudioSource))]
    [CanEditMultipleObjects]
    public class ImpactAudioSourceEditor : ImpactAudioSourceBaseEditor
    {
        private SerializedProperty audioSourceProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            audioSourceProperty = serializedObject.FindProperty("audioSource");
        }

        protected override void drawAdditionalProperties()
        {
            EditorGUILayout.PropertyField(audioSourceProperty);
            base.drawAdditionalProperties();
        }
    }
}

