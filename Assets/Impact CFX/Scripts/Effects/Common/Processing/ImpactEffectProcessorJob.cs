using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ImpactCFX
{
    /// <summary>
    /// Generic job for processing collision data to create effect results.
    /// </summary>
    /// <typeparam name="TEffect">The type of effect.</typeparam>
    /// <typeparam name="TResult">The type of result produced by the effect.</typeparam>
    [BurstCompile]
    public struct ImpactEffectProcessorJob<TEffect, TResult> : IJob where TEffect : struct, IEffectData<TResult> where TResult : struct, IEffectResult
    {
        /// <summary>
        /// The number of collision data to process.
        /// </summary>
        public int Count;

        /// <summary>
        /// The input collision data array.
        /// </summary>
        [ReadOnly]
        public NativeArray<CollisionInputData> CollisionData;
        /// <summary>
        /// The input velocity data array.
        /// </summary>
        [ReadOnly]
        public NativeArray<ImpactVelocityData> VelocityData;
        /// <summary>
        /// The input material composition array.
        /// </summary>
        [ReadOnly]
        public NativeArray<MaterialCompositionData> MaterialComposition;

        /// <summary>
        /// Array of effect data.
        /// </summary>
        [ReadOnly]
        public NativeArray<TEffect> Effects;
        /// <summary>
        /// Mapping from a material ID to a chunk of the effects array.
        /// </summary>
        [ReadOnly]
        public NativeParallelHashMap<int, ArrayChunk> MaterialEffectMap;

        /// <summary>
        /// Array of results.
        /// </summary>
        public NativeArray<TResult> Results;
        /// <summary>
        /// The number of results that were created.
        /// </summary>
        public NativeReference<int> ResultsCount;

        /// <summary>
        /// Random generator used for some effects.
        /// </summary>
        public Random Random;

        public void Execute()
        {
            for (int i = 0; i < Count; i++)
            {
                CollisionInputData collisionData = CollisionData[i];

                ImpactVelocityData velocityData = VelocityData[i];

                for (int j = 0; j < collisionData.TriggerObjectMaterialCompositionArrayChunk.Length; j++)
                {
                    int thisMaterialCompositionIndex = collisionData.TriggerObjectMaterialCompositionArrayChunk.Offset + j;
                    MaterialCompositionData thisMaterialCompositionData = MaterialComposition[thisMaterialCompositionIndex];

                    if (thisMaterialCompositionData.Composition > 0)
                    {
                        if (MaterialEffectMap.TryGetValue(thisMaterialCompositionData.MaterialData.MaterialID, out ArrayChunk effectsArrayChunk))
                        {
                            for (int k = 0; k < collisionData.HitObjectMaterialCompositionArrayChunk.Length; k++)
                            {
                                int otherMaterialCompositionIndex = collisionData.HitObjectMaterialCompositionArrayChunk.Offset + k;
                                MaterialCompositionData otherMaterialCompositionData = MaterialComposition[otherMaterialCompositionIndex];
                                ImpactTagMask otherMaterialTags = otherMaterialCompositionData.MaterialData.MaterialTags;

                                if (otherMaterialCompositionData.MaterialData.MaterialTags == ImpactTagMask.Empty)
                                {
#if IMPACTCFX_DEBUG
                                    ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_PROCESSING, "Using fallback tags for collision.", false);
#endif
                                    otherMaterialTags = thisMaterialCompositionData.MaterialData.FallbackTags;
                                }

                                for (int l = 0; l < effectsArrayChunk.Length; l++)
                                {
                                    int effectsArrayIndex = effectsArrayChunk.Offset + l;
                                    TEffect effect = Effects[effectsArrayIndex];

                                    if (effect.IncludeTags.CompareTagMask(otherMaterialTags) && !effect.ExcludeTags.CompareTagMask(otherMaterialTags))
                                    {
                                        TResult result = effect.GetResult(collisionData, otherMaterialCompositionData, velocityData, ref Random);
#if IMPACTCFX_DEBUG
                                        ImpactCFXLogger.LogEffectResult(effect.GetType(), effect.EffectID, result);
#endif
                                        if (result.IsEffectValid)
                                        {
                                            int resultIndex = ResultsCount.Value;

                                            result.CollisionIndex = i;
                                            result.MaterialCompositionIndex = otherMaterialCompositionIndex;

                                            Results[resultIndex] = result;
                                            ResultsCount.Value = resultIndex + 1;

                                            if (ResultsCount.Value >= Results.Length)
                                                return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ImpactCFXLogger.LogImpactMaterialNotFound(thisMaterialCompositionData.MaterialData.MaterialID);
                        }
                    }
                }
            }
        }
    }
}