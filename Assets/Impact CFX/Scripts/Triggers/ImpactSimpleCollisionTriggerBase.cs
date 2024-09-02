using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Base class for triggers that trigger simple collision events (i.e. triggers that do not recieve collision data in their collision messages)
    /// </summary>
    public abstract class ImpactSimpleCollisionTriggerBase : ImpactTriggerBase
    {
        public enum SimpleCollisionVelocityCalculationMethod
        {
            [Tooltip("Combine the velocities of the colliding objects.")]
            RelativeVelocities = 0,
            [Tooltip("Measure the change in velocity of the colliding object.")]
            ChangeInVelocity = 1
        }

        [SerializeField]
        [Tooltip("The method to use to calculate the collision velocity.")]
        protected SimpleCollisionVelocityCalculationMethod collisionVelocityMethod;
        [SerializeField]
        [Tooltip("An optional cooldown between collisions.")]
        private float cooldown;

        /// <summary>
        /// The method to use to calculate the collision velocity.
        /// </summary>
        public SimpleCollisionVelocityCalculationMethod SimpleCollisionVelocityMethod { get => collisionVelocityMethod; set => collisionVelocityMethod = value; }

        /// <summary>
        /// An optional cooldown between collisions.
        /// </summary>
        public float Cooldown { get => cooldown; set => cooldown = value; }

        private float lastCollisionTime;

        protected bool canQueueSimpleCollision()
        {
#if IMPACTCFX_DEBUG
            if (!triggerEnabled)
                ImpactCFXLogger.LogTriggerAbort(GetType(), "Trigger not enabled", gameObject);
            else if (!checkCooldown())
                ImpactCFXLogger.LogTriggerAbort(GetType(), "Cooldown active", gameObject);
#endif
            return triggerEnabled && base.canQueue(CollisionType.Collision) && checkCooldown();
        }

        protected bool checkCooldown()
        {
            return Time.time - lastCollisionTime > cooldown;
        }

        protected void setLastCollisionTime(float time)
        {
            lastCollisionTime = time;
        }

        protected CollisionVelocityMethod getCollisionVelocityMethod()
        {
            return (CollisionVelocityMethod)collisionVelocityMethod;
        }
    }
}