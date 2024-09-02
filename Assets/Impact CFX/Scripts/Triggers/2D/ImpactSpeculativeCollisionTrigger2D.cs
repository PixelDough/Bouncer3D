using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Speculative collision trigger for 2D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/2D Triggers/Impact Speculative Collision Trigger 2D", 0)]
    [DisallowMultipleComponent]
    public class ImpactSpeculativeCollisionTrigger2D : ImpactSpeculativeCollisionTriggerBase
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionEnter2D", gameObject);
#endif
            if (!canQueueCollision(collision.relativeVelocity))
                return;

            triggerSpeculativeCollision(new ImpactCollision(collision));
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionStay2D", gameObject);
#endif
            if (!canQueueCollision(collision.relativeVelocity))
                return;

            triggerSpeculativeCollision(new ImpactCollision(collision));
        }
    }
}