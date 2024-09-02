using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Slide and roll trigger for 2D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/2D Triggers/Impact Slide & Roll Trigger 2D", 0)]
    [DisallowMultipleComponent]
    public class ImpactSlideAndRollTrigger2D : ImpactSlideAndRollTriggerBase
    {
        private void OnCollisionStay2D(Collision2D collision)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionStay2D", gameObject);
#endif
            if (!canQueueSlideOrRoll(collision.relativeVelocity))
                return;

            triggerCollisionCore(new ImpactCollision(collision));
        }
    }
}