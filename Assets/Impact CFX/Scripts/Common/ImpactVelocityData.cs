using Unity.Mathematics;

namespace ImpactCFX
{
    /// <summary>
    /// Holds data suitable for jobs about the impact velocity at a collision point.
    /// </summary>
    public struct ImpactVelocityData
    {
        /// <summary>
        /// The velocity of the collision.
        /// </summary>
        public float3 ImpactVelocity;
    }
}
