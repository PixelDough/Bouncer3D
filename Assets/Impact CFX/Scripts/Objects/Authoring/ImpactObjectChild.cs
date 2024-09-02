using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Implementation of an Impact Object for objects that are children of a parent object.
    /// </summary>
    [AddComponentMenu("Impact CFX/Objects/Impact Object Child")]
    [DisallowMultipleComponent]
    public class ImpactObjectChild : ImpactObjectSingleMaterial
    {
        private IImpactObject parent;

        private void Awake()
        {
            parent = transform.parent.GetComponentInParent<ImpactObjectBase>();
        }

        private void Reset()
        {
            parent = transform.parent.GetComponentInParent<ImpactObjectBase>();
        }

        public override RigidbodyData GetRigidbodyData()
        {
            return parent.GetRigidbodyData();
        }

        public override Vector3 GetContactPointRelativePosition(Vector3 point)
        {
            return parent.GetContactPointRelativePosition(point);
        }
    }
}