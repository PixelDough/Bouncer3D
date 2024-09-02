using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Implementation of an Impact Object specifically for terrains.
    /// </summary>
    [AddComponentMenu("Impact CFX/Objects/Impact Terrain", 0)]
    [DisallowMultipleComponent]
    public class ImpactObjectTerrain : ImpactObjectBase
    {
        [SerializeField]
        [Tooltip("The terrain this object is associated with.")]
        private Terrain terrain;
        [SerializeField]
        private List<ImpactMaterialAuthoring> terrainMaterials = new List<ImpactMaterialAuthoring>();
        [SerializeField]
        [Tooltip("The default Impact Material to use, if one is not defined for a Terrain Layer.")]
        private ImpactMaterialAuthoring defaultMaterial;
        [Tooltip("If checked, this terrain will automatically register it's materials with the Impact CFX Manager on Start.")]
        [SerializeField]
        private bool registerMaterialsOnStart = true;

        private NativeArray<int> terrainLayerMaterialIDs;
        private NativeArray<float> cachedAlphamaps;

        /// <summary>
        /// Does this object have a valid terrain assigned?
        /// </summary>
        public bool HasValidTerrain
        {
            get { return terrain != null && terrain.terrainData != null; }
        }

        /// <summary>
        /// The number of terrain layers the terrain has.
        /// </summary>
        public int TerrainLayerCount
        {
            get
            {
                if (HasValidTerrain)
                    return terrain.terrainData.terrainLayers.Length;
                else
                    return 0;
            }
        }

        /// <summary>
        /// The resolution of the terrain's splatmap/alphamap.
        /// </summary>
        public int AlphamapResolution
        {
            get
            {
                if (HasValidTerrain)
                    return terrain.terrainData.alphamapResolution;
                else
                    return 0;
            }
        }

        /// <summary>
        /// The ID of the default fallback Impact Material, if one is set.
        /// </summary>
        public int DefaultMaterialID
        {
            get
            {
                if (defaultMaterial != null)
                    return defaultMaterial.GetID();
                else
                    return 0;
            }
        }

        /// <summary>
        /// The size of the terrain in world units.
        /// </summary>
        public Vector3 Size
        {
            get
            {
                if (HasValidTerrain)
                    return terrain.terrainData.size;
                else
                    return Vector3.zero;
            }
        }

        private void Awake()
        {
            if (HasValidTerrain)
            {
                RefreshCachedData();
            }
        }

        private void Start()
        {
            if (registerMaterialsOnStart)
            {
                RegisterMaterials();
            }
        }

        private void Reset()
        {
            terrain = GetComponent<Terrain>();
        }

        private void OnDestroy()
        {
            if (cachedAlphamaps.IsCreated)
                cachedAlphamaps.Dispose();
            if (terrainLayerMaterialIDs.IsCreated)
                terrainLayerMaterialIDs.Dispose();
        }

        public override void RegisterMaterials()
        {
            ImpactCFXGlobal.RegisterMaterials(terrainMaterials);
        }

        /// <summary>
        /// Refresh the cached alphamaps and terrain materials. 
        /// You should call this if you ever modify your terrain at runtime.
        /// </summary>
        public void RefreshCachedData()
        {
            if (!HasValidTerrain)
            {
                Debug.LogError($"Cannot refresh cached data for ImpactTerrain {gameObject.name} because it has no TerrainData.");
                return;
            }

            float[,,] alphamaps = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapResolution, terrain.terrainData.alphamapResolution);
            int alphamapSize = terrain.terrainData.alphamapResolution;
            int terrainLayerCount = terrain.terrainData.terrainLayers.Length;

            if (cachedAlphamaps.IsCreated)
                cachedAlphamaps.Dispose();

            cachedAlphamaps = new NativeArray<float>(alphamapSize * alphamapSize * terrainLayerCount, Allocator.Persistent);

            for (int i = 0; i < cachedAlphamaps.Length; i++)
            {
                int x = i % alphamapSize;
                int y = (i / alphamapSize) % alphamapSize;
                int z = i / (alphamapSize * alphamapSize);

                cachedAlphamaps[i] = alphamaps[x, y, z];
            }

            if (terrainLayerMaterialIDs.IsCreated)
                terrainLayerMaterialIDs.Dispose();

            terrainLayerMaterialIDs = new NativeArray<int>(terrainMaterials.Count, Allocator.Persistent);
            for (int i = 0; i < terrainLayerMaterialIDs.Length; i++)
            {
                ImpactMaterialAuthoring material = terrainMaterials[i];

                if (material != null)
                    terrainLayerMaterialIDs[i] = material.GetID();
            }
        }

        public void SyncTerrainLayersAndMaterialsList()
        {
            if (!HasValidTerrain)
            {
                Debug.LogError($"Cannot sync terrain layers and materials for ImpactTerrain {gameObject.name} because it has no TerrainData.");
                return;
            }

            TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;

            int terrainLayerCount = terrainLayers.Length;
            int terrainMaterialsCount = terrainMaterials.Count;
            int diff = terrainLayerCount - terrainMaterialsCount;

            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    terrainMaterials.Add(null);
                }
            }
            else if (diff < 0)
            {
                terrainMaterials.RemoveRange(terrainLayers.Length, -diff);
            }
        }

        public override RigidbodyData GetRigidbodyData()
        {
            return RigidbodyData.Default;
        }

        /// <summary>
        /// Gets the array of terrain layers defined in the terrain data.
        /// </summary>
        public TerrainLayer[] GetTerrainLayers()
        {
            return terrain.terrainData.terrainLayers;
        }

        /// <summary>
        /// Gets a flattened array of the terrain's texture alphamaps/splatmaps.
        /// </summary>
        public NativeArray<float> GetAlphamaps()
        {
            return cachedAlphamaps;
        }

        /// <summary>
        /// Gets the array of impact material IDs assigned to the terrain layers.
        /// </summary>
        public NativeArray<int> GetTerrainLayerMaterialIDs()
        {
            return terrainLayerMaterialIDs;
        }
    }
}