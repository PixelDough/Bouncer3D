using UnityEditor;

namespace ImpactCFX.EditorScripts
{
    [CustomEditor(typeof(ImpactObjectSingleMaterial))]
    [CanEditMultipleObjects]
    public class ImpactObjectSingleMaterialEditor : ImpactObjectBaseEditor
    {
        private SerializedProperty impactMaterialProperty;
        private SerializedProperty registerMaterialProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            impactMaterialProperty = serializedObject.FindProperty("impactMaterial");
            registerMaterialProperty = serializedObject.FindProperty("registerMaterialOnStart");
        }

        protected override void drawAdditionalProperties()
        {
            EditorGUILayout.PropertyField(impactMaterialProperty);
            EditorGUILayout.PropertyField(registerMaterialProperty);
        }
    }
}