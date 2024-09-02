using System;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Mapping from a 3D physic material to an impact material.
    /// </summary>
    [Serializable]
    public class ImpactPhysicMaterialMapping
    {
        /// <summary>
        /// The PhysicMaterial to map from.
        /// </summary>
        [Tooltip("The PhysicMaterial to map from.")]
        public PhysicMaterial PhysicMaterial;
        /// <summary>
        /// The Impact Material to map to.
        /// </summary>
        [Tooltip("The Impact Material to map to.")]
        public ImpactMaterialAuthoring ImpactMaterial;
    }

    /// <summary>
    /// Mapping from a 2D physics material to an impact material.
    /// </summary>
    [Serializable]
    public class ImpactPhysicsMaterial2DMapping
    {
        /// <summary>
        /// The PhysicsMaterial2D to map from.
        /// </summary>
        [Tooltip("The PhysicsMaterial2D to map from.")]
        public PhysicsMaterial2D PhysicsMaterial2D;
        /// <summary>
        /// The Impact Material to map to.
        /// </summary>
        [Tooltip("The Impact Material to map to.")]
        public ImpactMaterialAuthoring ImpactMaterial;
    }
}
