using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    public abstract class ImpactObjectBaseEditor : Editor
    {
        private SerializedProperty priorityProperty;
        private bool childrenFoldout;

        protected virtual void OnEnable()
        {
            priorityProperty = serializedObject.FindProperty("priority");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            drawAdditionalProperties();

            ImpactObjectBase r = target as ImpactObjectBase;
            IEnumerable<ImpactObjectChild> children = r.gameObject.GetComponentsInChildren<ImpactObjectChild>().Where(c => c != r);

            if (children.Count() > 0)
            {
                EditorGUILayout.Separator();

                EditorGUILayout.HelpBox("This Impact Object has children.", MessageType.Info);

                childrenFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(childrenFoldout, new GUIContent("Children"));

                if (childrenFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    GUI.enabled = false;
                    foreach (ImpactObjectChild child in children)
                    {
                        EditorGUILayout.ObjectField(child, typeof(ImpactObjectChild), true);
                    }
                    GUI.enabled = true;

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            EditorGUILayout.PropertyField(priorityProperty);

            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void drawAdditionalProperties();
    }
}