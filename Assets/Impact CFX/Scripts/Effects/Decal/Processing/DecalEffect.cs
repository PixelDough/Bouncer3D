using Unity.Mathematics;

namespace ImpactCFX.Decals
{
    public struct DecalEffect : IPooledEffectData<DecalEffectResult>
    {
        public int EffectID { get; set; }
        public int TemplateID { get; set; }

        public ImpactTagMaskFilter IncludeTags { get; set; }
        public ImpactTagMaskFilter ExcludeTags { get; set; }

        public float MinimumVelocity;
        public float CollisionNormalInfluence;


        public bool CreateOnCollision;

        public Range CreationInterval;
        public EffectIntervalType CreationIntervalType;
        public bool CreateOnSlide;
        public bool CreateOnRoll;

        public DecalEffectResult GetResult(CollisionInputData collisionData, MaterialCompositionData materialCompositionData, ImpactVelocityData velocityData, ref Random random)
        {
            DecalEffectResult result = new DecalEffectResult();

            float intensity = EffectUtility.GetCollisionIntensity(velocityData.ImpactVelocity, collisionData.Normal, CollisionNormalInfluence, collisionData.CollisionType) * materialCompositionData.Composition;

            if (intensity < MinimumVelocity)
            {
#if IMPACTCFX_DEBUG
                ImpactCFXLogger.LogEffectInvalid(GetType(), EffectID, $"Intensity ({intensity}) is less than Minimum Velocity ({MinimumVelocity})");
#endif          
                return result;
            }

            if (shouldPlace(collisionData.CollisionType))
            {
                result.TemplateID = TemplateID;
                result.Priority = collisionData.Priority;
                result.ContactPointID = ContactPointIDGenerator.CalculateContactPointID(collisionData.TriggerObjectID, collisionData.HitObjectID, collisionData.CollisionType, materialCompositionData.MaterialData.MaterialTags.Value, EffectID);
                result.CheckContactPointID = collisionData.CollisionType.RequiresContactPointID();

                result.CreationInterval = CreationInterval;
                result.CreationIntervalType = CreationIntervalType;

                result.IsEffectValid = result.IsObjectPoolRequestValid = true;
            }
            else
            {
#if IMPACTCFX_DEBUG
                ImpactCFXLogger.LogEffectInvalid(GetType(), EffectID, $"Decals not placed for {collisionData.CollisionType}");
#endif    
            }

            return result;
        }

        private bool shouldPlace(CollisionType collisionType)
        {
            return (collisionType == CollisionType.Collision && CreateOnCollision) ||
                (collisionType == CollisionType.Slide && CreateOnSlide) ||
                (collisionType == CollisionType.Roll && CreateOnRoll);
        }
    }
}