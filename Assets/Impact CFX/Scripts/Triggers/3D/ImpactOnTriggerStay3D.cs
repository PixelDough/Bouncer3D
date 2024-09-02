using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// OnTriggerStay trigger for 3D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/3D Triggers/Impact On Trigger Stay 3D", 0)]
    [DisallowMultipleComponent]
    public class ImpactOnTriggerStay3D : ImpactOnTriggerStayBase
    {
        private void OnTriggerStay(Collider other)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnTriggerStay", gameObject);
#endif
            if (canQueueTriggerStay())
                return;

            ImpactContactPoint contactPoint = new ImpactContactPoint(other.transform.position, Vector3.zero, impactObjectInternal.GetGameObject(), other, Vector3.zero);
            trigger(contactPoint);
        }
    }
}

