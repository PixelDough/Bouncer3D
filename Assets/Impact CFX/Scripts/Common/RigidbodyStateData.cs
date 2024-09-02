using Unity.Mathematics;

namespace ImpactCFX
{
    /// <summary>
    /// Holds rigidbody state data in a form suitable for jobs.
    /// </summary>
    public struct RigidbodyStateData
    {
        public static RigidbodyData Default => new RigidbodyData();

        /// <summary>
        /// The velocity of the rigidbody.
        /// </summary>
        public float3 LinearVelocity;

        /// <summary>
        /// The angular velocity of the rigidbody.
        /// </summary>
        public float3 AngularVelocity;

        /// <summary>
        /// The rigidbody's center of mass in world space.
        /// </summary>
        public float3 CenterOfMass;

        public RigidbodyStateData(float3 linearVelocity, float3 angularVelocity, float3 worldCenterOfMass)
        {
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
            CenterOfMass = worldCenterOfMass;
        }
    }
}