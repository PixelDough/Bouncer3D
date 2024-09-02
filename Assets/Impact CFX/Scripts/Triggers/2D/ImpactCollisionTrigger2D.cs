using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Collision trigger for 2D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/2D Triggers/Impact Collision Trigger 2D", 0)]
    [DisallowMultipleComponent]
    public class ImpactCollisionTrigger2D : ImpactCollisionTriggerBase
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionEnter2D", gameObject);
#endif
            if (!canQueueCollision(collision.relativeVelocity))
                return;

            triggerCollisionCore(new ImpactCollision(collision));
        }
    }
}