using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX
{
    [DisallowMultipleComponent]
    public partial class ImpactTerrainMaterialProcessor : ImpactMaterialProcessorBase
    {
        private class ImpactTerrainMaterialSubProcessor : IDisposable
        {
            private NativeArray<ImpactTerrainMaterialJobParameter> parameters;

            private ImpactObjectTerrain terrain;
            private int count;
            private int capacity;

            public ImpactTerrainMaterialSubProcessor(ImpactObjectTerrain terrain, int capacity)
            {
                this.terrain = terrain;
                this.capacity = capacity;

                parameters = new NativeArray<ImpactTerrainMaterialJobParameter>(capacity, Allocator.Persistent);
            }

            public void Dispose()
            {
                parameters.Dispose();
            }

            public void QueueObject(IImpactObject impactObject, ImpactContactPoint contactPoint, ArrayChunk resultArrayChunk)
            {
                if (count >= capacity)
                    return;

                if (impactObject is ImpactObjectTerrain impactObjectTerrain)
                {
                    parameters[count] = new ImpactTerrainMaterialJobParameter()
                    {
                        Point = contactPoint.Point,
                        ResultArrayChunk = resultArrayChunk
                    };

                    count++;
                }
            }

            public JobHandle ScheduleProcessorJobs(
                NativeArray<MaterialCompositionData> materialCompositionArray,
                NativeParallelHashMap<int, ImpactMaterialData> materialDataMap,
                JobHandle dependencies)
            {
                ImpactTerrainMaterialJob impactTerrainMaterialJob = new ImpactTerrainMaterialJob()
                {
                    Parameters = parameters,

                    TerrainPosition = terrain.transform.position,
                    TerrainSize = terrain.Size,

                    Alphamaps = terrain.GetAlphamaps(),
                    AlphamapResolution = terrain.AlphamapResolution,

                    TerrainLayerMaterialIDs = terrain.GetTerrainLayerMaterialIDs(),
                    DefaultMaterialID = terrain.DefaultMaterialID,

                    MaterialDataMap = materialDataMap,

                    Results = materialCompositionArray,
                };

                return impactTerrainMaterialJob.Schedule(count, count / 8, dependencies);
            }

            public void ResetProcessor()
            {
                count = 0;
            }

            public int GetID()
            {
                return terrain.GetID();
            }

            public bool IsValid()
            {
                return terrain != null && !terrain.Equals(null);
            }
        }

        [SerializeField]
        [Tooltip("The maximum number of objects that can be queued in a frame for processing.")]
        private OverrideValueInt queueCapacity;

        private List<ImpactTerrainMaterialSubProcessor> subProcessors = new List<ImpactTerrainMaterialSubProcessor>();

        private void OnEnable()
        {
            if (!queueCapacity.Override)
            {
                ImpactCFXManager impactCollisionEffectProcessor = GetComponentInParent<ImpactCFXManager>();
                queueCapacity.Value = impactCollisionEffectProcessor.MaterialQueueCapacity;
            }
        }

        private void OnDisable()
        {
            foreach (ImpactTerrainMaterialSubProcessor subProcessor in subProcessors)
            {
                subProcessor.Dispose();
            }

            subProcessors.Clear();
        }

        public override void QueueObject(IImpactObject impactObject, ImpactContactPoint contactPoint, ArrayChunk resultArrayChunk)
        {
            if (impactObject is ImpactObjectTerrain impactObjectTerrain)
            {
                int id = impactObjectTerrain.GetID();
                bool foundExisting = false;

                foreach (ImpactTerrainMaterialSubProcessor subProcessor in subProcessors)
                {
                    if (subProcessor.GetID() == id)
                    {
                        subProcessor.QueueObject(impactObject, contactPoint, resultArrayChunk);
                        foundExisting = true;
                        break;
                    }
                }

                if (!foundExisting)
                {
                    ImpactTerrainMaterialSubProcessor newSubProcessor = new ImpactTerrainMaterialSubProcessor(impactObjectTerrain, queueCapacity.Value);
                    newSubProcessor.QueueObject(impactObject, contactPoint, resultArrayChunk);

                    subProcessors.Add(newSubProcessor);
                }
            }
        }

        public override JobHandle ScheduleProcessorJobs(
            NativeArray<MaterialCompositionData> materialCompositionArray,
            NativeParallelHashMap<int, ImpactMaterialData> materialDataMap,
            JobHandle dependencies)
        {
            NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(subProcessors.Count, Allocator.Temp);

            int jobHandleIndex = 0;
            for (int i = 0; i < subProcessors.Count; i++)
            {
                ImpactTerrainMaterialSubProcessor subProcessor = subProcessors[i];

                if (subProcessor.IsValid())
                {
                    jobHandles[jobHandleIndex] = subProcessor.ScheduleProcessorJobs(materialCompositionArray, materialDataMap, dependencies);
                }
                else
                {
                    //Remove invalid sub-processors, for example if the terrain is destroyed.
                    jobHandles[jobHandleIndex] = new JobHandle();
                    subProcessor.Dispose();
                    subProcessors.RemoveAt(i);
                    i--;
                }

                jobHandleIndex++;
            }

            JobHandle combinedHandle = JobHandle.CombineDependencies(jobHandles);
            jobHandles.Dispose();

            return combinedHandle;
        }

        public override void ResetProcessor()
        {
            foreach (ImpactTerrainMaterialSubProcessor subProcessor in subProcessors)
            {
                subProcessor.ResetProcessor();
            }
        }
    }
}
