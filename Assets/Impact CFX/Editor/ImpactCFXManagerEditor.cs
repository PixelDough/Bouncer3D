using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomEditor(typeof(ImpactCFXManager))]
    public class ImpactCFXManagerEditor : Editor
    {
        private SerializedProperty enableCollisionProcessingProperty;

        private SerializedProperty collisionCapacityProperty;
        private SerializedProperty materialCapacityProperty;

        private SerializedProperty impactMaterialProcessorsProperty;
        private SerializedProperty impactEffectProcessorsProperty;
        private SerializedProperty materialMappingProcessorProperty;

        private SerializedProperty queueCollisionLimitProperty;
        private SerializedProperty queueSlideLimitProperty;
        private SerializedProperty queueRollLimitProperty;

        private SerializedProperty materialRegistryProperty;
        private SerializedProperty dontDestroyOnLoadProperty;
        private SerializedProperty setAsActiveInstanceProperty;

        private bool collisionLimitsFoldout;

        private void OnEnable()
        {
            enableCollisionProcessingProperty = serializedObject.FindProperty("enableCollisionProcessing");

            collisionCapacityProperty = serializedObject.FindProperty("collisionQueueCapacity");
            materialCapacityProperty = serializedObject.FindProperty("materialQueueCapacity");

            queueCollisionLimitProperty = serializedObject.FindProperty("queueCollisionLimit");
            queueSlideLimitProperty = serializedObject.FindProperty("queueSlideLimit");
            queueRollLimitProperty = serializedObject.FindProperty("queueRollLimit");

            impactMaterialProcessorsProperty = serializedObject.FindProperty("impactMaterialProcessors");
            impactEffectProcessorsProperty = serializedObject.FindProperty("impactEffectProcessors");
            materialMappingProcessorProperty = serializedObject.FindProperty("materialMapping");

            materialRegistryProperty = serializedObject.FindProperty("materialRegistry");
            dontDestroyOnLoadProperty = serializedObject.FindProperty("dontDestroyOnLoad");
            setAsActiveInstanceProperty = serializedObject.FindProperty("setAsActiveInstance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(enableCollisionProcessingProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(collisionCapacityProperty);
            collisionLimitsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(collisionLimitsFoldout, new GUIContent("Collision Limits", "Optional limits to impose on the different collision types."));

            if (collisionLimitsFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(queueCollisionLimitProperty, new GUIContent("Collision", "Limit the number of Collision type collisions."));
                EditorGUILayout.PropertyField(queueSlideLimitProperty, new GUIContent("Slide", "Limit the number of Slide type collisions."));
                EditorGUILayout.PropertyField(queueRollLimitProperty, new GUIContent("Roll", "Limit the number of Roll type collisions."));

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(materialCapacityProperty);

            int collisionCapacity = collisionCapacityProperty.intValue;
            int materialCapacity = materialCapacityProperty.intValue;

            if (materialCapacity < collisionCapacity * 2)
            {
                EditorGUILayout.HelpBox($"Material Queue Capacity should be at least 2x Collision Queue Capacity (2 x {collisionCapacity} = {collisionCapacity * 2}), so that at least 1 material can be retrieved for both objects involved in a collision.", MessageType.Warning);
            }

            ImpactEditorUtilities.Separator();

            EditorGUILayout.LabelField("Processors", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(impactEffectProcessorsProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(impactMaterialProcessorsProperty);
            EditorGUILayout.PropertyField(materialMappingProcessorProperty);

            ImpactEditorUtilities.Separator();

            EditorGUILayout.PropertyField(materialRegistryProperty);
            EditorGUILayout.PropertyField(dontDestroyOnLoadProperty);
            EditorGUILayout.PropertyField(setAsActiveInstanceProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
