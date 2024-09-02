using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Whether a particle is a one-shot or looped effect.
    /// </summary>
    public enum ParticleEffectType
    {
        /// <summary>
        /// The particles are a one-shot effect.
        /// </summary>
        [Tooltip("The particles are a one-shot effect.")]
        OneShot = 0,
        /// <summary>
        /// The particles are a looping effect.
        /// </summary>
        [Tooltip("The particles are a looping effect.")]
        Looped = 1
    }
}