using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// A "cheap" 3D or 2D rigidbody that does not have FixedUpdate.
    /// </summary>
    [AddComponentMenu("Impact CFX/Objects/Impact Object Rigidbody (Cheap)")]
    [DisallowMultipleComponent]
    public class ImpactObjectRigidbodyCheap : ImpactObjectSingleMaterial
    {
        private RigidbodyContainer rigidbodyContainer;

        private void Awake()
        {
            rigidbodyContainer = new RigidbodyContainer(gameObject);
        }

        public override RigidbodyData GetRigidbodyData()
        {
            RigidbodyStateData currentState = rigidbodyContainer.GetCurrentRigidbodyState();
            return new RigidbodyData(currentState, currentState);
        }
    }
}