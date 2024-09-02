using UnityEditor;
using UnityEngine;

namespace ImpactCFX.EditorScripts
{
    [CustomEditor(typeof(ImpactObjectTerrain))]
    public class ImpactTerrainEditor : ImpactObjectBaseEditor
    {
        private ImpactObjectTerrain impactTerrain;
        private TerrainLayer[] terrainLayers;

        private SerializedProperty terrainProperty;
        private SerializedProperty terrainMaterialsProperty;
        private SerializedProperty defaultMaterialProperty;
        private SerializedProperty registerMaterialsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            impactTerrain = target as ImpactObjectTerrain;

            terrainProperty = serializedObject.FindProperty("terrain");
            terrainMaterialsProperty = serializedObject.FindProperty("terrainMaterials");
            defaultMaterialProperty = serializedObject.FindProperty("defaultMaterial");
            registerMaterialsProperty = serializedObject.FindProperty("registerMaterialsOnStart");

            if (impactTerrain.HasValidTerrain)
            {
                impactTerrain.SyncTerrainLayersAndMaterialsList();
                terrainLayers = impactTerrain.GetTerrainLayers();
            }
        }

        protected override void drawAdditionalProperties()
        {
            EditorGUILayout.PropertyField(terrainProperty, new GUIContent("Terrain", "The Terrain this object is associated with."));

            if (!impactTerrain.HasValidTerrain)
            {
                EditorGUILayout.HelpBox("Assign a valid Terrain to begin editing terrain materials.", MessageType.Info);
            }
            else
            {
                ImpactEditorUtilities.Separator();
                EditorGUILayout.LabelField("Terrain Layer Materials", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(defaultMaterialProperty);

                drawTerrainLayersList();

                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(registerMaterialsProperty);
            }
        }

        private void drawTerrainLayersList()
        {
            bool allLayersValid = true;

            if (terrainMaterialsProperty.arraySize > 0)
            {
                terrainMaterialsProperty.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(terrainMaterialsProperty.isExpanded, new GUIContent("Terrain Materials"));

                if (terrainMaterialsProperty.isExpanded)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    for (int i = 0; i < terrainMaterialsProperty.arraySize; i++)
                    {
                        EditorGUILayout.BeginVertical();

                        bool isLayerValid = drawTerrainLayerMaterial(i);
                        allLayersValid = allLayersValid && isLayerValid;

                        GUILayout.Space(4);
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            else
            {
                EditorGUILayout.HelpBox("Assign Terrain Layers to your terrain to associate them with Impact Materials. Click the 'Refresh Terrain Layers' button below refresh the terrain materials list.", MessageType.Info);
            }

            EditorGUILayout.Space();

            if (!allLayersValid)
            {
                EditorGUILayout.HelpBox("Some Terrain Layers are invalid. Use the 'Refresh Terrain Layers' button to re-sync the terrain material data.", MessageType.Error);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Refresh Terrain Layers", "Manually re-sync the stored materials with the terrain layers.")))
            {
                impactTerrain.SyncTerrainLayersAndMaterialsList();
                terrainLayers = impactTerrain.GetTerrainLayers();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
        }

        private bool drawTerrainLayerMaterial(int index)
        {
            EditorGUILayout.BeginHorizontal();
            bool isValid = true;

            if (index >= 0 && index < terrainLayers.Length)
            {
                TerrainLayer layer = terrainLayers[index];

                if (layer != null)
                    GUILayout.Box(layer.diffuseTexture, GUILayout.Height(40), GUILayout.Width(40));
                else
                    GUILayout.Box(new GUIContent(), GUILayout.Height(40), GUILayout.Width(40));

                EditorGUILayout.BeginVertical();

                if (layer != null)
                    EditorGUILayout.LabelField(layer.name);
                else
                    EditorGUILayout.LabelField("Missing Terrain Layer");

                SerializedProperty terrainMaterial = terrainMaterialsProperty.GetArrayElementAtIndex(index);
                EditorGUILayout.PropertyField(terrainMaterial, new GUIContent());

                EditorGUILayout.EndVertical();

            }
            else
            {
                GUILayout.Box(new GUIContent(), GUILayout.Height(40), GUILayout.Width(40));

                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField("Missing Terrain Layer");

                SerializedProperty terrainMaterial = terrainMaterialsProperty.GetArrayElementAtIndex(index);
                EditorGUILayout.PropertyField(terrainMaterial, new GUIContent());

                EditorGUILayout.EndVertical();

                isValid = false;
            }

            EditorGUILayout.EndHorizontal();

            return isValid;
        }
    }
}