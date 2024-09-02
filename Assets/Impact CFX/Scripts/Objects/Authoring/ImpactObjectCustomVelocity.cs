using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Implementation of an Impact Object that calculates a velocity using the position and rotation of the object.
    /// </summary>
    [AddComponentMenu("Impact CFX/Objects/Impact Object Custom Velocity")]
    [DisallowMultipleComponent]
    public class ImpactObjectCustomVelocity : ImpactObjectSingleMaterial
    {
        [SerializeField]
        [Tooltip("A scalar value for the velocity.")]
        private float velocityScale = 1;

        private RigidbodyStateData rigidbodyState;

        private Vector3 previousPosition;
        private Quaternion previousRotation;

        /// <summary>
        /// A scalar value for the velocity.
        /// </summary>
        public float VelocityScale { get => velocityScale; set => velocityScale = value; }

        private void FixedUpdate()
        {
            rigidbodyState.CenterOfMass = transform.position;

            rigidbodyState.LinearVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime * VelocityScale;
            rigidbodyState.AngularVelocity = (transform.rotation * Quaternion.Inverse(previousRotation)).eulerAngles / Time.fixedDeltaTime * VelocityScale;

            previousPosition = transform.position;
            previousRotation = transform.rotation;
        }

        public override RigidbodyData GetRigidbodyData()
        {
            return new RigidbodyData(rigidbodyState, rigidbodyState);
        }
    }
}

