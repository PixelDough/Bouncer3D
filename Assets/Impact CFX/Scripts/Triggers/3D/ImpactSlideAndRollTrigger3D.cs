using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Slide and roll trigger for 3D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/3D Triggers/Impact Slide & Roll Trigger 3D", 0)]
    [DisallowMultipleComponent]
    public class ImpactSlideAndRollTrigger3D : ImpactSlideAndRollTriggerBase
    {
        private void OnCollisionStay(Collision collision)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionStay", gameObject);
#endif
            if (!canQueueSlideOrRoll(collision.relativeVelocity))
                return;

            triggerCollisionCore(new ImpactCollision(collision));
        }
    }
}