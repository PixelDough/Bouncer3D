using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    public class ImpactMaterialProcessorEditor : Editor
    {
        private SerializedProperty capacityProperty;

        protected virtual void OnEnable()
        {
            capacityProperty = serializedObject.FindProperty("queueCapacity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(capacityProperty);

            SerializedProperty capacityOverride = capacityProperty.FindPropertyRelative("Override");
            bool o = capacityOverride.boolValue;

            if (!o)
            {
                SerializedProperty capacityValue = capacityProperty.FindPropertyRelative("Value");

                Component c = target as Component;
                ImpactCFXManager impactCollisionEffectProcessor = c.GetComponentInParent<ImpactCFXManager>(true);

                capacityValue.intValue = impactCollisionEffectProcessor.MaterialQueueCapacity;
                EditorGUILayout.HelpBox("Queue Capacity defaults to the Material Queue Capacity configured in the Impact CFX Manager.", MessageType.Info);
            }


            extraInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void extraInspectorGUI() { }
    }

    [CustomEditor(typeof(ImpactSingleMaterialProcessor))]
    public class ImpactSingleMaterialProcessorEditor : ImpactMaterialProcessorEditor { }

    [CustomEditor(typeof(ImpactTerrainMaterialProcessor))]
    public class ImpactTerrainMaterialProcessorEditor : ImpactMaterialProcessorEditor { }

    [CustomEditor(typeof(ImpactMaterialMappingProcessor))]
    public class ImpactMaterialMappingProcessorEditor : ImpactMaterialProcessorEditor
    {
        private SerializedProperty physicMaterial3DMappingProperty;
        private SerializedProperty physicsMaterial2DMappingProperty;
        private SerializedProperty registerMaterialsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            physicMaterial3DMappingProperty = serializedObject.FindProperty("physicMaterialMap");
            physicsMaterial2DMappingProperty = serializedObject.FindProperty("physicsMaterial2DMap");
            registerMaterialsProperty = serializedObject.FindProperty("registerMaterialsOnStart");
        }

        protected override void extraInspectorGUI()
        {
            ImpactEditorUtilities.Separator();

            EditorGUILayout.PropertyField(physicMaterial3DMappingProperty);
            EditorGUILayout.PropertyField(physicsMaterial2DMappingProperty);

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(registerMaterialsProperty);
        }
    }
}

