using ImpactCFX.Pooling;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Provides common implementation for an effect that relies on pooled objects.
    /// </summary>
    /// <typeparam name="TEffectAuthoring">The type for the script used for effect authoring.</typeparam>
    /// <typeparam name="TEffect">The type for the script used for the effect data suitable for jobs.</typeparam>
    /// <typeparam name="TEffectResult">The type for the script used for the effect result suitable for jobs.</typeparam>
    /// <typeparam name="TPool">The type of pool being used.</typeparam>
    /// <typeparam name="TPooledObject">The type of pooled object being used.</typeparam>
    public abstract class ImpactPooledEffectProcessor<TEffectAuthoring, TEffect, TEffectResult, TPool, TPooledObject> : ImpactSimpleEffectProcessor<TEffectAuthoring, TEffect, TEffectResult>
        where TEffectAuthoring : ImpactPooledEffectAuthoringBase
        where TEffect : unmanaged, IPooledEffectData<TEffectResult>
        where TEffectResult : unmanaged, IEffectResult, IObjectPoolRequest
        where TPool : EffectObjectPool<TPooledObject>
        where TPooledObject : PooledEffectObjectBase
    {
        /// <summary>
        /// Event invoked when a pooled effect is played.
        /// </summary>
        public event Action<TEffectResult, TPooledObject, CollisionResultData> OnPooledEffectPlayed;

        private List<TPool> pools = new List<TPool>();
        private int largestPoolSize = 0;

        protected override TEffect getEffect(TEffectAuthoring effectAuthoring)
        {
            PooledEffectObjectBase pooledObjectTemplate = effectAuthoring.GetTemplateObject();

            if (!pools.Exists(p => p.PoolID == pooledObjectTemplate.GetPoolID()))
            {
                GameObject g = new GameObject("Object Pool (" + pooledObjectTemplate.name + ")");
                DontDestroyOnLoad(g);

                TPool pool = g.AddComponent<TPool>();
                pool.InitializeWithTemplate(pooledObjectTemplate as TPooledObject);

                pools.Add(pool);

                largestPoolSize = Mathf.Max(queueCapacity.Value, pool.PoolSize);
                if (!queueCapacity.Override)
                    queueCapacity.Value = largestPoolSize;
            }

            TEffect effect = getEffectForPooledEffect(effectAuthoring);
            effect.TemplateID = pooledObjectTemplate.GetPoolID();

            return effect;
        }

        /// <summary>
        /// Gets effect data from the effect authoring asset.
        /// </summary>
        /// <param name="effectAuthoring">The source effect authoring asset.</param>
        /// <returns>An effect data instance.</returns>
        protected abstract TEffect getEffectForPooledEffect(TEffectAuthoring effectAuthoring);

        public override void ClearAllRegistered()
        {
            base.ClearAllRegistered();

            foreach (TPool pool in pools)
            {
                if (pool != null && !pool.Equals(null))
                    pool.Destroy();
            }

            pools.Clear();
        }

        public override JobHandle ScheduleProcessorJobs(NativeArray<CollisionInputData> collisionData, int collisionDataCount,
            NativeArray<MaterialCompositionData> materialCompositionData,
            NativeArray<ImpactVelocityData> velocityData,
            JobHandle dependencies)
        {
            JobHandle baseJobHandle = base.ScheduleProcessorJobs(collisionData, collisionDataCount, materialCompositionData, velocityData, dependencies);

            //Get indices of pooled objects to use
            NativeArray<JobHandle> objectPoolJobHandles = new NativeArray<JobHandle>(pools.Count, Allocator.Temp);

            for (int i = 0; i < pools.Count; i++)
            {
                TPool pool = pools[i];

                ObjectPoolJob<TEffectResult> objectPoolJob = getObjectPoolJobBase();
                objectPoolJob.TemplateID = pool.PoolID;
                objectPoolJob.Stealing = pool.Stealing;
                objectPoolJob.PooledObjects = pool.GetPooledObjectDataArray();
                objectPoolJob.ObjectRequests = effectResults;
                objectPoolJob.ObjectRequestCount = effectResultCount;
                objectPoolJob.CurrentFrame = Time.frameCount;

                objectPoolJobHandles[i] = objectPoolJob.Schedule(baseJobHandle);
            }

            JobHandle combinedObjectPoolJobs = JobHandle.CombineDependencies(objectPoolJobHandles);
            objectPoolJobHandles.Dispose();

            return combinedObjectPoolJobs;
        }

        /// <summary>
        /// Gets a base instance of the object pool job.
        /// This instance does not need to have any data populated.
        /// This is needed to ensure that Burst compiles this job correctly, due to the use of generics and type constraints.
        /// </summary>
        /// <returns>An empty instance of the ObjectPoolJob.</returns>
        protected abstract ObjectPoolJob<TEffectResult> getObjectPoolJobBase();

        public override void ProcessResults(NativeArray<CollisionInputData> collisionDataArray,
            CollisionObjectPair[] collisionObjectPairArray,
            NativeArray<MaterialCompositionData> materialCompositionData,
            NativeArray<ImpactVelocityData> velocityData)
        {
            for (int i = 0; i < effectResultCount.Value; i++)
            {
                TEffectResult effectResult = effectResults[i];

                if (effectResult.IsEffectValid && effectResult.IsObjectPoolRequestValid && effectResult.ObjectIndex >= 0)
                {
                    CollisionInputData collision = collisionDataArray[effectResult.CollisionIndex];
                    CollisionObjectPair collisionObjectPair = collisionObjectPairArray[effectResult.CollisionIndex];
                    MaterialCompositionData materialComposition = materialCompositionData[effectResult.MaterialCompositionIndex];
                    ImpactVelocityData impactVelocityData = velocityData[effectResult.CollisionIndex];

                    PlayEffect(effectResult, collision, collisionObjectPair, materialComposition, impactVelocityData);
                }
            }
        }

        public override void PlayEffect(
            TEffectResult effectResult,
            CollisionResultData collisionResultData)
        {
            base.PlayEffect(effectResult, collisionResultData);

            if (findPool(effectResult.TemplateID, out TPool pool))
            {
                TPooledObject a = pool.RetrieveObject(effectResult.ObjectIndex, effectResult.Priority, effectResult.ContactPointID);
                PlayPooledEffect(effectResult, a, collisionResultData);
            }
        }

        /// <summary>
        /// Immediately plays an effect with a pooled object instance.
        /// This method provides uses data about the collision.
        /// </summary>
        /// <param name="effectResult">The effect result data.</param>
        /// <param name="pooledObject">The pooled object instance being used.</param>
        /// <param name="collisionData">The source collision data from which the effect was created.</param>
        /// <param name="collisionObjectPair">The pair of objects that are colliding.</param>
        /// <param name="materialCompositionData">The material composition data from the collision contact point.</param>
        /// <param name="velocityData">The velocity of the collision.</param>
        public virtual void PlayPooledEffect(
            TEffectResult effectResult,
            TPooledObject pooledObject,
            CollisionInputData collisionData,
            CollisionObjectPair collisionObjectPair,
            MaterialCompositionData materialCompositionData,
            ImpactVelocityData velocityData)
        {
            CollisionResultData collisionResultData = new CollisionResultData(collisionData, velocityData, materialCompositionData, collisionObjectPair);
            PlayPooledEffect(effectResult, pooledObject, collisionResultData);
        }

        /// <summary>
        /// Immediately plays an effect with a pooled object instance.
        /// This method uses simplified collision result data.
        /// </summary>
        /// <param name="effectResult">The effect result data.</param>
        /// <param name="pooledObject">The pooled object instance being used.</param>
        /// <param name="collisionResultData">Simplified data about the collision.</param>
        public virtual void PlayPooledEffect(TEffectResult effectResult, TPooledObject pooledObject, CollisionResultData collisionResultData)
        {
            invokePooledEffectPlayedEvent(effectResult, pooledObject, collisionResultData);
        }

        /// <summary>
        /// Invokes the <see cref=">OnPooledEffectPlayed"/> event with the given data.
        /// </summary>
        /// <param name="effectResult">The effect result data.</param>
        /// <param name="pooledObject">The pooled object instance being used.</param>
        /// <param name="collisionResultData">Data about the collision.</param>
        protected void invokePooledEffectPlayedEvent(TEffectResult effectResult, TPooledObject pooledObject, CollisionResultData collisionResultData)
        {
            OnPooledEffectPlayed?.Invoke(effectResult, pooledObject, collisionResultData);
        }

        private bool findPool(int poolID, out TPool pool)
        {
            foreach (TPool p in pools)
            {
                if (p.PoolID == poolID)
                {
                    pool = p;
                    return true;
                }
            }

            pool = default(TPool);
            return false;
        }

        public override void FixedUpdateProcessor()
        {
            foreach (TPool pool in pools)
            {
                pool.UpdatePooledObjects();
            }
        }

        public override void ResetProcessor()
        {
            foreach (TPool p in pools)
            {
                p.ReturnAllObjectsToPool();
            }
        }


        /// <summary>
        /// Scan each pool for missing objects that have been destroyed and re-instantiate them.
        /// </summary>
        public void ReinstantiateMissingPooledObjects()
        {
            foreach (TPool p in pools)
            {
                p.ReinstantiateMissingObjects();
            }
        }
    }
}