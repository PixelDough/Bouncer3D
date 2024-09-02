using FMOD;
using Unity.Mathematics;

namespace ImpactCFX.FMOD
{
    public struct FMODAudioEffect : IEffectData<FMODAudioEffectResult>
    {
        public int EffectID { get; set; }

        public ImpactTagMaskFilter IncludeTags { get; set; }
        public ImpactTagMaskFilter ExcludeTags { get; set; }

        public Range VelocityReferenceRange;
        public float CollisionNormalInfluence;

        public GUID CollisionEventGUID;
        public GUID SlideEventGUID;
        public GUID RollEventGUID;

        public FMODAudioEffectResult GetResult(CollisionInputData collisionData, MaterialCompositionData materialCompositionData, ImpactVelocityData velocityData, ref Random random)
        {
            FMODAudioEffectResult result = new FMODAudioEffectResult();

            float intensity = EffectUtility.GetCollisionIntensity(velocityData.ImpactVelocity, collisionData.Normal, CollisionNormalInfluence, collisionData.CollisionType);

            if (intensity < VelocityReferenceRange.Min)
                return result;

            result.RawIntensity = intensity;

            float normalizedIntensity = VelocityReferenceRange.Normalize(intensity);
            result.NormalizedIntensity = normalizedIntensity;

            if (collisionData.CollisionType == CollisionType.Collision)
            {
                result.FMODEventGUID = CollisionEventGUID;
            }
            else if (collisionData.CollisionType == CollisionType.Slide)
            {
                result.FMODEventGUID = SlideEventGUID;
            }
            else if (collisionData.CollisionType == CollisionType.Roll)
            {
                result.FMODEventGUID = RollEventGUID;
            }

            result.TemplateID = 1;
            result.Priority = collisionData.Priority;
            result.ContactPointID = ContactPointIDGenerator.CalculateContactPointID(collisionData.TriggerObjectID, collisionData.HitObjectID, collisionData.CollisionType, materialCompositionData.MaterialData.MaterialTags.Value, EffectID);
            result.CheckContactPointID = collisionData.CollisionType.RequiresContactPointID();

            result.IsEffectValid = result.IsObjectPoolRequestValid = !result.FMODEventGUID.IsNull;

            return result;
        }
    }
}

