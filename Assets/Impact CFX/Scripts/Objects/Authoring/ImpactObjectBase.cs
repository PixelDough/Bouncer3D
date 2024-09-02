using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Base class for impact objects.
    /// </summary>
    public abstract class ImpactObjectBase : MonoBehaviour, IImpactObject
    {
        [SerializeField]
        [Range(0, 100)]
        [Tooltip("The priority of this object. Higher priority means effects will have higher priority.")]
        private int priority = 0;

        /// <summary>
        /// Gets a unique identifier for this object.
        /// </summary>
        public virtual int GetID()
        {
            return GetInstanceID();
        }

        /// <summary>
        /// Gets the rigidbody velocity data for this object.
        /// </summary>
        public abstract RigidbodyData GetRigidbodyData();

        /// <summary>
        /// Tells this object to register its material(s) with the Impact CFX Manager.
        /// </summary>
        public abstract void RegisterMaterials();

        /// <summary>
        /// Gets the priority for this object.
        /// </summary>
        public virtual int GetPriority()
        {
            return priority;
        }

        /// <summary>
        /// Gets the local position of the contact point relative to this object's transform.
        /// </summary>
        public virtual Vector3 GetContactPointRelativePosition(Vector3 point)
        {
            return transform.InverseTransformPoint(point);
        }

        /// <summary>
        /// Gets the game object assoicated with this object.
        /// </summary>
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public override string ToString()
        {
            return $"{gameObject.name} ({GetType().Name}) [{GetID()}]";
        }
    }
}

