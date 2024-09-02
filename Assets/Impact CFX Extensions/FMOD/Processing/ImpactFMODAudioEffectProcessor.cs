using ImpactCFX.Pooling;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ImpactCFX.FMOD
{
    [AddComponentMenu("Impact CFX/FMOD/Impact FMOD Audio Effect Processor")]
    [DisallowMultipleComponent]
    public class ImpactFMODAudioEffectProcessor : ImpactSimpleEffectProcessor<ImpactFMODAudioEffectAuthoring, FMODAudioEffect, FMODAudioEffectResult>
    {
        [SerializeField]
        [Tooltip("Object pool configuration for the 'proxy' Game Objects used to communicate with FMOD.")]
        private ObjectPoolConfig proxyObjectPoolConfig;
        [SerializeField]
        [Tooltip("Behavior for attaching effects to one of the colliding objects that triggered the effect.")]
        private EffectAttachMode effectAttachMode;
        [SerializeField]
        [Tooltip("How long audio will fade out when sliding and rolling effects end.")]
        private float audioFadeOutTime = 0.2f;

        private FMODAudioEffectProxyPool audioPool;

        protected override void OnEnable()
        {
            base.OnEnable();

            GameObject g = new GameObject("FMOD Audio Effect Proxy Object Pool");
            DontDestroyOnLoad(g);

            audioPool = g.AddComponent<FMODAudioEffectProxyPool>();
            audioPool.Setup(effectAttachMode, audioFadeOutTime);
            audioPool.Initialize(1, proxyObjectPoolConfig);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (audioPool != null && !audioPool.Equals(null))
            {
                audioPool.Destroy();
                audioPool = null;
            }
        }

        protected override FMODAudioEffect getEffect(ImpactFMODAudioEffectAuthoring effectAuthoring)
        {
            //Set here due to timing issues
            if (!queueCapacity.Override)
                queueCapacity.Value = proxyObjectPoolConfig.PoolSize;

            return effectAuthoring.GetFMODAudioEffect();
        }

        protected override ImpactEffectProcessorJob<FMODAudioEffect, FMODAudioEffectResult> getEffectProcessorJobBase()
        {
            return new ImpactEffectProcessorJob<FMODAudioEffect, FMODAudioEffectResult>();
        }

        public override JobHandle ScheduleProcessorJobs(NativeArray<CollisionInputData> collisionData, int collisionDataCount,
            NativeArray<MaterialCompositionData> materialCompositionData,
            NativeArray<ImpactVelocityData> velocityData,
            JobHandle dependencies)
        {
            JobHandle baseJobHandle = base.ScheduleProcessorJobs(collisionData, collisionDataCount, materialCompositionData, velocityData, dependencies);

            //Get indices of pooled objects to use
            ObjectPoolJob<FMODAudioEffectResult> objectPoolJob = new ObjectPoolJob<FMODAudioEffectResult>()
            {
                TemplateID = 1,
                Stealing = audioPool.Stealing,
                PooledObjects = audioPool.GetPooledObjectDataArray(),
                ObjectRequests = effectResults,
                ObjectRequestCount = effectResultCount,
                CurrentFrame = Time.frameCount
            };

            JobHandle objectPoolJobHandle = objectPoolJob.Schedule(baseJobHandle);

            return objectPoolJobHandle;
        }

        public override void PlayEffect(FMODAudioEffectResult effectResult, CollisionResultData collisionResultData)
        {
            base.PlayEffect(effectResult, collisionResultData);

            FMODAudioEffectProxy a = audioPool.RetrieveObject(effectResult.ObjectIndex, effectResult.Priority, effectResult.ContactPointID);
            if (effectResult.IsUpdate)
            {
                a.UpdateFMODAudioEffectCollision(effectResult, collisionResultData);
            }
            else
            {
                a.PlayFMODAudioEffect(effectResult, collisionResultData);
            }
        }

        public override void FixedUpdateProcessor()
        {
            audioPool.UpdatePooledObjects();
        }

        public override void ResetProcessor()
        {
            audioPool.ReturnAllObjectsToPool();
        }

        public override bool HasEffects()
        {
            return effects.Length > 0;
        }
    }
}