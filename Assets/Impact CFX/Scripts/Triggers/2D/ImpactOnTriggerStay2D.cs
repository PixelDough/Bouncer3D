using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// OnTriggerStay trigger for 2D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/2D Triggers/Impact On Trigger Stay 2D", 0)]
    [DisallowMultipleComponent]
    public class ImpactOnTriggerStay2D : ImpactOnTriggerStayBase
    {
        private void OnTriggerStay2D(Collider2D other)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnTriggerStay2D", gameObject);
#endif
            if (canQueueTriggerStay())
                return;

            ImpactContactPoint contactPoint = new ImpactContactPoint(other.transform.position, Vector3.zero, impactObjectInternal.GetGameObject(), other, Vector3.zero);
            trigger(contactPoint);
        }
    }
}

