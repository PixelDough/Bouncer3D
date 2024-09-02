using ImpactCFX.EditorScripts;
using UnityEditor;

namespace ImpactCFX.Audio.EditorScripts
{
    [CustomEditor(typeof(ImpactAudioSourceBase))]
    [CanEditMultipleObjects]
    public class ImpactAudioSourceBaseEditor : PooledEffectObjectBaseEditor
    {
        private SerializedProperty audioFadeOutProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            audioFadeOutProperty = serializedObject.FindProperty("audioFadeOutTime");
        }

        protected override void drawAdditionalProperties()
        {
            EditorGUILayout.PropertyField(audioFadeOutProperty);
        }
    }
}

