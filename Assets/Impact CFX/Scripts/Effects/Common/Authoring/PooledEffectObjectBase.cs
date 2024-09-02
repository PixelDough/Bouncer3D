using ImpactCFX.Pooling;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Base class for an effect object (such as an audio source, particle system, or decal) that is part of on object pool.
    /// </summary>
    public abstract class PooledEffectObjectBase : MonoBehaviour, IPoolable
    {
        [SerializeField]
        [Tooltip("Behavior for attaching this effect to one of the colliding objects that triggered the effect.")]
        private EffectAttachMode effectAttachMode;
        [SerializeField]
        [Tooltip("Configuration for this object's object pool.")]
        private ObjectPoolConfig objectPoolConfig;
        [SerializeField]
        [Tooltip("How the ID for this object's object pool is determined.")]
        private ObjectID poolID;

        /// <summary>
        /// Object pool configuration for this object when used as a template.
        /// </summary>
        public ObjectPoolConfig ObjectPoolConfig => objectPoolConfig;

        /// <summary>
        /// Behavior for attaching this effect to one of the colliding objects that triggered the effect.
        /// </summary>
        public EffectAttachMode EffectAttachMode { get => effectAttachMode; set => effectAttachMode = value; }

        private GameObject parentPoolGameObject;

        public int LastRetrievedFrame { get; protected set; }
        public float Priority { get; protected set; }
        public long ContactPointID { get; protected set; }

        public bool NeedsUpdate { get; protected set; }

        private bool isAvailable;
        private ImpactAttachedEffectManager impactAttachedEffectManager;

        /// <summary>
        /// Gets an ID for this object's object pool.
        /// </summary>
        public virtual int GetPoolID()
        {
            return poolID.GetIDForObject(this);
        }

        /// <summary>
        /// Sets this object's parent pool so that it can be returned to it.
        /// </summary>
        /// <param name="parentPoolGameObject">The game object for the object pool.</param>
        public virtual void SetParentPoolGameObject(GameObject parentPoolGameObject)
        {
            this.parentPoolGameObject = parentPoolGameObject;
        }

        public virtual bool IsAvailable()
        {
            return isAvailable;
        }

        public virtual void RetrieveFromPool(float priority, long contactPointID)
        {
            ContactPointID = contactPointID;
            LastRetrievedFrame = Time.frameCount;
            Priority = priority;

            gameObject.SetActive(true);

            NeedsUpdate = true;
            isAvailable = false;
        }

        /// <summary>
        /// Run any necessary update logic for the pooled object, such as checking if it should be returned to its pool.
        /// </summary>
        public virtual void UpdatePooledObject()
        {

        }

        public virtual void ReturnToPool()
        {
            ContactPointID = 0;
            gameObject.SetActive(false);
            NeedsUpdate = false;
            isAvailable = true;

            detach();
        }

        protected void attach(CollisionResultData collisionResultData)
        {
            detach();

            if (effectAttachMode == EffectAttachMode.None)
                return;

            GameObject attachTo = null;
            if (effectAttachMode == EffectAttachMode.TriggerObject)
            {
                attachTo = collisionResultData.TriggerObject;
            }
            else if (effectAttachMode == EffectAttachMode.HitObject)
            {
                attachTo = collisionResultData.HitObject;
            }

            if (attachTo != null)
            {
                impactAttachedEffectManager = attachTo.GetOrAddComponent<ImpactAttachedEffectManager>(true);
                impactAttachedEffectManager.AddAttachedEffect(this);

                transform.SetParent(attachTo.transform);
            }
        }

        protected void detach()
        {
            if (impactAttachedEffectManager != null && !impactAttachedEffectManager.Equals(null))
            {
                impactAttachedEffectManager.RemoveAttachedEffect(this);
            }

            impactAttachedEffectManager = null;
            transform.SetParent(parentPoolGameObject.transform);
        }

        public virtual void Destroy()
        {
            ReturnToPool();
            Destroy(this.gameObject);
        }
    }
}