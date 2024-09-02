using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Collision trigger for 3D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/3D Triggers/Impact Collision Trigger 3D", 0)]
    [DisallowMultipleComponent]
    public class ImpactCollisionTrigger3D : ImpactCollisionTriggerBase
    {
        private void OnCollisionEnter(Collision collision)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionEnter", gameObject);
#endif
            if (!canQueueCollision(collision.relativeVelocity))
                return;

            triggerCollisionCore(new ImpactCollision(collision));
        }
    }
}