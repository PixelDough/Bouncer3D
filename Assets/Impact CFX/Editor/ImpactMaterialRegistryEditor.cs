using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomEditor(typeof(ImpactMaterialRegistry))]
    public class ImpactMaterialRegistryEditor : Editor
    {
        private SerializedProperty materialsProperty;

        private void OnEnable()
        {
            materialsProperty = serializedObject.FindProperty("Materials");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(materialsProperty);

            EditorGUILayout.Separator();

            if (GUILayout.Button(new GUIContent("Scan For Impact Materials", "Automatically scan for Impact Material assets and populate the Impact Material Registry.")))
            {
                scanForMaterials();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void scanForMaterials()
        {
            string[] materialGUIDs = AssetDatabase.FindAssets("t:ImpactMaterialAuthoring");
            List<ImpactMaterialAuthoring> materials = new List<ImpactMaterialAuthoring>();

            foreach (string guid in materialGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                ImpactMaterialAuthoring impactMaterialAuthoring = AssetDatabase.LoadAssetAtPath<ImpactMaterialAuthoring>(path);
                materials.Add(impactMaterialAuthoring);
            }

            materialsProperty.arraySize = materials.Count;

            for (int i = 0; i < materials.Count; i++)
            {
                SerializedProperty arrayElement = materialsProperty.GetArrayElementAtIndex(i);
                arrayElement.objectReferenceValue = materials[i];
            }
        }
    }
}

