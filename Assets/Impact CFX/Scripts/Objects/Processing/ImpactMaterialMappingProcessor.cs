using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Impact CFX/Impact Material Mapping Processor")]
    public partial class ImpactMaterialMappingProcessor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The maximum number of materials that can be processed in a frame.")]
        private OverrideValueInt queueCapacity;
        [SerializeField]
        [Tooltip("Mapping from 3D physic materials to impact materials.")]
        private List<ImpactPhysicMaterialMapping> physicMaterialMap = new List<ImpactPhysicMaterialMapping>();
        [SerializeField]
        [Tooltip("Mapping from 2D physics materials to impact materials.")]
        private List<ImpactPhysicsMaterial2DMapping> physicsMaterial2DMap = new List<ImpactPhysicsMaterial2DMapping>();
        [Tooltip("If checked, any materials defined in the material mapping will be registered with the Impact CFX Manager on Start.")]
        [SerializeField]
        private bool registerMaterialsOnStart = true;

        private NativeParallelHashMap<int, int> physicsMaterialMap;

        private NativeArray<ImpactMaterialMappingJobParameter> parameters;
        private int count;

        private void Awake()
        {
            physicsMaterialMap = new NativeParallelHashMap<int, int>(physicMaterialMap.Count + physicsMaterial2DMap.Count, Allocator.Persistent);
        }

        private void Start()
        {
            if (!queueCapacity.Override)
            {
                ImpactCFXManager impactCollisionEffectProcessor = GetComponentInParent<ImpactCFXManager>();
                queueCapacity.Value = impactCollisionEffectProcessor.MaterialQueueCapacity;
            }

            //Create mappings from physics materials to impact material IDs
            foreach (ImpactPhysicMaterialMapping mapping in physicMaterialMap)
            {
                physicsMaterialMap.Add(mapping.PhysicMaterial.GetInstanceID(), mapping.ImpactMaterial.GetID());

                if (registerMaterialsOnStart)
                {
                    ImpactCFXGlobal.RegisterMaterial(mapping.ImpactMaterial);
                }
            }

            foreach (ImpactPhysicsMaterial2DMapping mapping in physicsMaterial2DMap)
            {
                physicsMaterialMap.Add(mapping.PhysicsMaterial2D.GetInstanceID(), mapping.ImpactMaterial.GetID());

                if (registerMaterialsOnStart)
                {
                    ImpactCFXGlobal.RegisterMaterial(mapping.ImpactMaterial);
                }
            }

            parameters = new NativeArray<ImpactMaterialMappingJobParameter>(queueCapacity.Value, Allocator.Persistent);
        }

        private void OnDestroy()
        {
            parameters.Dispose();
            physicsMaterialMap.Dispose();
        }

        public void QueueObject(int physicsMaterialID, ArrayChunk resultArrayChunk)
        {
            ImpactMaterialMappingJobParameter p = new ImpactMaterialMappingJobParameter()
            {
                PhysicsMaterialID = physicsMaterialID,
                ResultArrayChunk = resultArrayChunk
            };

            parameters[count] = p;

            count++;
        }

        public JobHandle ScheduleProcessorJobs(
            NativeArray<MaterialCompositionData> materialCompositionArray,
            NativeParallelHashMap<int, ImpactMaterialData> materialDataMap,
            JobHandle dependencies)
        {
            ImpactMaterialMappingJob impactMaterialMappingJob = new ImpactMaterialMappingJob()
            {
                Parameters = parameters,
                PhysicsMaterialMap = physicsMaterialMap,
                MaterialDataMap = materialDataMap,
                Results = materialCompositionArray
            };

            return impactMaterialMappingJob.Schedule(count, count / 8, dependencies);
        }

        public void ResetProcessor()
        {
            count = 0;
        }
    }
}
