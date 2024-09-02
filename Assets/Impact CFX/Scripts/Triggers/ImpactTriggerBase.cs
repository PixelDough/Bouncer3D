using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Base class for many of the built-in trigger components, holding shared properties and collision message processing logic.
    /// </summary>
    public abstract class ImpactTriggerBase : MonoBehaviour
    {
        [Tooltip("Is the trigger enabled?")]
        [SerializeField]
        protected bool triggerEnabled = true;
        [Tooltip("The Impact Object the trigger will use.")]
        [SerializeField]
        private ImpactObjectBase impactObject;

        [Tooltip("An optional threshold that the collision velocity must be greater than to trigger an effect.")]
        [SerializeField]
        protected float threshold;
        [Tooltip("How contact points should be handled.")]
        [SerializeField]
        protected ImpactTriggerContactMode contactMode = ImpactTriggerContactMode.Single;
        [Tooltip("The number of materials to get for this trigger's object that triggered the collision.")]
        [SerializeField]
        [Range(1, 4)]
        protected int triggerMaterialCount = 1;
        [Tooltip("The number of materials to get for the object that was hit.")]
        [SerializeField]
        [Range(1, 4)]
        protected int hitMaterialCount = 1;
        [Tooltip("Behavior for collision normals.")]
        [SerializeField]
        protected CollisionNormalMode collisionNormalMode;

        [Tooltip("If enabled, this trigger will ignore collisions with objects with the same parent.")]
        [SerializeField]
        protected bool ignoreSameParents;
        [Tooltip("Parent to use for ignoring same parents. If not set, the topmost root parent of the hierarchy will be used.")]
        [SerializeField]
        protected Transform rootParent;

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
        /// An optional threshold that the collision velocity must be greater than to trigger an effect.
        /// </summary>
        public float Threshold { get => threshold; set => threshold = value; }

        /// <summary>
        /// How contact points should be handled.
        /// </summary>
        public ImpactTriggerContactMode ContactMode { get => contactMode; set => contactMode = value; }

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

        protected void triggerCollisionCore(ImpactCollision collision)
        {
            if (contactMode == ImpactTriggerContactMode.Single || contactMode == ImpactTriggerContactMode.Average)
                triggerCollisionSingle(collision);
        }

        protected void triggerCollisionSingle(ImpactCollision collision)
        {
            ImpactContactPoint firstContactPoint = collision.GetContact(0);

            if (contactMode == ImpactTriggerContactMode.Average)
            {
                for (int i = 1; i < collision.ContactCount; i++)
                {
                    ImpactContactPoint c = collision.GetContact(i);
                    firstContactPoint.Point += c.Point;
                    firstContactPoint.Normal += c.Normal;
                }

                firstContactPoint.Point /= collision.ContactCount;
                firstContactPoint.Normal /= collision.ContactCount;
            }

            triggerCollisionContact(firstContactPoint);
        }

        protected void triggerCollisionContact(ImpactContactPoint contactPoint)
        {
            if (ignoreSameParents)
            {
                bool hasCommonParent = GameObjectExtensions.ShareParent(contactPoint.TriggerObject.transform, contactPoint.HitObject.transform, rootParent);
                if (hasCommonParent)
                    return;
            }

            IImpactObject hitObject = contactPoint.HitObject.GetComponentInParent<IImpactObject>();
            IImpactObject triggerObject = getImpactObject(contactPoint.TriggerObject);

            if (collisionNormalMode == CollisionNormalMode.Inverted)
                contactPoint.Normal = -contactPoint.Normal;

            finalizeCollisionTrigger(triggerObject, hitObject, contactPoint);
        }

        protected IImpactObject getImpactObject(GameObject collider)
        {
            if (impactObjectInternal != null)
                return impactObjectInternal;
            else
                return collider.GetComponentInParent<IImpactObject>();
        }

        protected virtual void finalizeCollisionTrigger(IImpactObject triggerObject, IImpactObject hitObject, ImpactContactPoint contactPoint)
        {

        }

        protected bool canQueue(CollisionType collisionType)
        {
            return ImpactCFXGlobal.CanQueueCollision(collisionType, triggerMaterialCount + hitMaterialCount);
        }
    }
}
