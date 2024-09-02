using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Base class for triggers that trigger one-shot collision events.
    /// </summary>
    public abstract class ImpactCollisionTriggerBase : ImpactTriggerBase
    {
        [SerializeField]
        [Tooltip("The method to use to calculate the collision velocity.")]
        protected CollisionVelocityMethod collisionVelocityMethod;
        [SerializeField]
        [Tooltip("An optional cooldown between collisions.")]
        private float cooldown;

        /// <summary>
        /// The method to use to calculate the collision velocity.
        /// </summary>
        public CollisionVelocityMethod CollisionVelocityMethod { get => collisionVelocityMethod; set => collisionVelocityMethod = value; }

        /// <summary>
        /// An optional cooldown between collisions.
        /// </summary>
        public float Cooldown { get => cooldown; set => cooldown = value; }

        private float lastCollisionTime;

        protected bool checkCooldown()
        {
            return Time.time - lastCollisionTime > cooldown;
        }

        protected override void finalizeCollisionTrigger(IImpactObject triggerObject, IImpactObject hitObject, ImpactContactPoint contactPoint)
        {
            lastCollisionTime = Time.time;
            ImpactCFXGlobal.QueueCollision(triggerObject, hitObject, contactPoint, CollisionType.Collision, triggerMaterialCount, hitMaterialCount, collisionVelocityMethod);
        }

        protected bool canQueueCollision(Vector3 relativeVelocity)
        {
#if IMPACTCFX_DEBUG
            if (!triggerEnabled)
                ImpactCFXLogger.LogTriggerAbort(GetType(), "Trigger not enabled", gameObject);
            else if (!checkCooldown())
                ImpactCFXLogger.LogTriggerAbort(GetType(), "Cooldown active", gameObject);
            else if (relativeVelocity.sqrMagnitude < threshold)
                ImpactCFXLogger.LogTriggerAbort(GetType(), $"Velocity ({relativeVelocity.sqrMagnitude:F2}) is less than threshold ({threshold:F2})", gameObject);
#endif
            return triggerEnabled && base.canQueue(CollisionType.Collision) && checkCooldown() && relativeVelocity.sqrMagnitude >= threshold;
        }
    }
}