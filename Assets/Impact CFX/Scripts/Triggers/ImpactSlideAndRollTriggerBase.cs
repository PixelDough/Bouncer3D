using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Base class for triggers that trigger slide and roll collision events.
    /// </summary>
    public class ImpactSlideAndRollTriggerBase : ImpactTriggerBase
    {
        public enum SlideVelocityCalculationMethod
        {
            [Tooltip("Combine the velocities of the colliding objects.")]
            RelativeVelocities = 0,
            [Tooltip("Use the relative velocity provided by the collision message.")]
            CollisionMessage = 2
        }

        [Tooltip("Are sliding effects enabled?")]
        [SerializeField]
        private bool enableSliding = true;
        [Tooltip("Are rolling effects enabled?")]
        [SerializeField]
        private bool enableRolling = true;

        [SerializeField]
        [Tooltip("The method to use to calculate the collision velocity.")]
        private SlideVelocityCalculationMethod slideVelocityMethod;

        /// <summary>
        /// The method to use to calculate the collision velocity.
        /// </summary>
        public SlideVelocityCalculationMethod SlideVelocityMethod { get => slideVelocityMethod; set => slideVelocityMethod = value; }

        /// <summary>
        /// Are sliding effects enabled?
        /// </summary>
        public bool EnableSliding { get => enableSliding; set => enableSliding = value; }

        /// <summary>
        /// Are rolling effects enabled?
        /// </summary>
        public bool EnableRolling { get => enableRolling; set => enableRolling = value; }

        protected override void finalizeCollisionTrigger(IImpactObject triggerObject, IImpactObject hitObject, ImpactContactPoint contactPoint)
        {
            if (enableSliding)
                ImpactCFXGlobal.QueueCollision(triggerObject, hitObject, contactPoint, CollisionType.Slide, triggerMaterialCount, hitMaterialCount, getCollisionVelocityMethod());
            if (enableRolling)
                ImpactCFXGlobal.QueueCollision(triggerObject, hitObject, contactPoint, CollisionType.Roll, triggerMaterialCount, hitMaterialCount, getCollisionVelocityMethod());
        }

        private CollisionVelocityMethod getCollisionVelocityMethod()
        {
            return (CollisionVelocityMethod)slideVelocityMethod;
        }

        protected bool canQueueSlideOrRoll(Vector3 relativeVelocity)
        {
#if IMPACTCFX_DEBUG
            if (!triggerEnabled)
                ImpactCFXLogger.LogTriggerAbort(GetType(), "Trigger not enabled", gameObject);
            else if (relativeVelocity.sqrMagnitude < threshold)
                ImpactCFXLogger.LogTriggerAbort(GetType(), $"{relativeVelocity.sqrMagnitude} < {threshold} threshold", gameObject);
#endif
            bool canQueueSlidingOrRolling = base.canQueue(CollisionType.Slide) || base.canQueue(CollisionType.Roll);
            return triggerEnabled && canQueueSlidingOrRolling && relativeVelocity.sqrMagnitude >= threshold;
        }
    }
}