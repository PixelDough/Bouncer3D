using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// The primary manager of the entire Impact CFX system.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Impact CFX/Impact CFX Manager")]
    public class ImpactCFXManager : MonoBehaviour
    {
        /// <summary>
        /// Defines a limit for a certain type of collision.
        /// </summary>
        [Serializable]
        public class CollisionTypeLimit
        {
            /// <summary>
            /// Is the limit being enforced?
            /// </summary>
            public bool Enabled;

            /// <summary>
            /// The maximum number of collisions of this type.
            /// </summary>
            public int Limit;

            private int currentCount;

            /// <summary>
            /// Increments the current count of collisions.
            /// </summary>
            public void Increment()
            {
                currentCount++;
            }

            /// <summary>
            /// Resets the current count of collisions.
            /// </summary>
            public void Reset()
            {
                currentCount = 0;
            }

            /// <summary>
            /// Has the limit been reached for this type of collision?
            /// </summary>
            public bool HasLimitBeenReached()
            {
                return Enabled && currentCount >= Limit;
            }
        }

        [SerializeField]
        [Tooltip("If false, collisions will not be queued and processed, but updates will still be run on active effects.")]
        private bool enableCollisionProcessing = true;

        [SerializeField]
        [Tooltip("The maximum number of collisions that can be processed in a frame.")]
        private int collisionQueueCapacity = 32;
        [SerializeField]
        [Tooltip("The capacity of material data that can be processed in a frame. This should be at least double the collision capacity.")]
        private int materialQueueCapacity = 64;

        [SerializeField]
        [Tooltip("The maximum number of Collision type collisions can be queued.")]
        private CollisionTypeLimit queueCollisionLimit = new CollisionTypeLimit();
        [SerializeField]
        [Tooltip("The maximum number of Slide type collisions can be queued.")]
        private CollisionTypeLimit queueSlideLimit = new CollisionTypeLimit();
        [SerializeField]
        [Tooltip("The maximum number of Roll type collisions can be queued.")]
        private CollisionTypeLimit queueRollLimit = new CollisionTypeLimit();

        [SerializeField]
        [Tooltip("Processors for retrieving impact materials from colliding objects.")]
        private List<ImpactMaterialProcessorBase> impactMaterialProcessors = new List<ImpactMaterialProcessorBase>();
        [SerializeField]
        [Tooltip("Processors for creating collision effects.")]
        private List<ImpactEffectProcessorBase> impactEffectProcessors = new List<ImpactEffectProcessorBase>();
        [SerializeField]
        [Tooltip("Processor for mapping physics materials to impact materials.")]
        private ImpactMaterialMappingProcessor materialMapping;

        [SerializeField]
        [Tooltip("Registry of Impact Materials to load on Start.")]
        private ImpactMaterialRegistry materialRegistry;
        [SerializeField]
        [Tooltip("Don't destroy on load so that this manager persists between scene loads.")]
        private bool dontDestroyOnLoad = true;
        [SerializeField]
        [Tooltip("Set this manager as the active manager instance on Awake.")]
        private bool setAsActiveInstance = true;

        private NativeArray<CollisionInputData> collisionDataArray;
        private CollisionObjectPair[] collisionObjectPairArray;
        private NativeArray<MaterialCompositionData> materialCompositionArray;
        private NativeArray<RigidbodyData> rigidbodyDataArray;
        private NativeParallelHashMap<int, ImpactMaterialData> materialDataMap;

        private int currentCollisionCount;
        private int materialDataArrayChunkOffset;

        /// <summary>
        /// If false, collisions will not be queued and processed, but updates will still be run on active effects.
        /// </summary>
        public bool EnableCollisionProcessing { get => enableCollisionProcessing; set => enableCollisionProcessing = value; }

        /// <summary>
        /// The maximum number of "Collision" type collisions can be queued in a frame.
        /// </summary>
        public CollisionTypeLimit QueuedCollisionsLimit => queueCollisionLimit;

        /// <summary>
        /// The maximum number of "Slide" type collisions can be queued in a frame.
        /// </summary>
        public CollisionTypeLimit QueuedSlidesLimit => queueSlideLimit;

        /// <summary>
        /// The maximum number of "Roll" type collisions can be queued in a frame.
        /// </summary>
        public CollisionTypeLimit QueuedRollsLimit => queueRollLimit;

        /// <summary>
        /// The capacity of the array used to gather collision data from colliding objects in a frame.
        /// </summary>
        public int CollisionQueueCapacity => collisionQueueCapacity;

        /// <summary>
        /// The capacity of the array used to gather materials from colliding objects in a single frame.
        /// </summary>
        public int MaterialQueueCapacity => materialQueueCapacity;

        private void OnEnable()
        {
            collisionDataArray = new NativeArray<CollisionInputData>(collisionQueueCapacity, Allocator.Persistent);
            collisionObjectPairArray = new CollisionObjectPair[collisionQueueCapacity];
            materialCompositionArray = new NativeArray<MaterialCompositionData>(materialQueueCapacity, Allocator.Persistent);
            rigidbodyDataArray = new NativeArray<RigidbodyData>(collisionQueueCapacity * 2, Allocator.Persistent);
            materialDataMap = new NativeParallelHashMap<int, ImpactMaterialData>(1, Allocator.Persistent);

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            if (setAsActiveInstance)
                ImpactCFXGlobal.SetInstance(this);

            if (materialRegistry != null)
            {
                RegisterMaterials(materialRegistry);
            }
        }

        private void OnDisable()
        {
            ClearAllRegistered();

            collisionDataArray.Dispose();
            rigidbodyDataArray.Dispose();
            materialCompositionArray.Dispose();
            materialDataMap.Dispose();

            ImpactCFXGlobal.ClearInstance(this);
        }

        /// <summary>
        /// Registers a list of materials from a registry to convert them into data structures suitable for use in jobs.
        /// </summary>
        /// <param name="materials">The material registry containing the materials to be registered.</param>
        public void RegisterMaterials(ImpactMaterialRegistry materials)
        {
            foreach (ImpactMaterialAuthoring material in materials.Materials)
            {
                RegisterMaterial(material);
            }
        }

        /// <summary>
        /// Registers a list of materials to convert them into data structures suitable for use in jobs.
        /// </summary>
        /// <param name="materials">The materials being registered.</param>
        public void RegisterMaterials(IEnumerable<ImpactMaterialAuthoring> materials)
        {
            foreach (ImpactMaterialAuthoring material in materials)
            {
                RegisterMaterial(material);
            }
        }

        /// <summary>
        /// Registers a single material to convert it into data structures suitable for use in jobs.
        /// </summary>
        /// <param name="material">The material being registered.</param>
        public void RegisterMaterial(ImpactMaterialAuthoring material)
        {
            int id = material.GetID();
            if (!materialDataMap.ContainsKey(id))
            {
#if IMPACTCFX_DEBUG
                ImpactCFXLogger.LogImpactMaterialRegistered(material);
#endif
                //Register material with processors
                foreach (ImpactEffectProcessorBase processor in impactEffectProcessors)
                {
                    processor.RegisterMaterial(material);
                }

                materialDataMap.Add(id, material.GetMaterialData());
            }
        }

        /// <summary>
        /// Completely disposes of and destroys all registered material and effect data and objects.
        /// </summary>
        public void ClearAllRegistered()
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogClear($"Clearing all registered materials and effects from the Impact CFX Manager.");
#endif
            foreach (ImpactEffectProcessorBase processor in impactEffectProcessors)
            {
                processor.ClearAllRegistered();
            }
            materialDataMap.Clear();
        }

        /// <summary>
        /// Finds all Impact Objects in the active scene and forces them to register their materials.
        /// </summary>
        public void FindObjectsAndRegisterMaterials()
        {
            ImpactObjectBase[] impactObjects = FindObjectsOfType<ImpactObjectBase>(true);
            foreach (var item in impactObjects)
            {
                item.RegisterMaterials();
            }
        }

        /// <summary>
        /// Queues a collision between 2 objects.
        /// </summary>
        /// <param name="triggerObject">The object that triggered the collision. This object will be responsible for playing effects.</param>
        /// <param name="hitObject">The object being collided with.</param>
        /// <param name="contactPoint">The contact point of the collision.</param>
        /// <param name="collisionType">The type of collision.</param>
        /// <param name="triggerObjectMaterialCount">The number of materials to get for the object that triggered the collision.</param>
        /// <param name="hitObjectMaterialCount">The number of materials to get for the object that was hit.</param>
        /// <param name="collisionVelocityMethod">The method to use for calculating the collision velocity.</param>
        /// <param name="triggerFallbackMaterial">Optional fallback material to use if no material can be found on the trigger object.</param>
        /// <param name="hitFallbackTags">Optional fallback tags to use if no material can be found on the hit object.</param>
        public void QueueCollision(
            IImpactObject triggerObject,
            IImpactObject hitObject,
            ImpactContactPoint contactPoint,
            CollisionType collisionType,
            int triggerObjectMaterialCount,
            int hitObjectMaterialCount,
            CollisionVelocityMethod collisionVelocityMethod,
            ImpactMaterialAuthoring triggerFallbackMaterial = default,
            ImpactTagMask hitFallbackTags = default)
        {
            if (!CanQueueCollision(collisionType, triggerObjectMaterialCount + hitObjectMaterialCount))
                return;

#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_QUEUE,
                $"{collisionType} : TriggerObject = {ImpactCFXLogger.GetImpactObjectString(triggerObject, contactPoint.TriggerObject)}, HitObject = {ImpactCFXLogger.GetImpactObjectString(hitObject, contactPoint.HitObject)}", true);
#endif
            //Create collision input data
            CollisionInputData c = new CollisionInputData();
            c.CollisionType = collisionType;
            c.Point = contactPoint.Point;
            c.Normal = contactPoint.Normal;
            c.CollisionMessageVelocity = contactPoint.CollisionMessageVelocity;
            c.CollisionVelocityMethod = collisionVelocityMethod;
            c.Priority = triggerObject != null ? triggerObject.GetPriority() : 0;

            c.TriggerObjectID = contactPoint.GetTriggerObjectID();
            c.HitObjectID = contactPoint.GetHitObjectID();

            MaterialCompositionData triggerFallback = triggerFallbackMaterial != null ? triggerFallbackMaterial.GetMaterialComposition() : MaterialCompositionData.Default;
            c.TriggerObjectMaterialCompositionArrayChunk = queueObjectMaterialProcessing(triggerObject, contactPoint, triggerObjectMaterialCount, 0, triggerFallback);

            MaterialCompositionData hitFallback = new MaterialCompositionData(new ImpactMaterialData() { MaterialTags = hitFallbackTags }, 1);
            c.HitObjectMaterialCompositionArrayChunk = queueObjectMaterialProcessing(hitObject, contactPoint, hitObjectMaterialCount, 1, hitFallback);

            queueRigidbodyData(triggerObject, 0);
            queueRigidbodyData(hitObject, 1);

            //Assign values to arrays
            collisionDataArray[currentCollisionCount] = c;
            collisionObjectPairArray[currentCollisionCount] = new CollisionObjectPair(contactPoint.TriggerObject, contactPoint.HitObject);

            //Increment counters
            currentCollisionCount++;
            incrementCollisionTypeLimit(collisionType);
        }

        /// <summary>
        /// Queues a collision using basic data for the object that was hit.
        /// </summary>
        /// <param name="triggerObject">The object that triggered the collision. This object will be responsible for playing effects.</param>
        /// <param name="hitObjectData">Basic data for the object being collided with.</param>
        /// <param name="contactPoint">The contact point of the collision.</param>
        /// <param name="collisionType">The type of collision.</param>
        /// <param name="triggerObjectMaterialCount">The number of materials to get for the object that triggered the collision.</param>
        /// <param name="collisionVelocityMethod">The method to use for calculating the collision velocity.</param>
        /// <param name="triggerFallbackMaterial">Optional fallback material to use if no material can be found on the trigger object.</param>
        public void QueueCollision(
            IImpactObject triggerObject,
            ImpactObjectData hitObjectData,
            ImpactContactPoint contactPoint,
            CollisionType collisionType,
            int triggerObjectMaterialCount,
            CollisionVelocityMethod collisionVelocityMethod,
            ImpactMaterialAuthoring triggerFallbackMaterial = default)
        {
            if (!CanQueueCollision(collisionType, triggerObjectMaterialCount + 1))
                return;

#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_QUEUE,
                $"{collisionType} : TriggerObject = {ImpactCFXLogger.GetImpactObjectString(triggerObject, contactPoint.TriggerObject)}, HitObject = {hitObjectData}", true);
#endif

            //Create collision input data
            CollisionInputData c = new CollisionInputData();
            c.CollisionType = collisionType;
            c.Point = contactPoint.Point;
            c.Normal = contactPoint.Normal;
            c.CollisionMessageVelocity = contactPoint.CollisionMessageVelocity;
            c.CollisionVelocityMethod = collisionVelocityMethod;
            c.Priority = triggerObject != null ? triggerObject.GetPriority() : 0;

            c.TriggerObjectID = contactPoint.GetTriggerObjectID();
            c.HitObjectID = hitObjectData.ID;

            MaterialCompositionData triggerFallback = triggerFallbackMaterial != null ? triggerFallbackMaterial.GetMaterialComposition() : MaterialCompositionData.Default;
            c.TriggerObjectMaterialCompositionArrayChunk = queueObjectMaterialProcessing(triggerObject, contactPoint, triggerObjectMaterialCount, 0, triggerFallback);

            c.HitObjectMaterialCompositionArrayChunk = new ArrayChunk(materialDataArrayChunkOffset, 1);
            materialCompositionArray[c.HitObjectMaterialCompositionArrayChunk.Offset] = hitObjectData.MaterialComposition;
            materialDataArrayChunkOffset += 1;

            queueRigidbodyData(triggerObject, 0);
            rigidbodyDataArray[currentCollisionCount * 2 + 1] = hitObjectData.RigidbodyData;

            //Assign values to arrays
            collisionDataArray[currentCollisionCount] = c;
            collisionObjectPairArray[currentCollisionCount] = new CollisionObjectPair(contactPoint.TriggerObject, contactPoint.HitObject);

            //Increment counters
            currentCollisionCount++;
            incrementCollisionTypeLimit(collisionType);
        }

        /// <summary>
        /// Queues a collision using basic data for the object triggering the collision.
        /// </summary>
        /// <param name="triggerObjectData">Basic data for the object that triggered the collision. This object will be responsible for playing effects.</param>
        /// <param name="hitObject">The object being collided with.</param>
        /// <param name="contactPoint">The contact point of the collision.</param>
        /// <param name="collisionType">The type of collision.</param>
        /// <param name="collisionVelocityMethod">The method to use for calculating the collision velocity.</param>
        /// <param name="hitFallbackTags">Optional fallback tags to use if no material can be found on the hit object.</param>
        public void QueueCollision(
            ImpactObjectData triggerObjectData,
            IImpactObject hitObject,
            ImpactContactPoint contactPoint,
            CollisionType collisionType,
            int hitObjectMaterialCount,
            CollisionVelocityMethod collisionVelocityMethod,
            ImpactTagMask hitFallbackTags = default)
        {
            if (!CanQueueCollision(collisionType, hitObjectMaterialCount + 1))
                return;

#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_QUEUE,
                $"{collisionType} : TriggerObject = {triggerObjectData}, HitObject = {ImpactCFXLogger.GetImpactObjectString(hitObject, contactPoint.HitObject)}", true);
#endif

            //Create collision input data
            CollisionInputData c = new CollisionInputData();
            c.CollisionType = collisionType;
            c.Point = contactPoint.Point;
            c.Normal = contactPoint.Normal;
            c.CollisionMessageVelocity = contactPoint.CollisionMessageVelocity;
            c.CollisionVelocityMethod = collisionVelocityMethod;
            c.Priority = triggerObjectData.Priority;

            c.TriggerObjectID = triggerObjectData.ID;
            c.HitObjectID = contactPoint.GetHitObjectID();

            c.TriggerObjectMaterialCompositionArrayChunk = new ArrayChunk(materialDataArrayChunkOffset, 1);
            materialCompositionArray[c.TriggerObjectMaterialCompositionArrayChunk.Offset] = triggerObjectData.MaterialComposition;
            materialDataArrayChunkOffset += 1;

            MaterialCompositionData hitFallback = new MaterialCompositionData(new ImpactMaterialData() { MaterialTags = hitFallbackTags }, 1);
            c.HitObjectMaterialCompositionArrayChunk = queueObjectMaterialProcessing(hitObject, contactPoint, hitObjectMaterialCount, 1, hitFallback);

            rigidbodyDataArray[currentCollisionCount * 2] = triggerObjectData.RigidbodyData;
            queueRigidbodyData(hitObject, 1);

            //Assign values to arrays
            collisionDataArray[currentCollisionCount] = c;
            collisionObjectPairArray[currentCollisionCount] = new CollisionObjectPair(contactPoint.TriggerObject, contactPoint.HitObject);

            //Increment counters
            currentCollisionCount++;
            incrementCollisionTypeLimit(collisionType);
        }

        /// <summary>
        /// Queues a collision using basic data for both the object triggering the collision and the object that was hit.
        /// </summary>
        /// <param name="triggerObjectData">Basic data for the object that triggered the collision. This object will be responsible for playing effects.</param>
        /// <param name="hitObjectData">Basic data for the object being collided with.</param>
        /// <param name="contactPoint">The contact point of the collision.</param>
        /// <param name="collisionType">The type of collision.</param>
        /// <param name="collisionVelocityMethod">The method to use for calculating the collision velocity.</param>
        public void QueueCollision(
            ImpactObjectData triggerObjectData,
            ImpactObjectData hitObjectData,
            ImpactContactPoint contactPoint,
            CollisionType collisionType,
            CollisionVelocityMethod collisionVelocityMethod)
        {
            if (!CanQueueCollision(collisionType, 1))
                return;

#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_QUEUE,
                $"{collisionType} : TriggerObject = {triggerObjectData}, HitObject = {hitObjectData}", true);
#endif

            //Create collision input data
            CollisionInputData c = new CollisionInputData();
            c.CollisionType = collisionType;
            c.Point = contactPoint.Point;
            c.Normal = contactPoint.Normal;
            c.CollisionMessageVelocity = contactPoint.CollisionMessageVelocity;
            c.CollisionVelocityMethod = collisionVelocityMethod;
            c.Priority = triggerObjectData.Priority;

            c.TriggerObjectID = triggerObjectData.ID;
            c.HitObjectID = contactPoint.GetHitObjectID();

            c.TriggerObjectMaterialCompositionArrayChunk = new ArrayChunk(materialDataArrayChunkOffset, 1);
            materialCompositionArray[c.TriggerObjectMaterialCompositionArrayChunk.Offset] = triggerObjectData.MaterialComposition;
            materialDataArrayChunkOffset += 1;

            c.HitObjectMaterialCompositionArrayChunk = new ArrayChunk(materialDataArrayChunkOffset, 1);
            materialCompositionArray[c.HitObjectMaterialCompositionArrayChunk.Offset] = hitObjectData.MaterialComposition;
            materialDataArrayChunkOffset += 1;

            rigidbodyDataArray[currentCollisionCount * 2] = triggerObjectData.RigidbodyData;
            rigidbodyDataArray[currentCollisionCount * 2 + 1] = hitObjectData.RigidbodyData;

            //Assign values to arrays
            collisionDataArray[currentCollisionCount] = c;
            collisionObjectPairArray[currentCollisionCount] = new CollisionObjectPair(contactPoint.TriggerObject, contactPoint.HitObject);

            //Increment counters
            currentCollisionCount++;
            incrementCollisionTypeLimit(collisionType);
        }

        private ArrayChunk queueObjectMaterialProcessing(IImpactObject impactObject, ImpactContactPoint contactPoint, int materialCount, int objectIndex, MaterialCompositionData fallback)
        {
            ArrayChunk materialDataArrayChunk;

            if (impactObject == null)
            {
                materialDataArrayChunk = new ArrayChunk(materialDataArrayChunkOffset, 1);

                int physicsMaterialID = objectIndex == 0 ? contactPoint.GetTriggerObjectPhysicsMaterialID() : contactPoint.GetHitObjectPhysicsMaterialID();
                if (physicsMaterialID == 0)
                {
                    materialCompositionArray[materialDataArrayChunk.Offset] = fallback;
                }
                else
                {
                    materialMapping.QueueObject(physicsMaterialID, materialDataArrayChunk);
                }

                materialDataArrayChunkOffset += 1;
            }
            else
            {
                materialDataArrayChunk = new ArrayChunk(materialDataArrayChunkOffset, materialCount);

                foreach (ImpactMaterialProcessorBase impactMaterialProcessor in impactMaterialProcessors)
                {
                    impactMaterialProcessor.QueueObject(impactObject, contactPoint, materialDataArrayChunk);
                }

                materialDataArrayChunkOffset += materialCount;
            }

            return materialDataArrayChunk;
        }

        private void queueRigidbodyData(IImpactObject impactObject, int index)
        {
            if (impactObject == null)
            {
                rigidbodyDataArray[currentCollisionCount * 2 + index] = RigidbodyData.Default;
            }
            else
            {
                rigidbodyDataArray[currentCollisionCount * 2 + index] = impactObject.GetRigidbodyData();
            }
        }

        private void FixedUpdate()
        {
            if (enableCollisionProcessing)
            {
                processQueuedCollisions();
            }

            updateProcessors();
        }

        private void processQueuedCollisions()
        {
            if (currentCollisionCount == 0)
                return;

#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_PROCESSING, $"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ BEGIN COLLISION PROCESSING ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", true);
            ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_PROCESSING, $"Processing {currentCollisionCount} collisions.", true);
#endif
            JobHandle materialProcessorDependency = new JobHandle();

            //Calculate impact velocity
            NativeArray<ImpactVelocityData> velocityResultsArray = new NativeArray<ImpactVelocityData>(currentCollisionCount, Allocator.TempJob);
            ImpactVelocityDataJob calculateVelocityDataJob = new ImpactVelocityDataJob()
            {
                CollisionData = collisionDataArray,
                RigidbodyData = rigidbodyDataArray,
                Results = velocityResultsArray
            };
            JobHandle calculateVelocityDataJobHandle = calculateVelocityDataJob.Schedule(currentCollisionCount, currentCollisionCount / 8, materialProcessorDependency);

            //Material processing jobs
            NativeArray<JobHandle> materialProcessorJobHandles = new NativeArray<JobHandle>(impactMaterialProcessors.Count, Allocator.Temp);
            int index = 0;
            foreach (ImpactMaterialProcessorBase impactMaterialProcessor in impactMaterialProcessors)
            {
                materialProcessorJobHandles[index] = impactMaterialProcessor.ScheduleProcessorJobs(materialCompositionArray, materialDataMap, materialProcessorDependency);
                index++;
            }

            JobHandle combinedMaterialJobHandles = JobHandle.CombineDependencies(materialProcessorJobHandles);

            //Combine dependencies from velocity and material jobs
            JobHandle materialMappingJobHandle = materialMapping.ScheduleProcessorJobs(materialCompositionArray, materialDataMap, materialProcessorDependency);

            JobHandle effectProcessorDependency = JobHandle.CombineDependencies(combinedMaterialJobHandles, materialMappingJobHandle);
            effectProcessorDependency = JobHandle.CombineDependencies(effectProcessorDependency, calculateVelocityDataJobHandle);

            NativeArray<JobHandle> effectProcessorJobHandles = new NativeArray<JobHandle>(impactEffectProcessors.Count, Allocator.Temp);
            index = 0;
            foreach (ImpactEffectProcessorBase impactEffectProcessor in impactEffectProcessors)
            {
                if (impactEffectProcessor.HasEffects())
                    effectProcessorJobHandles[index] = impactEffectProcessor.ScheduleProcessorJobs(collisionDataArray, currentCollisionCount, materialCompositionArray, velocityResultsArray, effectProcessorDependency);
                else
                    effectProcessorJobHandles[index] = effectProcessorDependency;

                index++;
            }

            //Complete all jobs
            JobHandle.CompleteAll(effectProcessorJobHandles);

            //Process results for each effect
            foreach (ImpactEffectProcessorBase impactEffectProcessor in impactEffectProcessors)
            {
                if (impactEffectProcessor.HasEffects())
                {
                    impactEffectProcessor.ProcessResults(collisionDataArray, collisionObjectPairArray, materialCompositionArray, velocityResultsArray);
                }
            }

            //Dispose temp arrays
            materialProcessorJobHandles.Dispose();
            effectProcessorJobHandles.Dispose();
            velocityResultsArray.Dispose();

            //Reset counts
            currentCollisionCount = 0;
            materialDataArrayChunkOffset = 0;

            queueCollisionLimit.Reset();
            queueSlideLimit.Reset();
            queueRollLimit.Reset();

            //Reset material processors
            foreach (ImpactMaterialProcessorBase impactMaterialProcessor in impactMaterialProcessors)
            {
                impactMaterialProcessor.ResetProcessor();
            }

            materialMapping.ResetProcessor();

            //Reset all material data so next frame is working with clean data
            ImpactMaterialDataResetJob impactMaterialDataResetJob = new ImpactMaterialDataResetJob()
            {
                MaterialComposition = materialCompositionArray
            };
            impactMaterialDataResetJob.Run(materialCompositionArray.Length);

#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_PROCESSING, $"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ END COLLISION PROCESSING ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", true);
#endif
        }

        private void updateProcessors()
        {
            foreach (ImpactEffectProcessorBase processor in impactEffectProcessors)
            {
                processor.FixedUpdateProcessor();
            }
        }

        /// <summary>
        /// Can a collision of the given type be queued?
        /// </summary>
        /// <param name="collisionType">The type of collision</param>
        /// <param name="materialCount">The number of materials that will need to be processed for the collision.</param>
        public bool CanQueueCollision(CollisionType collisionType, int materialCount)
        {
#if IMPACTCFX_DEBUG
            if (currentCollisionCount >= collisionQueueCapacity || materialDataArrayChunkOffset >= materialQueueCapacity - materialCount)
                ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_TRIGGER, $"Cannot queue collision: collision queue full. ({currentCollisionCount}/{collisionQueueCapacity})", true);
            else if (hasCollisionTypeReachedLimit(collisionType))
                ImpactCFXLogger.LogDebug(ImpactCFXLogger.DEBUG_HEADER_TRIGGER, $"Cannot queue collision: CollisionType '{collisionType}' has reached its limit.", true);
#endif
            return enableCollisionProcessing
                && enabled
                && currentCollisionCount < collisionQueueCapacity
                && materialDataArrayChunkOffset < materialQueueCapacity - materialCount
                && !hasCollisionTypeReachedLimit(collisionType)
                && gameObject.activeInHierarchy;
        }

        private bool hasCollisionTypeReachedLimit(CollisionType collisionType)
        {
            if (collisionType == CollisionType.Collision)
            {
                return queueCollisionLimit.HasLimitBeenReached();
            }
            else if (collisionType == CollisionType.Slide)
            {
                return queueSlideLimit.HasLimitBeenReached();
            }
            else if (collisionType == CollisionType.Roll)
            {
                return queueRollLimit.HasLimitBeenReached();
            }

            return false;
        }

        private void incrementCollisionTypeLimit(CollisionType collisionType)
        {
            if (collisionType == CollisionType.Collision)
            {
                queueCollisionLimit.Increment();
            }
            else if (collisionType == CollisionType.Slide)
            {
                queueSlideLimit.Increment();
            }
            else if (collisionType == CollisionType.Roll)
            {
                queueRollLimit.Increment();
            }
        }

        /// <summary>
        /// Try to get the effect processor of the given type.
        /// </summary>
        /// <typeparam name="T">The type of effect processor.</typeparam>
        /// <param name="effectProcessor">The effect processor that was found, if any.</param>
        /// <returns>True if an effect processor matching the given type was found. False otherwise.</returns>
        public bool TryGetEffectProcessor<T>(out T effectProcessor) where T : ImpactEffectProcessorBase
        {
            foreach (ImpactEffectProcessorBase item in impactEffectProcessors)
            {
                if (item is T converted)
                {
                    effectProcessor = converted;
                    return true;
                }
            }

            effectProcessor = default(T);
            return false;
        }

        /// <summary>
        /// Try to get the material processor of the given type.
        /// </summary>
        /// <typeparam name="T">The type of effect processor.</typeparam>
        /// <param name="materialProcessor">The material processor that was found, if any.</param>
        /// <returns>True if an material processor matching the given type was found. False otherwise.</returns>
        public bool TryGetMaterialProcessor<T>(out T materialProcessor) where T : ImpactMaterialProcessorBase
        {
            foreach (ImpactMaterialProcessorBase item in impactMaterialProcessors)
            {
                if (item is T converted)
                {
                    materialProcessor = converted;
                    return true;
                }
            }

            materialProcessor = default(T);
            return false;
        }

        /// <summary>
        /// Resets all effect processors controlled by this manager.
        /// </summary>
        public void ResetAllEffectProcessors()
        {
            foreach (ImpactEffectProcessorBase item in impactEffectProcessors)
            {
                item.ResetProcessor();
            }
        }
    }
}