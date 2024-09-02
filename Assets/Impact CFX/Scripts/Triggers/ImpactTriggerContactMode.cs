using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Specifies the different contact point modes for collision-based triggers.
    /// </summary>
    public enum ImpactTriggerContactMode
    {
        [Tooltip("Play interactions at the first contact point. This will only trigger 1 effect.")]
        /// <summary>
        /// Use only the first contact point.
        /// </summary>
        Single = 0,

        [Tooltip("Calculate the average of all contact points. This will only trigger 1 effect.")]
        /// <summary>
        /// Calculate the average of all contact points.
        /// </summary>
        Average = 1,
    }
}
