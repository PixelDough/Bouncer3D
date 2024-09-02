using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace ImpactCFX
{
    public partial class ImpactTerrainMaterialProcessor
    {
        private struct ImpactTerrainMaterialJobParameter
        {
            public float3 Point;
            public ArrayChunk ResultArrayChunk;
        }

        [BurstCompile]
        private struct ImpactTerrainMaterialJob : IJobParallelFor
        {
            private struct MaterialCompositionBufferComparer : IComparer<MaterialCompositionData>
            {
                public int Compare(MaterialCompositionData x, MaterialCompositionData y)
                {
                    return y.Composition.CompareTo(x.Composition);
                }
            }

            public float3 TerrainPosition;
            public float3 TerrainSize;

            [ReadOnly]
            public NativeArray<float> Alphamaps;
            public int AlphamapResolution;

            [ReadOnly]
            public NativeArray<int> TerrainLayerMaterialIDs;
            public int DefaultMaterialID;

            [ReadOnly]
            public NativeArray<ImpactTerrainMaterialJobParameter> Parameters;
            [ReadOnly]
            public NativeParallelHashMap<int, ImpactMaterialData> MaterialDataMap;

            [NativeDisableParallelForRestriction]
            [NativeDisableContainerSafetyRestriction]
            public NativeArray<MaterialCompositionData> Results;

            public void Execute(int index)
            {
                ImpactTerrainMaterialJobParameter p = Parameters[index];

                int2 alphamapIndices = getAlphamapIndicesAtPoint(p.Point);
                int terrainLayerCount = TerrainLayerMaterialIDs.Length;

                NativeArray<MaterialCompositionData> buffer = new NativeArray<MaterialCompositionData>(terrainLayerCount, Allocator.Temp);

                //Get the composition of all impact materials, combining when needed (since you can have multiple textures mapped to the same impact material)
                for (int i = 0; i < terrainLayerCount; i++)
                {
                    int materialId = TerrainLayerMaterialIDs[i];
                    if (materialId == 0)
                        materialId = DefaultMaterialID;

                    int alphamapIndex = alphamapIndices.y + (alphamapIndices.x * AlphamapResolution) + (i * AlphamapResolution * AlphamapResolution);

                    float comp = Alphamaps[alphamapIndex];

                    //Check if buffer already contains an entry for this material ID
                    //This ensures that terrain layers with the same material are combined instead of being doubled up
                    bool foundExisting = false;
                    for (int j = 0; j < terrainLayerCount; j++)
                    {
                        MaterialCompositionData existingMaterialCompisitionData = buffer[j];

                        if (existingMaterialCompisitionData.MaterialData.MaterialID == materialId)
                        {
                            existingMaterialCompisitionData.Composition += comp;
                            buffer[j] = existingMaterialCompisitionData;

                            foundExisting = true;
                            break;
                        }
                    }

                    //Add new entry
                    if (!foundExisting)
                    {
                        MaterialCompositionData materialCompositionData = new MaterialCompositionData()
                        {
                            MaterialData = new ImpactMaterialData() { MaterialID = materialId },
                            Composition = comp
                        };

                        buffer[i] = materialCompositionData;
                    }
                }

                //Sort composition buffer by composition value descending
                buffer.Sort(new MaterialCompositionBufferComparer());

                //Populate final composition results
                int resultsLength = math.min(TerrainLayerMaterialIDs.Length, p.ResultArrayChunk.Length);
                for (int i = 0; i < resultsLength; i++)
                {
                    MaterialCompositionData materialCompositionData = buffer[i];

                    if (MaterialDataMap.TryGetValue(materialCompositionData.MaterialData.MaterialID, out ImpactMaterialData materialData))
                    {
                        materialCompositionData.MaterialData = materialData;
                    }
                    else
                    {
                        materialCompositionData.MaterialData = ImpactMaterialData.Default;
                        ImpactCFXLogger.LogImpactMaterialNotFound(materialCompositionData.MaterialData.MaterialID);
                    }

                    Results[p.ResultArrayChunk.Offset + i] = materialCompositionData;
                }

                for (int i = resultsLength; i < p.ResultArrayChunk.Length; i++)
                {
                    Results[p.ResultArrayChunk.Offset + i] = new MaterialCompositionData(new ImpactMaterialData(), 0);
                }

                buffer.Dispose();
            }

            private int2 getAlphamapIndicesAtPoint(float3 point)
            {
                int2 v = new int2();

                v.x = (int)(((point.x - TerrainPosition.x) / TerrainSize.x) * AlphamapResolution);
                v.y = (int)(((point.z - TerrainPosition.z) / TerrainSize.z) * AlphamapResolution);

                v.x = math.clamp(v.x, 0, AlphamapResolution - 1);
                v.y = math.clamp(v.y, 0, AlphamapResolution - 1);

                return v;
            }
        }
    }
}