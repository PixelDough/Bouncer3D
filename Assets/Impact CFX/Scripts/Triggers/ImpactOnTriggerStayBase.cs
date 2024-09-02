using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Base class for triggers based on the OnTriggerStay message.
    /// </summary>
    public abstract class ImpactOnTriggerStayBase : MonoBehaviour
    {
        [Tooltip("Is the trigger enabled?")]
        [SerializeField]
        protected bool triggerEnabled = true;
        [Tooltip("The Impact Object the trigger will use.")]
        [SerializeField]
        protected ImpactObjectBase impactObject;

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

        protected IImpactObject impactObjectInternal;

        private void Reset()
        {
            impactObject = GetComponent<ImpactObjectBase>();
        }

        private void Awake()
        {
            if (impactObject != null)
                impactObjectInternal = impactObject;
        }

        protected void trigger(ImpactContactPoint impactContactPoint)
        {
            IImpactObject hitObject = impactContactPoint.HitObject.GetComponentInParent<IImpactObject>();
            ImpactCFXGlobal.QueueCollision(impactObject, hitObject, impactContactPoint, CollisionType.Slide, 1, 1, CollisionVelocityMethod.RelativeVelocities);
        }

        protected bool canQueueTriggerStay()
        {
#if IMPACTCFX_DEBUG
            if (!triggerEnabled)
                ImpactCFXLogger.LogTriggerAbort(GetType(), "Trigger not enabled", gameObject);
#endif
            return triggerEnabled && ImpactCFXGlobal.CanQueueCollision(CollisionType.Slide, 1);
        }
    }
}

