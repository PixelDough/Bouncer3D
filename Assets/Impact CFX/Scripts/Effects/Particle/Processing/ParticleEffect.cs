using Unity.Mathematics;

namespace ImpactCFX.Particles
{
    public struct ParticleEffect : IPooledEffectData<ParticleEffectResult>
    {
        public int EffectID { get; set; }
        public int TemplateID { get; set; }

        public ImpactTagMaskFilter IncludeTags { get; set; }
        public ImpactTagMaskFilter ExcludeTags { get; set; }

        public float MinimumVelocity;
        public float CollisionNormalInfluence;


        public ParticleEffectType ParticleEffectType;

        public bool EmitOnSlide;
        public bool EmitOnRoll;

        public ParticleEffectResult GetResult(CollisionInputData collisionData, MaterialCompositionData materialCompositionData, ImpactVelocityData velocityData, ref Random random)
        {
            ParticleEffectResult result = new ParticleEffectResult();
            float intensity = EffectUtility.GetCollisionIntensity(velocityData.ImpactVelocity, collisionData.Normal, CollisionNormalInfluence, collisionData.CollisionType) * materialCompositionData.Composition;

            if (intensity < MinimumVelocity)
            {
#if IMPACTCFX_DEBUG
                ImpactCFXLogger.LogEffectInvalid(GetType(), EffectID, $"Intensity ({intensity}) is less than Minimum Velocity ({MinimumVelocity})");
#endif          
                return result;
            }

            if (shouldEmit(collisionData.CollisionType))
            {
                result.TemplateID = TemplateID;
                result.Priority = collisionData.Priority;
                result.ContactPointID = ContactPointIDGenerator.CalculateContactPointID(collisionData.TriggerObjectID, collisionData.HitObjectID, collisionData.CollisionType, materialCompositionData.MaterialData.MaterialTags.Value, EffectID);
                result.CheckContactPointID = collisionData.CollisionType.RequiresContactPointID();

                result.IsEffectValid = result.IsObjectPoolRequestValid = true;
            }
            else
            {
#if IMPACTCFX_DEBUG
                ImpactCFXLogger.LogEffectInvalid(GetType(), EffectID, $"Particles not emitted for {collisionData.CollisionType}");
#endif    
            }
            return result;
        }

        private bool shouldEmit(CollisionType collisionType)
        {
            return (collisionType == CollisionType.Collision && ParticleEffectType == ParticleEffectType.OneShot) ||
                (collisionType == CollisionType.Slide && ParticleEffectType == ParticleEffectType.Looped && EmitOnSlide) ||
                (collisionType == CollisionType.Roll && ParticleEffectType == ParticleEffectType.Looped && EmitOnRoll);
        }
    }
}