using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// OnTriggerEnter trigger for 2D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/2D Triggers/Impact On Trigger Enter 2D", 0)]
    [DisallowMultipleComponent]
    public class ImpactOnTriggerEnter2D : ImpactOnTriggerEnterBase
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnTriggerEnter2D", gameObject);
#endif
            if (!canQueueTriggerEnter())
                return;

            ImpactContactPoint impactContactPoint = new ImpactContactPoint(other.transform.position, Vector3.zero, impactObjectInternal.GetGameObject(), other, Vector3.zero);
            trigger(impactContactPoint);
        }
    }
}