using ImpactCFX.Audio.EditorScripts;
using ImpactCFX.EditorScripts;
using UnityEditor;
using UnityEngine;

namespace ImpactCFX.Decals.EditorScripts
{
    [CustomEditor(typeof(ImpactDecalEffectAuthoring))]
    public class ImpactDecalEffectAuthoringEditor : ImpactEffectAuthoringBaseEditor
    {
        private SerializedProperty minimumVelocityProperty;
        private SerializedProperty collisionNormalInfluenceProperty;

        private SerializedProperty decalPrefabProperty;

        private SerializedProperty createOnCollisionProperty;
        private SerializedProperty createOnSlideProperty;
        private SerializedProperty createOnRollProperty;

        private SerializedProperty creationIntervalProperty;
        private SerializedProperty creationIntervalTypeProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            minimumVelocityProperty = serializedObject.FindProperty("minimumVelocity");
            collisionNormalInfluenceProperty = serializedObject.FindProperty("collisionNormalInfluence");

            decalPrefabProperty = serializedObject.FindProperty("decalPrefab");

            createOnCollisionProperty = serializedObject.FindProperty("createOnCollision");
            createOnSlideProperty = serializedObject.FindProperty("createOnSlide");
            createOnRollProperty = serializedObject.FindProperty("createOnRoll");

            creationIntervalProperty = serializedObject.FindProperty("creationInterval");
            creationIntervalTypeProperty = serializedObject.FindProperty("creationIntervalType");
        }

        protected override void drawEffectProperties()
        {
            EditorGUILayout.PropertyField(decalPrefabProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(createOnCollisionProperty);
            EditorGUILayout.PropertyField(createOnSlideProperty);
            EditorGUILayout.PropertyField(createOnRollProperty);

            EditorGUILayout.Separator();

            bool slideOrRoll = createOnSlideProperty.boolValue || createOnRollProperty.boolValue;

            GUI.enabled = slideOrRoll;

            EditorGUILayout.PropertyField(creationIntervalProperty);
            EditorGUILayout.PropertyField(creationIntervalTypeProperty);

            GUI.enabled = true;

            ImpactEditorUtilities.Separator();

            EditorGUILayout.PropertyField(minimumVelocityProperty);
            EditorGUILayout.Slider(collisionNormalInfluenceProperty, 0, 1);
        }

    }
}