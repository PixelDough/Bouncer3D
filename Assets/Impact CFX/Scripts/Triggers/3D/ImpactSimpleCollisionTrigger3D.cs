using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Simple collision trigger for 3D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/3D Triggers/Impact Simple Collision Trigger 3D", 0)]
    [DisallowMultipleComponent]
    public class ImpactSimpleCollisionTrigger3D : ImpactSimpleCollisionTriggerBase
    {
        private void OnCollisionEnter()
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionEnter", gameObject);
#endif
            if (!canQueueSimpleCollision())
                return;

            setLastCollisionTime(Time.time);

            ImpactContactPoint impactContactPoint = new ImpactContactPoint(transform.position, Vector3.zero, impactObjectInternal.GetGameObject(), PhysicsType.Physics3D);
            ImpactCFXGlobal.QueueCollision(impactObjectInternal, null, impactContactPoint, CollisionType.Collision, 1, 1, getCollisionVelocityMethod());
        }
    }
}