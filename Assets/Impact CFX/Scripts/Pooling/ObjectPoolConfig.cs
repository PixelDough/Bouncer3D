using System;
using UnityEngine;

namespace ImpactCFX.Pooling
{
    /// <summary>
    /// Holds common object pool configuration data.
    /// </summary>
    [Serializable]
    public class ObjectPoolConfig
    {
        /// <summary>
        /// The size of the pool.
        /// </summary>
        [Tooltip("The size of the object pool.")]
        public int PoolSize = 32;
        /// <summary>
        /// How the object pool will handle stealing already active objects, if all objects are in use.
        /// </summary>
        [Tooltip("How the object pool will handle stealing already active objects, if all objects are in use.")]
        public ObjectPoolStealing Stealing = ObjectPoolStealing.None;
    }
}
