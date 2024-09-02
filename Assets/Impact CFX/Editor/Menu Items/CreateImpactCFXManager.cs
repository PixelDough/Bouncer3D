using ImpactCFX.Audio;
using ImpactCFX.Decals;
using ImpactCFX.Particles;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    public class CreateImpactCFXManager
    {
        [MenuItem("GameObject/Impact CFX/Create Impact CFX Manager")]
        public static ImpactCFXManager Create()
        {
            //Create Root
            GameObject impactCFXManagerObject = new GameObject("Impact CFX Manager");
            ImpactCFXManager impactCFXManager = impactCFXManagerObject.AddComponent<ImpactCFXManager>();
            SerializedObject impactCFXManagerSerializedObject = new SerializedObject(impactCFXManager);

            //Create Audio Effect Processor
            ImpactAudioEffectProcessor audioEffectProcessor = createImpactCFXManagerSubProcessor<ImpactAudioEffectProcessor>("Audio Effect Processor", impactCFXManagerObject.transform);

            //Create Particle Effect Processor
            ImpactParticleEffectProcessor particleEffectProcessor = createImpactCFXManagerSubProcessor<ImpactParticleEffectProcessor>("Particle Effect Processor", impactCFXManagerObject.transform);

            //Create Decal Effect Processor
            ImpactDecalEffectProcessor decalEffectProcessor = createImpactCFXManagerSubProcessor<ImpactDecalEffectProcessor>("Decal Effect Processor", impactCFXManagerObject.transform);

            //Assign effect processors
            SerializedProperty effectProcessorsProperty = impactCFXManagerSerializedObject.FindProperty("impactEffectProcessors");
            effectProcessorsProperty.arraySize = 3;

            effectProcessorsProperty.GetArrayElementAtIndex(0).objectReferenceValue = audioEffectProcessor;
            effectProcessorsProperty.GetArrayElementAtIndex(1).objectReferenceValue = particleEffectProcessor;
            effectProcessorsProperty.GetArrayElementAtIndex(2).objectReferenceValue = decalEffectProcessor;

            //Create Single Material Processor
            ImpactSingleMaterialProcessor singleMaterialProcessor = createImpactCFXManagerSubProcessor<ImpactSingleMaterialProcessor>("Single Material Processor", impactCFXManagerObject.transform);

            //Create Terrain Material Processor
            ImpactTerrainMaterialProcessor terrainMaterialProcessor = createImpactCFXManagerSubProcessor<ImpactTerrainMaterialProcessor>("Terrain Material Processor", impactCFXManagerObject.transform);

            //Assign material processors
            SerializedProperty materialProcessorsProperty = impactCFXManagerSerializedObject.FindProperty("impactMaterialProcessors");
            materialProcessorsProperty.arraySize = 2;

            materialProcessorsProperty.GetArrayElementAtIndex(0).objectReferenceValue = singleMaterialProcessor;
            materialProcessorsProperty.GetArrayElementAtIndex(1).objectReferenceValue = terrainMaterialProcessor;

            //Create Material Mapping Processor
            ImpactMaterialMappingProcessor materialMappingProcessor = impactCFXManagerObject.AddComponent<ImpactMaterialMappingProcessor>();

            //Assign Material Mapping Processor
            SerializedProperty materialMappingProperty = impactCFXManagerSerializedObject.FindProperty("materialMapping");
            materialMappingProperty.objectReferenceValue = materialMappingProcessor;

            string[] materialRegistryGUIDs = AssetDatabase.FindAssets("t:ImpactMaterialRegistry");
            if (materialRegistryGUIDs.Length == 0)
            {
                if (EditorUtility.DisplayDialog("Create Impact Material Registry", "No Impact Material Registries exist in your project. Would you like to create one now?", "Yes", "No"))
                {
                    ImpactMaterialRegistry impactMaterialRegistry = ScriptableObject.CreateInstance<ImpactMaterialRegistry>();
                    string savePath = EditorUtility.SaveFilePanel("Save Impact Material Registry", Application.dataPath, "New Impact Material Registry", "asset");

                    if (string.IsNullOrEmpty(savePath))
                    {
                        Object.DestroyImmediate(impactMaterialRegistry);
                    }
                    else
                    {
                        string assetsPath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, savePath));
                        AssetDatabase.CreateAsset(impactMaterialRegistry, assetsPath);
                        AssetDatabase.SaveAssets();

                        impactCFXManagerSerializedObject.FindProperty("materialRegistry").objectReferenceValue = impactMaterialRegistry;
                    }
                }
            }
            else
            {
                string materialRegistryPath = AssetDatabase.GUIDToAssetPath(materialRegistryGUIDs[0]);
                impactCFXManagerSerializedObject.FindProperty("materialRegistry").objectReferenceValue = AssetDatabase.LoadAssetAtPath<ImpactMaterialRegistry>(materialRegistryPath);
            }

            impactCFXManagerSerializedObject.ApplyModifiedProperties();
            impactCFXManagerSerializedObject.Dispose();

            Selection.activeObject = impactCFXManager;
            EditorGUIUtility.PingObject(impactCFXManager);

            return impactCFXManager;
        }

        private static T createImpactCFXManagerSubProcessor<T>(string name, Transform parent) where T : Component
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent);
            T component = gameObject.AddComponent<T>();

            return component;
        }
    }
}

