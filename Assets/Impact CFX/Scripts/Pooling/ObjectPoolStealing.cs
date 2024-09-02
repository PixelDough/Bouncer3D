using UnityEngine;

namespace ImpactCFX.Pooling
{
    /// <summary>
    /// Defines how objects should be "stolen" from a pool if no objects are available.
    /// </summary>
    public enum ObjectPoolStealing
    {
        /// <summary>
        /// Do not steal any objects.
        /// </summary>
        [Tooltip("Do not steal any objects.")]
        None = 0,
        /// <summary>
        /// Steal objects with a lower priority.
        /// </summary>
        [Tooltip("Steal objects with a lower priority.")]
        LowerPriority = 1,
        /// <summary>
        /// Steal objects are the oldest, that is, have been retrieved least recently.
        /// </summary>
        [Tooltip("Steal objects are the oldest, that is, have been retrieved least recently.")]
        Oldest = 2,
    }
}

