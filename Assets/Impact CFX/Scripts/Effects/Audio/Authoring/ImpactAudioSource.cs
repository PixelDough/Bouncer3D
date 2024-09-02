using UnityEngine;

namespace ImpactCFX.Audio
{
    [AddComponentMenu("Impact CFX/Audio/Impact Audio Source")]
    public class ImpactAudioSource : ImpactAudioSourceBase
    {
        [SerializeField]
        protected AudioSource audioSource;

        protected float baseVolume, basePitch;

        protected virtual void Reset()
        {
            audioSource = GetComponentInChildren<AudioSource>();
        }

        protected virtual void Awake()
        {
            baseVolume = audioSource.volume;
            basePitch = audioSource.pitch;
        }

        protected override void playAudio(AudioClip audioClip, float volume, float pitch, bool loop, CollisionResultData collisionResultData)
        {
            audioSource.enabled = true;
            audioSource.loop = loop;
            audioSource.clip = audioClip;
            audioSource.volume = baseVolume * volume;
            audioSource.pitch = basePitch * pitch;

            if (loop)
                audioSource.timeSamples = Random.Range(0, audioClip.samples - 1);
            else
                audioSource.timeSamples = 1;

            audioSource.Play();

        }

        protected override void updateAudio(float volume, float pitch, CollisionResultData collisionResultData)
        {
            audioSource.volume = baseVolume * volume;
            audioSource.pitch = basePitch * pitch;
        }

        protected override void stopAudio()
        {
            audioSource.Stop();
        }

        protected override bool isPlaying()
        {
            return audioSource.isPlaying;
        }
    }
}