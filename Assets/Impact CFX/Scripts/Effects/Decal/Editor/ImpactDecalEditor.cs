using ImpactCFX.EditorScripts;
using UnityEditor;

namespace ImpactCFX.Decals.EditorScripts
{
    [CustomEditor(typeof(ImpactDecal))]
    [CanEditMultipleObjects]
    public class ImpactDecalEditor : PooledEffectObjectBaseEditor
    {
        private SerializedProperty decalDistanceProperty;
        private SerializedProperty rotationModeProperty;
        private SerializedProperty axisProperty;

        private SerializedProperty scaleWithVelocityProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            decalDistanceProperty = serializedObject.FindProperty("decalDistance");
            rotationModeProperty = serializedObject.FindProperty("rotationMode");
            axisProperty = serializedObject.FindProperty("axis");

            scaleWithVelocityProperty = serializedObject.FindProperty("scaleWithVelocity");
        }

        protected override void drawAdditionalProperties()
        {
            EditorGUILayout.PropertyField(axisProperty);
            EditorGUILayout.PropertyField(rotationModeProperty);
            EditorGUILayout.PropertyField(decalDistanceProperty);

            EditorGUILayout.Separator();

            drawModifiers();
        }

        protected virtual void drawModifiers()
        {
            EditorGUILayout.PropertyField(scaleWithVelocityProperty);
        }
    }
}

