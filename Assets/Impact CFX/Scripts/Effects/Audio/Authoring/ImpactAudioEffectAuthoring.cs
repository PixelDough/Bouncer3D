using System.Collections.Generic;
using UnityEngine;

namespace ImpactCFX.Audio
{
    [CreateAssetMenu(fileName = "New Impact Audio Effect", menuName = "Impact CFX/Audio Effect", order = 2)]
    public class ImpactAudioEffectAuthoring : ImpactPooledEffectAuthoringBase
    {
        [Tooltip("The velocity magnitude range to use when calculating collision intensity.")]
        [SerializeField]
        private Range velocityReferenceRange = new Range(1, 10);

        [Tooltip("Should volume be scaled based on the velocity?")]
        [SerializeField]
        private bool scaleVolumeWithVelocity = true;
        [Tooltip("How much the normal should affect the collision intensity.")]
        [SerializeField]
        private float collisionNormalInfluence = 1;
        [Tooltip("How much to increase the pitch as sliding and rolling velocity increases.")]
        [SerializeField]
        private float velocityPitchInfluence = 0.025f;

        [Tooltip("Random multiplier for the pitch.")]
        [SerializeField]
        private Range randomPitchRange = new Range(0.9f, 1.1f);
        [SerializeField]
        [Tooltip("Random multiplier for the volume.")]
        private Range randomVolumeRange = new Range(0.9f, 1.1f);

        [Tooltip("How collision audio clips are chosen from the audio clips list.")]
        [SerializeField]
        private CollisionAudioMode collisionClipSelectionMode;
        [Tooltip("Audio clips that will be used for collisions.")]
        [SerializeField]
        private List<AudioClip> collisionAudioClips = new List<AudioClip>();

        [Tooltip("The audio clip to play when sliding.")]
        [SerializeField]
        private AudioClip slideAudioClip;
        [Tooltip("The audio clip to play when rolling.")]
        [SerializeField]
        private AudioClip rollAudioClip;

        [Tooltip("The audio source that will be used when playing sounds from this interaction.")]
        [SerializeField]
        private ImpactAudioSourceBase audioSourceTemplate;

        public IReadOnlyList<AudioClip> CollisionAudioClips => collisionAudioClips.AsReadOnly();

        public AudioClip SlideAudioClip => slideAudioClip;
        public AudioClip RollAudioClip => rollAudioClip;

        public override bool Validate()
        {
            return audioSourceTemplate != null;
        }

        public AudioEffect GetAudioEffect()
        {
            return new AudioEffect()
            {
                VelocityReferenceRange = this.velocityReferenceRange,
                RandomVolumeRange = this.randomVolumeRange,
                RandomPitchRange = this.randomPitchRange,
                CollisionNormalInfluence = this.collisionNormalInfluence,
                ScaleVolumeWithVelocity = this.scaleVolumeWithVelocity,
                VelocityPitchInfluence = this.velocityPitchInfluence,
                CollisionAudioClipSelectionMode = this.collisionClipSelectionMode,
                CollisionAudioClipArrayChunk = ArrayChunk.Default,
                SlideAudioClipIndex = -1,
                RollAudioClipIndex = -1
            };
        }

        public override PooledEffectObjectBase GetTemplateObject()
        {
            return audioSourceTemplate;
        }
    }
}