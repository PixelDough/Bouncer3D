using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Base class for effect processing.
    /// </summary>
    public abstract class ImpactEffectProcessorBase : MonoBehaviour
    {
        /// <summary>
        /// Registers a list of materials to convert them into data structures suitable for use in jobs.
        /// </summary>
        /// <param name="materials">The materials being registered.</param>
        public abstract void RegisterMaterials(IEnumerable<ImpactMaterialAuthoring> materials);

        /// <summary>
        /// Registers a single material to convert it into data structures suitable for use in jobs.
        /// </summary>
        /// <param name="material">The material being registered.</param>
        public abstract void RegisterMaterial(ImpactMaterialAuthoring material);

        /// <summary>
        /// Completely dispose of and destroy all registered material and effect data and objects.
        /// </summary>
        public abstract void ClearAllRegistered();

        /// <summary>
        /// Schedules any jobs needed to process the collisions.
        /// </summary>
        /// <param name="collisionData">The input collision data.</param>
        /// <param name="collisionDataCount">The actual number of collisions that need to be processed. </param>
        /// <param name="materialCompositionData">Material data retrieved from the collision points.</param>
        /// <param name="velocityData">Collision velocity data.</param>
        /// <param name="dependencies">Job dependencies that must be run before processing.</param>
        public abstract JobHandle ScheduleProcessorJobs(NativeArray<CollisionInputData> collisionData, int collisionDataCount,
            NativeArray<MaterialCompositionData> materialCompositionData,
            NativeArray<ImpactVelocityData> velocityData,
            JobHandle dependencies);

        /// <summary>
        /// Processes the effect results after the jobs scheduled in ScheduleProcessorJobs have completed.
        /// </summary>
        /// <param name="collisionDataArray">Array of the collision data from which effects were created.</param>
        /// <param name="collisionObjectPairArray">Array of pairs of colliding objects.</param>
        /// <param name="materialCompositionData">Array of material data retrieved from each collision contact point.</param>
        /// <param name="velocityData">Array of collision velocity data for each collision.</param>
        public abstract void ProcessResults(NativeArray<CollisionInputData> collisionDataArray,
            CollisionObjectPair[] collisionObjectPairArray,
            NativeArray<MaterialCompositionData> materialCompositionData,
            NativeArray<ImpactVelocityData> velocityData);

        /// <summary>
        /// Update any needed data during FixedUpdate.
        /// </summary>
        public abstract void FixedUpdateProcessor();

        /// <summary>
        /// Completely resets the processor and any effects being managed by it.
        /// </summary>
        public abstract void ResetProcessor();

        /// <summary>
        /// Validate that the processor has any registered effects.
        /// </summary>
        public abstract bool HasEffects();
    }
}