using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Base class that provides a common implementation for simple effects that do not require object pooling.
    /// </summary>
    /// <typeparam name="TEffectAuthoring">The type for the script used for effect authoring.</typeparam>
    /// <typeparam name="TEffect">The type for the script used for the effect data suitable for jobs.</typeparam>
    /// <typeparam name="TEffectResult">The type for the script used for the effect result suitable for jobs.</typeparam>
    public abstract class ImpactSimpleEffectProcessor<TEffectAuthoring, TEffect, TEffectResult> : ImpactEffectProcessorBase
        where TEffectAuthoring : ImpactEffectAuthoringBase
        where TEffect : unmanaged, IEffectData<TEffectResult>
        where TEffectResult : unmanaged, IEffectResult
    {
        /// <summary>
        /// Event invoked when an effect is played.
        /// </summary>
        public event Action<TEffectResult, CollisionResultData> OnEffectPlayed;

        [SerializeField]
        [Tooltip("The maximum number of effects that can be queued in a frame.")]
        protected OverrideValueInt queueCapacity;

        protected NativeParallelHashMap<int, ArrayChunk> effectArrayChunkMap;
        protected NativeList<TEffect> effects;

        protected NativeArray<TEffectResult> effectResults;
        protected NativeReference<int> effectResultCount;

        private bool collectionsInitialized;

        protected virtual void OnEnable()
        {
            initCollections();
        }

        protected virtual void OnDisable()
        {
            disposeCollections();
        }

        private void initCollections()
        {
            if (collectionsInitialized)
                return;

            effectResultCount = new NativeReference<int>(0, Allocator.Persistent);
            effectArrayChunkMap = new NativeParallelHashMap<int, ArrayChunk>(0, Allocator.Persistent);
            effects = new NativeList<TEffect>(0, Allocator.Persistent);

            //If not overriden, zero out queue capacity so it can be determined automatically.
            if (!queueCapacity.Override)
                queueCapacity.Value = 0;
            else
            {
                effectResults = new NativeArray<TEffectResult>(queueCapacity.Value, Allocator.Persistent);
            }

            collectionsInitialized = true;
        }

        private void disposeCollections()
        {
            if (effectResultCount.IsCreated)
                effectResultCount.Dispose();
            if (effects.IsCreated)
                effects.Dispose();
            if (effectArrayChunkMap.IsCreated)
                effectArrayChunkMap.Dispose();
            if (effectResults.IsCreated)
                effectResults.Dispose();

            collectionsInitialized = false;
        }

        public override void RegisterMaterials(IEnumerable<ImpactMaterialAuthoring> impactMaterials)
        {
            initCollections();

            foreach (ImpactMaterialAuthoring impactMaterial in impactMaterials)
            {
                RegisterMaterial(impactMaterial);
            }
        }

        public override void RegisterMaterial(ImpactMaterialAuthoring impactMaterial)
        {
            initCollections();

            int key = impactMaterial.GetID();

            if (!effectArrayChunkMap.ContainsKey(key))
            {
                ArrayChunk effectsArrayChunk = new ArrayChunk();
                effectsArrayChunk.Offset = effects.Length;

                foreach (ImpactMaterialAuthoring.EffectSet effectSet in impactMaterial.EffectSets)
                {
                    foreach (ImpactEffectAuthoringBase effectAuthoring in effectSet.Effects)
                    {
                        if (effectAuthoring is TEffectAuthoring matchingEffectAuthoring)
                        {
                            if (!effectAuthoring.Validate())
                                continue;

#if IMPACTCFX_DEBUG
                            ImpactCFXLogger.LogImpactEffectRegistered(effectAuthoring);
#endif
                            TEffect effectData = getEffect(matchingEffectAuthoring);
                            effectData.EffectID = effectAuthoring.GetID();
                            effectData.IncludeTags = effectSet.IncludeTags;
                            effectData.ExcludeTags = effectSet.ExcludeTags;

                            effects.Add(effectData);
                            effectsArrayChunk.Length += 1;
                        }
                    }
                }

                effectArrayChunkMap.Add(key, effectsArrayChunk);

                reinitEffectResults();
            }
        }

        private void reinitEffectResults()
        {
            if (effectResults.IsCreated)
            {
                //If queue capacity is not override (meaning it is determined automatically), check if the effect result length does not match the current value.
                if (!queueCapacity.Override && effectResults.Length < queueCapacity.Value)
                {
                    effectResults.Dispose();
                    effectResults = new NativeArray<TEffectResult>(queueCapacity.Value, Allocator.Persistent);
                }
            }
            else
            {
                effectResults = new NativeArray<TEffectResult>(queueCapacity.Value, Allocator.Persistent);
            }
        }

        public override void ClearAllRegistered()
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogClear($"Clearing all registered effects from processor '{name}'.");
#endif
            if (effects.IsCreated)
                effects.Clear();
            if (effectArrayChunkMap.IsCreated)
                effectArrayChunkMap.Clear();
            if (effectResults.IsCreated)
                effectResults.Dispose();
        }

        /// <summary>
        /// Gets effect data from the effect authoring asset.
        /// </summary>
        /// <param name="effectAuthoring">The source effect authoring asset.</param>
        /// <returns>An effect data instance.</returns>
        protected abstract TEffect getEffect(TEffectAuthoring effectAuthoring);

        public override void FixedUpdateProcessor() { }

        public override JobHandle ScheduleProcessorJobs(NativeArray<CollisionInputData> collisionData,
            int collisionDataCount,
            NativeArray<MaterialCompositionData> materialCompositionData,
            NativeArray<ImpactVelocityData> velocityData,
            JobHandle dependencies)
        {
            int count = Mathf.Min(effectResults.Length, collisionDataCount);
            effectResultCount.Value = 0;

            ImpactEffectProcessorJob<TEffect, TEffectResult> effectProcessorJob = getEffectProcessorJobBase();
            effectProcessorJob.Count = count;
            effectProcessorJob.CollisionData = collisionData;
            effectProcessorJob.MaterialComposition = materialCompositionData;
            effectProcessorJob.VelocityData = velocityData;
            effectProcessorJob.MaterialEffectMap = effectArrayChunkMap;
            effectProcessorJob.Effects = effects;
            effectProcessorJob.Results = effectResults;
            effectProcessorJob.ResultsCount = effectResultCount;
            effectProcessorJob.Random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue));

            JobHandle effectProcessorJobHandle = effectProcessorJob.Schedule(dependencies);
            return effectProcessorJobHandle;
        }

        /// <summary>
        /// Gets a base instance of the effect processor job.
        /// This instance does not need to have any data populated.
        /// This is needed to ensure that Burst compiles this job correctly, due to the use of generics and type constraints.
        /// </summary>
        /// <returns>An empty instance of the ImpactEffectProcessorJob.</returns>
        protected abstract ImpactEffectProcessorJob<TEffect, TEffectResult> getEffectProcessorJobBase();

        public override void ProcessResults(NativeArray<CollisionInputData> collisionDataArray,
            CollisionObjectPair[] collisionObjectPairArray,
            NativeArray<MaterialCompositionData> materialCompositionData,
            NativeArray<ImpactVelocityData> velocityData)
        {
            for (int i = 0; i < effectResultCount.Value; i++)
            {
                TEffectResult effectResult = effectResults[i];

                if (!effectResult.IsEffectValid)
                    continue;

                CollisionInputData source = collisionDataArray[effectResult.CollisionIndex];
                CollisionObjectPair collisionObjectPair = collisionObjectPairArray[effectResult.CollisionIndex];
                MaterialCompositionData materialComposition = materialCompositionData[effectResult.MaterialCompositionIndex];
                ImpactVelocityData impactVelocityData = velocityData[effectResult.CollisionIndex];

                PlayEffect(effectResult, source, collisionObjectPair, materialComposition, impactVelocityData);
            }
        }

        /// <summary>
        /// Immediately plays an effect using the given effect and collision data.
        /// This method uses detailed data about the collision.
        /// </summary>
        /// <param name="effectResult">The effect result data.</param>
        /// <param name="collisionData">The source collision data from which the effect was created.</param>
        /// <param name="collisionObjectPair">The pair of objects that are colliding.</param>
        /// <param name="materialCompositionData">The material composition data from the collision contact point.</param>
        /// <param name="velocityData">The velocity of the collision.</param>
        public virtual void PlayEffect(TEffectResult effectResult,
            CollisionInputData collisionData,
            CollisionObjectPair collisionObjectPair,
            MaterialCompositionData materialCompositionData,
            ImpactVelocityData velocityData)
        {
            CollisionResultData collisionResultData = new CollisionResultData(collisionData, velocityData, materialCompositionData, collisionObjectPair);
            PlayEffect(effectResult, collisionResultData);
        }

        /// <summary>
        /// Immediately plays an effect using the given effect and collision result data.
        /// This method uses simplified data about the collision.
        /// </summary>
        /// <param name="effectResult">The effect result data.</param>
        /// <param name="collisionResultData">Data about the collision.</param>
        public virtual void PlayEffect(TEffectResult effectResult, CollisionResultData collisionResultData)
        {
            invokeEffectPlayedEvent(effectResult, collisionResultData);
        }

        /// <summary>
        /// Invokes the <see cref=">OnEffectPlayed"/> event with the given data.
        /// </summary>
        /// <param name="effectResult">The effect result data.</param>
        /// <param name="collisionResultData">Data about the collision.</param>
        protected void invokeEffectPlayedEvent(TEffectResult effectResult, CollisionResultData collisionResultData)
        {
            OnEffectPlayed?.Invoke(effectResult, collisionResultData);
        }

        public override void ResetProcessor() { }

        public override bool HasEffects()
        {
            return effects.Length > 0;
        }
    }
}