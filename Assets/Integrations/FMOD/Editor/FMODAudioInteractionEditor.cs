using Impact.EditorScripts;
using UnityEditor;
using UnityEngine;

namespace Impact.Integration.FMOD.EditorScripts
{
    [CustomEditor(typeof(FMODAudioInteraction))]
    public class FMODAudioInteractionEditor : Editor
    {
        private readonly Color warningColor = new Color(1, 1, 0.5f);

        private FMODAudioInteraction interaction;

        private void OnEnable()
        {
            interaction = target as FMODAudioInteraction;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            drawInteractionProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(interaction);
            }
        }

        private void drawInteractionProperties()
        {
            EditorGUILayout.LabelField("FMOD Properties", EditorStyles.boldLabel);

            SerializedProperty collisionEvent = serializedObject.FindProperty("_collisionEvent");
            EditorGUILayout.PropertyField(collisionEvent, new GUIContent("Collision Event"));

            SerializedProperty slideEvent = serializedObject.FindProperty("_slideEvent");
            EditorGUILayout.PropertyField(slideEvent, new GUIContent("Slide Event"));

            SerializedProperty rollEvent = serializedObject.FindProperty("_rollEvent");
            EditorGUILayout.PropertyField(rollEvent, new GUIContent("Roll Event"));

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Interaction Properties", EditorStyles.boldLabel);
            interaction.VelocityRange = ImpactEditorUtilities.RangeEditor(interaction.VelocityRange, new GUIContent("Velocity Range (Min/Max)", "The velocity magnitude range to use when calculating collision intensity."));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Collision Normal Influence", "How much the normal should affect the intensity."), GUILayout.Width(180));
            interaction.CollisionNormalInfluence = EditorGUILayout.Slider("", interaction.CollisionNormalInfluence, 0, 1);
            EditorGUILayout.EndHorizontal();
        }
    }
}