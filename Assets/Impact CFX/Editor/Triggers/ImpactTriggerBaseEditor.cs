using UnityEditor;

namespace ImpactCFX.Triggers.EditorScripts
{
    public abstract class ImpactTriggerBaseEditor : Editor
    {
        protected SerializedProperty triggerEnabledProperty;
        protected SerializedProperty impactObjectProperty;

        protected SerializedProperty thresholdProperty;
        protected SerializedProperty contactModeProperty;

        protected SerializedProperty thisMaterialCountProperty;
        protected SerializedProperty otherMaterialCountProperty;

        protected SerializedProperty collisionNormalModeProperty;

        protected SerializedProperty ignoreSameParentsProperty;
        protected SerializedProperty rootParentProperty;

        private bool advancedFoldout;

        protected virtual void OnEnable()
        {
            triggerEnabledProperty = serializedObject.FindProperty("triggerEnabled");
            impactObjectProperty = serializedObject.FindProperty("impactObject");
            thresholdProperty = serializedObject.FindProperty("threshold");
            contactModeProperty = serializedObject.FindProperty("contactMode");
            thisMaterialCountProperty = serializedObject.FindProperty("triggerMaterialCount");
            otherMaterialCountProperty = serializedObject.FindProperty("hitMaterialCount");
            collisionNormalModeProperty = serializedObject.FindProperty("collisionNormalMode");
            ignoreSameParentsProperty = serializedObject.FindProperty("ignoreSameParents");
            rootParentProperty = serializedObject.FindProperty("rootParent");
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

        protected virtual void drawBasicProperties()
        {
            EditorGUILayout.PropertyField(triggerEnabledProperty);
            EditorGUILayout.PropertyField(impactObjectProperty);

            if (impactObjectProperty.objectReferenceValue == null)
            {
                showImpactObjectNotAssignedMessage();
            }
            else
            {
                IImpactObject impactObject = impactObjectProperty.objectReferenceValue as IImpactObject;
                ImpactObjectChild[] children = impactObject.GetGameObject().GetComponentsInChildren<ImpactObjectChild>();

                if (children.Length > 0)
                {
                    EditorGUILayout.HelpBox("The Impact Object assigned to this trigger has children. This trigger's Impact Object field should be left empty so that the Impact Materials assigned to the children will be used.", MessageType.Warning);
                }

            }
        }

        protected virtual void showImpactObjectNotAssignedMessage()
        {
            EditorGUILayout.HelpBox("No Impact Object is assigned. An Impact Object will be retrieved from the collision.", MessageType.Info);
        }

        protected virtual void drawAdvancedProperties()
        {
            EditorGUILayout.PropertyField(thresholdProperty);
            EditorGUILayout.PropertyField(contactModeProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(thisMaterialCountProperty);
            EditorGUILayout.PropertyField(otherMaterialCountProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(collisionNormalModeProperty);

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(ignoreSameParentsProperty);
            if (ignoreSameParentsProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(rootParentProperty);
                EditorGUI.indentLevel--;
            }
        }
    }
}

