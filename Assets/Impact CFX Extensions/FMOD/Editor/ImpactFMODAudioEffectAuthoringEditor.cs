using ImpactCFX.EditorScripts;
using UnityEditor;

namespace ImpactCFX.FMOD.EditorScripts
{
    [CustomEditor(typeof(ImpactFMODAudioEffectAuthoring))]
    public class ImpactFMODAudioEffectAuthoringEditor : ImpactEffectAuthoringBaseEditor
    {
        private SerializedProperty collisionEventProperty;
        private SerializedProperty slideEventProperty;
        private SerializedProperty rollEventProperty;

        private SerializedProperty velocityReferenceRangeProperty;
        private SerializedProperty collisionNormalInfluenceProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            collisionEventProperty = serializedObject.FindProperty("collisionEvent");
            slideEventProperty = serializedObject.FindProperty("slideEvent");
            rollEventProperty = serializedObject.FindProperty("rollEvent");

            velocityReferenceRangeProperty = serializedObject.FindProperty("velocityReferenceRange");
            collisionNormalInfluenceProperty = serializedObject.FindProperty("collisionNormalInfluence");
        }

        protected override void drawEffectProperties()
        {
            EditorGUILayout.PropertyField(collisionEventProperty);
            EditorGUILayout.PropertyField(slideEventProperty);
            EditorGUILayout.PropertyField(rollEventProperty);

            ImpactEditorUtilities.Separator();

            EditorGUILayout.PropertyField(velocityReferenceRangeProperty);
            EditorGUILayout.Slider(collisionNormalInfluenceProperty, 0, 1);
        }
    }
}

