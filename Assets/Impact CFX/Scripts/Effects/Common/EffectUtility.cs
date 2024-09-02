using Unity.Mathematics;

namespace ImpactCFX
{
    /// <summary>
    /// Utilities for effects.
    /// </summary>
    public static class EffectUtility
    {
        /// <summary>
        /// Gets an intensity value for the collision, taking into account the collision normal.
        /// </summary>
        /// <param name="velocity">The collision velocity.</param>
        /// <param name="normal">The collision normal.</param>
        /// <param name="collisionNormalInfluence">How much influence the collision normal has on the resulting intensity.</param>
        /// <param name="collisionType">The type of collision.</param>
        public static float GetCollisionIntensity(float3 velocity, float3 normal, float collisionNormalInfluence, CollisionType collisionType)
        {
            float dotProduct;
            float velocityMagnitude = math.length(velocity);

            if (math.lengthsq(normal) == 0)
                dotProduct = 1;
            else
            {
                float3 normalizedVelocity = velocityMagnitude == 0 ? float3.zero : velocity / velocityMagnitude;

                if (collisionType == CollisionType.Collision)
                    dotProduct = math.abs(math.dot(normalizedVelocity, normal));
                else
                    dotProduct = 1 - math.abs(math.dot(normalizedVelocity, normal));
            }

            float intensity = (dotProduct + (1 - dotProduct) * (1 - collisionNormalInfluence)) * velocityMagnitude;

            return intensity;
        }
    }
}
