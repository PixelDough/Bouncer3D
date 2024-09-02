using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace ImpactCFX
{
    public partial class ImpactSingleMaterialProcessor
    {
        private struct ImpactSingleMaterialJobParameter
        {
            public int MaterialID;
            public ArrayChunk ResultArrayChunk;
        }

        [BurstCompile]
        private struct ImpactSingleMaterialJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<ImpactSingleMaterialJobParameter> Parameters;
            [ReadOnly]
            public NativeParallelHashMap<int, ImpactMaterialData> MaterialDataMap;

            [NativeDisableParallelForRestriction]
            [NativeDisableContainerSafetyRestriction]
            public NativeArray<MaterialCompositionData> Results;

            public void Execute(int index)
            {
                ImpactSingleMaterialJobParameter p = Parameters[index];

                //For single material objects, only the first element in the chunk is set, with a Composition of 1
                MaterialCompositionData m = new MaterialCompositionData();
                m.Composition = 1;

                if (MaterialDataMap.TryGetValue(p.MaterialID, out ImpactMaterialData materialData))
                {
                    m.MaterialData = materialData;
                }
                else
                {
                    m.MaterialData = ImpactMaterialData.Default;
                    ImpactCFXLogger.LogImpactMaterialNotFound(p.MaterialID);
                }

                Results[p.ResultArrayChunk.Offset] = m;

                for (int i = 1; i < p.ResultArrayChunk.Length; i++)
                {
                    Results[p.ResultArrayChunk.Offset + i] = new MaterialCompositionData(new ImpactMaterialData(), 0);
                }
            }
        }
    }
}