using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// OnTriggerEnter trigger for 3D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/3D Triggers/Impact On Trigger Enter 3D", 0)]
    [DisallowMultipleComponent]
    public class ImpactOnTriggerEnter3D : ImpactOnTriggerEnterBase
    {
        private void OnTriggerEnter(Collider other)
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnTriggerEnter", gameObject);
#endif
            if (!canQueueTriggerEnter())
                return;

            ImpactContactPoint impactContactPoint = new ImpactContactPoint(other.transform.position, Vector3.zero, impactObjectInternal.GetGameObject(), other, Vector3.zero);
            trigger(impactContactPoint);
        }
    }
}