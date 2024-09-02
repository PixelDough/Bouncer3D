using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Base for impact material processors that get the material data for a specific kind of object.
    /// </summary>
    public abstract class ImpactMaterialProcessorBase : MonoBehaviour
    {
        /// <summary>
        /// Queues an object for material processing. The impact object could be of any type, so it is important to check the type before proceeding.
        /// </summary>
        /// <param name="impactObject">The object to quque. This object could be of any type, so it is important to check the type before proceeding.</param>
        /// <param name="contactPoint">The contact point to get the material data from.</param>
        /// <param name="resultArrayChunk">The chunk of the material composition array that has been assigned for this object.</param>
        public abstract void QueueObject(IImpactObject impactObject, ImpactContactPoint contactPoint, ArrayChunk resultArrayChunk);

        /// <summary>
        /// Schedules all jobs needed to get the material data.
        /// </summary>
        /// <param name="materialCompositionArray">The array that all of the resulting data should be put in.</param>
        /// <param name="materialDataMap">Mapping from material IDs to material data.</param>
        public abstract JobHandle ScheduleProcessorJobs(
            NativeArray<MaterialCompositionData> materialCompositionArray,
            NativeParallelHashMap<int, ImpactMaterialData> materialDataMap,
            JobHandle dependencies);

        /// <summary>
        /// Resets data at the end of a fixed update step.
        /// </summary>
        public abstract void ResetProcessor();
    }
}

