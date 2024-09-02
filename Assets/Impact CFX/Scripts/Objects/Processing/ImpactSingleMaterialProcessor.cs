using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX
{
    [DisallowMultipleComponent]
    public partial class ImpactSingleMaterialProcessor : ImpactMaterialProcessorBase
    {
        [SerializeField]
        [Tooltip("The maximum number of objects that can be queued in a frame for processing.")]
        private OverrideValueInt queueCapacity;

        private NativeArray<ImpactSingleMaterialJobParameter> parameters;
        private int count;

        private void OnEnable()
        {
            if (!queueCapacity.Override)
            {
                ImpactCFXManager impactCollisionEffectProcessor = GetComponentInParent<ImpactCFXManager>();
                queueCapacity.Value = impactCollisionEffectProcessor.MaterialQueueCapacity;
            }

            parameters = new NativeArray<ImpactSingleMaterialJobParameter>(queueCapacity.Value, Allocator.Persistent);
        }

        private void OnDisable()
        {
            parameters.Dispose();
        }

        public override void QueueObject(IImpactObject impactObject, ImpactContactPoint contactPoint, ArrayChunk resultArrayChunk)
        {
            if (count >= queueCapacity.Value)
                return;

            if (impactObject is ImpactObjectSingleMaterial impactObjectSingleMaterial)
            {
                ImpactMaterialAuthoring m = impactObjectSingleMaterial.Material;
                ImpactSingleMaterialJobParameter p = new ImpactSingleMaterialJobParameter();

                if (m != null)
                {
                    p.MaterialID = m.GetID();
                }
                else
                {
                    p.MaterialID = 0;
                }

                p.ResultArrayChunk = resultArrayChunk;
                parameters[count] = p;

                count++;
            }
        }

        public override JobHandle ScheduleProcessorJobs(
            NativeArray<MaterialCompositionData> materialCompositionArray,
            NativeParallelHashMap<int, ImpactMaterialData> materialDataMap,
            JobHandle dependencies)
        {
            ImpactSingleMaterialJob impactSingleMaterialJob = new ImpactSingleMaterialJob()
            {
                Parameters = parameters,
                MaterialDataMap = materialDataMap,
                Results = materialCompositionArray
            };

            return impactSingleMaterialJob.Schedule(count, count / 8, dependencies);
        }

        public override void ResetProcessor()
        {
            count = 0;
        }
    }
}
