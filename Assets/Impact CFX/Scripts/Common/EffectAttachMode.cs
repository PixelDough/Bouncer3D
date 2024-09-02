using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Behavior for attaching effects to colliding objects.
    /// </summary>
    public enum EffectAttachMode
    {
        /// <summary>
        /// Don't attach to any objects.
        /// </summary>
        [Tooltip("Don't attach to any objects.")]
        None = 0,
        /// <summary>
        /// Attach the effect to the object that triggered the effect.
        /// </summary>
        [Tooltip("Attach the effect to the object that triggered the effect.")]
        TriggerObject = 1,
        /// <summary>
        /// Attach the effect to the object that was hit.
        /// </summary>
        [Tooltip("Attach the effect to the object that was hit.")]
        HitObject = 2
    }
}
