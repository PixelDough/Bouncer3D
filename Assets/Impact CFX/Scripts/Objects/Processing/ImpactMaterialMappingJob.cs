using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace ImpactCFX
{
    public partial class ImpactMaterialMappingProcessor
    {
        private struct ImpactMaterialMappingJobParameter
        {
            public int PhysicsMaterialID;
            public ArrayChunk ResultArrayChunk;
        }

        [BurstCompile]
        private struct ImpactMaterialMappingJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<ImpactMaterialMappingJobParameter> Parameters;

            [ReadOnly]
            public NativeParallelHashMap<int, int> PhysicsMaterialMap;
            [ReadOnly]
            public NativeParallelHashMap<int, ImpactMaterialData> MaterialDataMap;

            [NativeDisableParallelForRestriction]
            [NativeDisableContainerSafetyRestriction]
            public NativeArray<MaterialCompositionData> Results;

            public void Execute(int index)
            {
                ImpactMaterialMappingJobParameter p = Parameters[index];

                //For single material objects, only the first element in the chunk is set, with a Composition of 1
                MaterialCompositionData m = new MaterialCompositionData();
                m.Composition = 1;

                if (PhysicsMaterialMap.TryGetValue(p.PhysicsMaterialID, out int materialID))
                {
                    if (MaterialDataMap.TryGetValue(materialID, out ImpactMaterialData materialData))
                    {
                        m.MaterialData = materialData;
                    }
                    else
                    {
                        m.MaterialData = ImpactMaterialData.Default;
                    }
                }
                else
                {
                    m.MaterialData = ImpactMaterialData.Default;
                }

                Results[p.ResultArrayChunk.Offset] = m;
            }
        }
    }

}

