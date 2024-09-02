using Unity.Mathematics;

namespace ImpactCFX
{
    /// <summary>
    /// Utility methods for physics calculations.
    /// </summary>
    public static class PhysicsUtility
    {
        /// <summary>
        /// Calculates the velocity of a point on a rigidbody.
        /// </summary>
        /// <param name="point">The point in world space.</param>
        /// <param name="angularVelocity">the angular velocity of the rigidbody.</param>
        /// <param name="centerOfRotation">The center of rotation of the rigidbody in world space.</param>
        public static float3 CalculateTangentialVelocity(float3 point, float3 angularVelocity, float3 centerOfRotation)
        {
            float3 p = point - centerOfRotation;
            float3 v = math.cross(angularVelocity, p);
            return v;
        }
    }
}