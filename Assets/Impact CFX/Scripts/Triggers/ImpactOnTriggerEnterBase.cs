using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Base class for triggers based on the OnTriggerEnter message.
    /// </summary>
    public abstract class ImpactOnTriggerEnterBase : MonoBehaviour
    {
        [Tooltip("Is the trigger enabled?")]
        [SerializeField]
        protected bool triggerEnabled = true;
        [Tooltip("The Impact Object the trigger will use.")]
        [SerializeField]
        protected ImpactObjectBase impactObject;
        [SerializeField]
        [Tooltip("An optional cooldown between collisions.")]
        protected float cooldown;

        /// <summary>
        /// Is the trigger enabled?
        /// </summary>
        public bool TriggerEnabled { get => triggerEnabled; set => triggerEnabled = value; }

        /// <summary>
        /// The Impact Object the trigger will use to trigger effects.
        /// </summary>
        public IImpactObject ImpactObject
        {
            get
            {
                if (Application.isEditor)
                    return impactObject;
                else return impactObjectInternal;
            }
            set
            {
                if (Application.isEditor)
                {
                    if (value is ImpactObjectBase b)
                    {
                        impactObject = b;
                    }
                    else
                    {
                        Debug.LogError($"Cannot set ImpactObject to value {value} in the editor. {value} must inherit from ImpactObjectBase");
                    }
                }
                else
                {
                    impactObjectInternal = value;
                }
            }
        }

        /// <summary>
        /// An optional cooldown between collisions.
        /// </summary>
        public float Cooldown { get => cooldown; set => cooldown = value; }

        protected IImpactObject impactObjectInternal;
        private float lastCollisionTime;

        private void Reset()
        {
            impactObject = GetComponent<ImpactObjectBase>();
        }

        private void Awake()
        {
            if (impactObject != null)
                impactObjectInternal = impactObject;
        }

        protected void trigger(ImpactContactPoint contactPoint)
        {
            lastCollisionTime = Time.time;

            IImpactObject hitObject = contactPoint.HitObject.GetComponentInParent<IImpactObject>();
            ImpactCFXGlobal.QueueCollision(impactObject, hitObject, contactPoint, CollisionType.Collision, 1, 1, CollisionVelocityMethod.RelativeVelocities);
        }

        protected bool checkCooldown()
        {
            return Time.time - lastCollisionTime > cooldown;
        }

        protected bool canQueueTriggerEnter()
        {
#if IMPACTCFX_DEBUG
            if (!triggerEnabled)
                ImpactCFXLogger.LogTriggerAbort(GetType(), "Trigger not enabled", gameObject);
            else if (!checkCooldown())
                ImpactCFXLogger.LogTriggerAbort(GetType(), "Cooldown active", gameObject);
#endif
            return triggerEnabled && ImpactCFXGlobal.CanQueueCollision(CollisionType.Collision, 1) || !checkCooldown();
        }
    }
}