using UnityEditor;

namespace ImpactCFX.Triggers.EditorScripts
{
    public abstract class ImpactSlideAndRollTriggerEditor : ImpactTriggerBaseEditor
    {
        protected SerializedProperty enableSlidingProperty;
        protected SerializedProperty enableRollingProperty;
        protected SerializedProperty slideVelocityMethodProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            enableSlidingProperty = serializedObject.FindProperty("enableSliding");
            enableRollingProperty = serializedObject.FindProperty("enableRolling");
            slideVelocityMethodProperty = serializedObject.FindProperty("slideVelocityMethod");
        }

        protected override void drawBasicProperties()
        {
            base.drawBasicProperties();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(enableSlidingProperty);
            EditorGUILayout.PropertyField(enableRollingProperty);
        }

        protected override void drawAdvancedProperties()
        {
            base.drawAdvancedProperties();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(slideVelocityMethodProperty);
        }
    }

    [CustomEditor(typeof(ImpactSlideAndRollTrigger3D))]
    [CanEditMultipleObjects]
    public class ImpactSlideAndRollTrigger3DEditor : ImpactSlideAndRollTriggerEditor { }

    [CustomEditor(typeof(ImpactSlideAndRollTrigger2D))]
    [CanEditMultipleObjects]
    public class ImpactSlideAndRollTrigger2DEditor : ImpactSlideAndRollTriggerEditor { }
}

