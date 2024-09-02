using UnityEngine;

namespace ImpactCFX.Audio
{
    public enum CollisionAudioMode
    {
        [Tooltip("Audio clips will be selected based on the collision velocity, with the first element being the lowest velocity and the last element being the highest velocity.")]
        Velocity,
        [Tooltip("Audio clips will be selected at random.")]
        Random
    }
}
