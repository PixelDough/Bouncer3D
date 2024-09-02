using UnityEngine;

namespace ImpactCFX
{
    public enum CollisionVelocityMethod
    {
        [Tooltip("Combine the velocities of the colliding objects.")]
        RelativeVelocities = 0,
        [Tooltip("Measure the change in velocity of the colliding object.")]
        ChangeInVelocity = 1,
        [Tooltip("Use the relative velocity provided by the collision message.")]
        CollisionMessage = 2
    }
}
