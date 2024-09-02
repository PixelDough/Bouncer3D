using UnityEditor;

namespace ImpactCFX.EditorScripts
{
    [CustomEditor(typeof(ImpactObjectCustomVelocity))]
    [CanEditMultipleObjects]
    public class ImpactObjectCustomVelocityEditor : ImpactObjectSingleMaterialEditor
    {
        private SerializedProperty velocityScaleProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            velocityScaleProperty = serializedObject.FindProperty("velocityScale");
        }

        protected override void drawAdditionalProperties()
        {
            base.drawAdditionalProperties();
            EditorGUILayout.PropertyField(velocityScaleProperty);
        }
    }
}