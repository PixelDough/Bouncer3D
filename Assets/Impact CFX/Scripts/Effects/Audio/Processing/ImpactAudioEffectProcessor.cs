using ImpactCFX.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace ImpactCFX.Audio
{
    [AddComponentMenu("Impact CFX/Audio/Impact Audio Effect Processor")]
    [DisallowMultipleComponent]
    public class ImpactAudioEffectProcessor : ImpactPooledEffectProcessor<ImpactAudioEffectAuthoring, AudioEffect, AudioEffectResult, AudioPool, ImpactAudioSourceBase>
    {
        private List<AudioClip> audioClips = new List<AudioClip>();

        protected override AudioEffect getEffectForPooledEffect(ImpactAudioEffectAuthoring effectAuthoring)
        {
            AudioEffect audioEffect = effectAuthoring.GetAudioEffect();

            ArrayChunk collisionClipsArrayChunk = new ArrayChunk();
            collisionClipsArrayChunk.Offset = audioClips.Count;

            foreach (AudioClip collisionAudioClip in effectAuthoring.CollisionAudioClips)
            {
                audioClips.Add(collisionAudioClip);
                collisionClipsArrayChunk.Length += 1;
            }
            audioEffect.CollisionAudioClipArrayChunk = collisionClipsArrayChunk;

            if (effectAuthoring.SlideAudioClip != null)
            {
                audioEffect.SlideAudioClipIndex = audioClips.Count;
                audioClips.Add(effectAuthoring.SlideAudioClip);
            }

            if (effectAuthoring.RollAudioClip != null)
            {
                audioEffect.RollAudioClipIndex = audioClips.Count;
                audioClips.Add(effectAuthoring.RollAudioClip);
            }

            return audioEffect;
        }

        protected override ImpactEffectProcessorJob<AudioEffect, AudioEffectResult> getEffectProcessorJobBase()
        {
            return new ImpactEffectProcessorJob<AudioEffect, AudioEffectResult>();
        }

        protected override ObjectPoolJob<AudioEffectResult> getObjectPoolJobBase()
        {
            return new ObjectPoolJob<AudioEffectResult>();
        }

        public override void PlayPooledEffect(
            AudioEffectResult effectResult,
            ImpactAudioSourceBase pooledObject,
            CollisionResultData collisionResultData)
        {
            base.PlayPooledEffect(effectResult, pooledObject, collisionResultData);

            AudioClip audioClip = audioClips[effectResult.AudioClipIndex];

            if (effectResult.IsUpdate)
            {
                pooledObject.UpdateAudioSourceBaseCollision(audioClip, effectResult.Volume, effectResult.Pitch, effectResult.PitchVelocityAdd, collisionResultData);
            }
            else
            {
                pooledObject.PlayAudioSourceBase(audioClip, effectResult.Volume, effectResult.Pitch, collisionResultData);
            }
        }
    }
}