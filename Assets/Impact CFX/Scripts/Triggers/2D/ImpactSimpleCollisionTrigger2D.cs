using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Simple collision trigger for 2D physics.
    /// </summary>
    [AddComponentMenu("Impact CFX/2D Triggers/Impact Simple Collision Trigger 2D", 0)]
    [DisallowMultipleComponent]
    public class ImpactSimpleCollisionTrigger2D : ImpactSimpleCollisionTriggerBase
    {
        private void OnCollisionEnter2D()
        {
#if IMPACTCFX_DEBUG
            ImpactCFXLogger.LogTriggerInvoked(GetType(), "OnCollisionEnter2D", gameObject);
#endif
            if (!canQueueSimpleCollision())
                return;

            setLastCollisionTime(Time.time);

            ImpactContactPoint impactContactPoint = new ImpactContactPoint(transform.position, Vector3.zero, impactObjectInternal.GetGameObject(), PhysicsType.Physics2D);
            ImpactCFXGlobal.QueueCollision(impactObjectInternal, null, impactContactPoint, CollisionType.Collision, 1, 1, getCollisionVelocityMethod());
        }
    }
}