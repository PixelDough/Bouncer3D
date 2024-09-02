using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace ImpactCFX
{
    [BurstCompile]
    public struct ImpactMaterialDataResetJob : IJobParallelFor
    {
        public NativeArray<MaterialCompositionData> MaterialComposition;

        public void Execute(int index)
        {
            MaterialComposition[index] = MaterialCompositionData.Default;
        }
    }
}

