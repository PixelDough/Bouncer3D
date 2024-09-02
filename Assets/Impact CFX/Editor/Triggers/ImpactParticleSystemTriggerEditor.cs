using UnityEditor;
using UnityEngine;

namespace ImpactCFX.Triggers.EditorScripts
{
    [CustomEditor(typeof(ImpactParticleSystemCollisionTrigger))]
    [CanEditMultipleObjects]
    public class ImpactParticleSystemTriggerEditor : Editor
    {
        private SerializedProperty triggerEnabledProperty;
        private SerializedProperty impactObjectProperty;

        private SerializedProperty isParticleSystemProperty;
        private SerializedProperty particlesProperty;

        private SerializedProperty cooldownProperty;
        private SerializedProperty thresholdProperty;

        private bool advancedFoldout;

        private void OnEnable()
        {
            triggerEnabledProperty = serializedObject.FindProperty("triggerEnabled");
            impactObjectProperty = serializedObject.FindProperty("impactObject");

            isParticleSystemProperty = serializedObject.FindProperty("isParticleSystem");
            particlesProperty = serializedObject.FindProperty("particles");

            cooldownProperty = serializedObject.FindProperty("cooldown");
            thresholdProperty = serializedObject.FindProperty("threshold");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            drawBasicProperties();

            EditorGUILayout.Separator();

            advancedFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(advancedFoldout, "Advanced");
            if (advancedFoldout)
            {
                drawAdvancedProperties();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void drawBasicProperties()
        {
            EditorGUILayout.PropertyField(triggerEnabledProperty);
            EditorGUILayout.PropertyField(impactObjectProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(isParticleSystemProperty);
            bool isParticleSystem = isParticleSystemProperty.boolValue;

            GUI.enabled = isParticleSystem;
            EditorGUILayout.PropertyField(particlesProperty);
            GUI.enabled = true;

            if (!isParticleSystem)
            {
                EditorGUILayout.HelpBox("Effects will be triggered when this object collides with other Particle Systems.", MessageType.Info);
            }
            else
            {

                ParticleSystem p = particlesProperty.objectReferenceValue as ParticleSystem;

                if (p == null)
                {
                    EditorGUILayout.HelpBox("You need to assign a Particle System!", MessageType.Error);
                }
                else
                {
                    if (!p.collision.enabled)
                    {
                        EditorGUILayout.HelpBox("Collision is not enabled on the attached Particle System!", MessageType.Error);
                    }
                    else if (!p.collision.sendCollisionMessages)
                    {
                        EditorGUILayout.HelpBox("Send Collision Messages is not enabled on the attached Particle System!", MessageType.Error);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Effects will be triggered when the particles of this object's Particle System collide with other objects.", MessageType.Info);
                    }

                }
            }
        }

        private void drawAdvancedProperties()
        {
            EditorGUILayout.PropertyField(cooldownProperty);
            EditorGUILayout.PropertyField(thresholdProperty);
        }
    }
}

