using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// The type of interval for effects like particles and decals when placing for sliding and rolling.
    /// </summary>
    public enum EffectIntervalType
    {
        /// <summary>
        /// Interval is based on time.
        /// </summary>
        [Tooltip("Interval is based on time.")]
        Time = 0,
        /// <summary>
        /// Interval is based on distance.
        /// </summary>
        [Tooltip("Interval is based on distance.")]
        Distance = 1
    }
}
