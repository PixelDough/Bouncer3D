using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Implementation of an Impact Object for objects with a 3D or 2D rigidbody.
    /// </summary>
    [AddComponentMenu("Impact CFX/Objects/Impact Object Rigidbody")]
    [DisallowMultipleComponent]
    public class ImpactObjectRigidbody : ImpactObjectSingleMaterial
    {
        private RigidbodyContainer rigidbodyContainer;

        private void Awake()
        {
            rigidbodyContainer = new RigidbodyContainer(gameObject);
            rigidbodyContainer.SyncRigidbodyData();
        }

        private void FixedUpdate()
        {
            rigidbodyContainer.SyncRigidbodyData();
        }

        public override RigidbodyData GetRigidbodyData()
        {
            return rigidbodyContainer.GetRigidbodyData();
        }

        public override Vector3 GetContactPointRelativePosition(Vector3 point)
        {
            return rigidbodyContainer.GetContactPointRelativePosition(point);
        }
    }
}