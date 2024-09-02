using Unity.Mathematics;

namespace ImpactCFX.Audio
{
    public struct AudioEffect : IPooledEffectData<AudioEffectResult>
    {
        public int EffectID { get; set; }
        public int TemplateID { get; set; }

        public ImpactTagMaskFilter IncludeTags { get; set; }
        public ImpactTagMaskFilter ExcludeTags { get; set; }

        public Range VelocityReferenceRange;

        public bool ScaleVolumeWithVelocity;
        public float CollisionNormalInfluence;
        public float VelocityPitchInfluence;

        public Range RandomPitchRange;
        public Range RandomVolumeRange;


        public ArrayChunk CollisionAudioClipArrayChunk;
        public CollisionAudioMode CollisionAudioClipSelectionMode;

        public int SlideAudioClipIndex;
        public int RollAudioClipIndex;

        public AudioEffectResult GetResult(CollisionInputData collisionData, MaterialCompositionData materialCompositionData, ImpactVelocityData velocityData, ref Random random)
        {
            AudioEffectResult result = new AudioEffectResult();

            float intensity = EffectUtility.GetCollisionIntensity(velocityData.ImpactVelocity, collisionData.Normal, CollisionNormalInfluence, collisionData.CollisionType);

            if (intensity < VelocityReferenceRange.Min)
            {
#if IMPACTCFX_DEBUG
                ImpactCFXLogger.LogEffectInvalid(GetType(), EffectID, $"Intensity ({intensity}) is less than Velocity Reference Range Min ({VelocityReferenceRange.Min})");
#endif          
                return result;
            }

            float normalizedIntensity = VelocityReferenceRange.Normalize(intensity);

            result.TemplateID = TemplateID;
            result.Priority = collisionData.Priority;
            result.ContactPointID = ContactPointIDGenerator.CalculateContactPointID(collisionData.TriggerObjectID, collisionData.HitObjectID, collisionData.CollisionType, materialCompositionData.MaterialData.MaterialTags.Value, EffectID);
            result.CheckContactPointID = collisionData.CollisionType.RequiresContactPointID();

            result.AudioClipIndex = getAudioClipIndex(collisionData.CollisionType, normalizedIntensity, ref random);
            result.Volume = getVolume(normalizedIntensity) * materialCompositionData.Composition * random.NextFloat(RandomVolumeRange.Min, RandomVolumeRange.Max);
            result.Pitch = random.NextFloat(RandomPitchRange.Min, RandomPitchRange.Max);
            result.PitchVelocityAdd = math.length(velocityData.ImpactVelocity) * VelocityPitchInfluence;

#if IMPACTCFX_DEBUG
            if (result.TemplateID == 0)
                ImpactCFXLogger.LogEffectInvalid(GetType(), EffectID, "TemplateID is 0");
            else if (result.AudioClipIndex == -1)
                ImpactCFXLogger.LogEffectInvalid(GetType(), EffectID, "AudioClipIndex is -1");
            else if (result.Volume <= 0.01f)
                ImpactCFXLogger.LogEffectInvalid(GetType(), EffectID, "Volume is less than 0.01f");
#endif       

            if (result.TemplateID != 0 && result.AudioClipIndex > -1 && result.Volume > 0.01f)
            {
                result.IsEffectValid = result.IsObjectPoolRequestValid = true;
            }

            return result;
        }

        private int getAudioClipIndex(CollisionType collisionType, float normalizedIntensity, ref Random random)
        {
            if (collisionType == CollisionType.Collision)
                return getCollisionAudioClipIndex(normalizedIntensity, ref random);
            else if (collisionType == CollisionType.Slide)
                return SlideAudioClipIndex;
            else if (collisionType == CollisionType.Roll)
                return RollAudioClipIndex;

            return -1;
        }

        private int getCollisionAudioClipIndex(float normalizedIntensity, ref Random random)
        {
            if (CollisionAudioClipArrayChunk.Length == 0)
                return -1;

            if (CollisionAudioClipSelectionMode == CollisionAudioMode.Velocity)
            {
                int relativeIndex = (int)(math.clamp(normalizedIntensity, 0, 1) * (CollisionAudioClipArrayChunk.Length - 1));
                return CollisionAudioClipArrayChunk.Offset + relativeIndex;
            }
            else
            {
                return CollisionAudioClipArrayChunk.Offset + random.NextInt(0, CollisionAudioClipArrayChunk.Length);
            }
        }

        private float getVolume(float normalizedIntensity)
        {
            if (ScaleVolumeWithVelocity)
            {
                return normalizedIntensity;
            }
            else
            {
                return 1;
            }
        }
    }
}

