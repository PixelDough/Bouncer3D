using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Behavior for collision normals.
    /// </summary>
    public enum CollisionNormalMode
    {
        [Tooltip("Use the default normal from the contact point, facing away from the collision surface.")]
        /// <summary>
        /// Use the default normal from the contact point, facing away from the collision surface.
        /// </summary>
        Default = 0,
        [Tooltip("Invert the normal from the contact point, facing into the collision surface.")]
        /// <summary>
        /// Invert the normal from the contact point, facing into the collision surface.
        /// </summary>
        Inverted = 1,
    }
}
