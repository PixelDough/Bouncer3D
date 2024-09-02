using UnityEngine;

namespace ImpactCFX.Audio
{
    /// <summary>
    /// Base class for audio sources used by the built-in audio effect.
    /// </summary>
    public abstract class ImpactAudioSourceBase : PooledEffectObjectBase
    {
        private float targetVolume;
        private float targetPitch;

        private float currentVolume;
        private float currentPitch;

        private float originalPitch;

        private CollisionResultData collisionResultData;

        [SerializeField]
        [Tooltip("How long (in seconds) audio will fade out when sliding and rolling effects end.")]
        private float audioFadeOutTime = 0.2f;

        /// <summary>
        /// Plays the given audio clip with the given parameters for a collision.
        /// </summary>
        /// <param name="audioClip">The audio clip to play.</param>
        /// <param name="volume">The volume to play at.</param>
        /// <param name="pitch">The pitch to play at.</param>
        /// <param name="collisionResultData">Collision data holding important information like the collision position.</param>
        public void PlayAudioSourceBase(AudioClip audioClip, float volume, float pitch, CollisionResultData collisionResultData)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogEffectPlay(GetType(), collisionResultData, $"AudioClip = {audioClip.name}), Volume = {volume}, Pitch = {pitch}");
#endif    

            this.collisionResultData = collisionResultData;

            targetVolume = currentVolume = volume;
            originalPitch = targetPitch = currentPitch = pitch;

            transform.position = collisionResultData.Point;

            if (collisionResultData.CollisionType == CollisionType.Collision)
                attach(collisionResultData);

            playAudio(audioClip, volume, pitch, collisionResultData.CollisionType.IsLooping(), collisionResultData);
        }

        /// <summary>
        /// Updates the audio source for sliding and rolling.
        /// </summary>
        /// <param name="audioClip">The audio clip to play, in case the audio needs to be started again.</param>
        /// <param name="volume">The new volume.</param>
        /// <param name="pitch">The pitch to play at, in case the audio needs to be started again.</param>
        /// <param name="pitchAdd">The amount to add to the pitch.</param>
        /// <param name="collisionResultData">Updated collision data holding important information like the collision position.</param>
        public void UpdateAudioSourceBaseCollision(AudioClip audioClip, float volume, float pitch, float pitchAdd, CollisionResultData collisionResultData)
        {
#if IMPACTCFX_DEBUG
    ImpactCFXLogger.LogEffectUpdate(GetType(), collisionResultData, $"Volume = {volume}, PitchAdd = {pitchAdd}");
#endif
            //Rare case if audio is trying to be updated but is not actually playing.
            if (!isPlaying())
            {
                PlayAudioSourceBase(audioClip, volume, pitch, collisionResultData);
            }

            this.collisionResultData = collisionResultData;

            targetVolume = volume;
            targetPitch = originalPitch + pitchAdd;

            transform.position = collisionResultData.Point;
        }

        /// <summary>
        /// Updates the audio source and returns it to its pool if it has stopped playing or the volume is 0.
        /// </summary>
        public override void UpdatePooledObject()
        {
            if (!isPlaying() || currentVolume < 0.01f)
            {
                ReturnToPool();
            }
            else
            {
                updateAudio(currentVolume, currentPitch, collisionResultData);

                currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, Time.deltaTime / audioFadeOutTime);
                currentPitch = Mathf.MoveTowards(currentPitch, targetPitch, Time.deltaTime / audioFadeOutTime);

                if (collisionResultData.CollisionType.IsLooping())
                {
                    targetVolume = 0;
                }
            }
        }

        /// <summary>
        /// Play the given audio clip with the given parameters.
        /// </summary>
        /// <param name="audioClip">The audio clip to play.</param>
        /// <param name="volume">The volume to play at.</param>
        /// <param name="pitch">The pitch to play at.</param>
        /// <param name="loop">Should the audio be looped?</param>
        /// <param name="collisionResultData">Collision result data for more in-depth processing, if needed.</param>
        protected abstract void playAudio(AudioClip audioClip, float volume, float pitch, bool loop, CollisionResultData collisionResultData);

        /// <summary>
        /// Update the audio source for sliding and rolling.
        /// </summary>
        /// <param name="volume">The new volume.</param>
        /// <param name="pitch">The new pitch.</param>
        /// <param name="collisionResultData">Collision result data for more in-depth processing, if needed.</param>
        protected abstract void updateAudio(float volume, float pitch, CollisionResultData collisionResultData);

        /// <summary>
        /// Stop the audio.
        /// </summary>
        protected abstract void stopAudio();

        /// <summary>
        /// Check if the audio is currently playing.
        /// </summary>
        protected abstract bool isPlaying();

        public override void ReturnToPool()
        {
            stopAudio();

            base.ReturnToPool();
        }
    }
}
