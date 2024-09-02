using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Speculative collision trigger for 3D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/3D Triggers/Impact Speculative Collision Trigger 3D", 0)]
    [DisallowMultipleComponent]
    public class ImpactSpeculativeCollisionTrigger3D : ImpactSpeculativeCollisionTriggerBase
    {
        private void OnCollisionEnter(Collision collision)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionEnter", gameObject);
#endif
            if (!canQueueCollision(collision.relativeVelocity))
                return;

            triggerSpeculativeCollision(new ImpactCollision(collision));
        }

        private void OnCollisionStay(Collision collision)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionStay", gameObject);
#endif
            if (!canQueueCollision(collision.relativeVelocity))
                return;

            triggerSpeculativeCollision(new ImpactCollision(collision));
        }
    }
}