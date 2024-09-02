using Unity.Mathematics;

namespace ImpactCFX
{
    /// <summary>
    /// Holds input data suitable for jobs for a collision event.
    /// </summary>
    public struct CollisionInputData
    {
        /// <summary>
        /// The type of collision.
        /// </summary>
        public CollisionType CollisionType;

        /// <summary>
        /// The ID of the object that triggered the collision.
        /// </summary>
        public int TriggerObjectID;

        /// <summary>
        /// The ID of the object being collided with.
        /// </summary>
        public int HitObjectID;

        /// <summary>
        /// Chunk of the material composition array used to store material data for the object that triggered the collision.
        /// </summary>
        public ArrayChunk TriggerObjectMaterialCompositionArrayChunk;

        /// <summary>
        /// Chunk of the material composition array used to store material data for the object that was hit.
        /// </summary>
        public ArrayChunk HitObjectMaterialCompositionArrayChunk;

        /// <summary>
        /// The contact point.
        /// </summary>
        public float3 Point;

        /// <summary>
        /// The contact normal.
        /// </summary>
        public float3 Normal;

        /// <summary>
        /// The method to use for calculating the collision velocity.
        /// </summary>
        public CollisionVelocityMethod CollisionVelocityMethod;

        /// <summary>
        /// The collision velocity provided by the collision message.
        /// </summary>
        public float3 CollisionMessageVelocity;

        /// <summary>
        /// The priority of the collision.
        /// </summary>
        public float Priority;

        public override string ToString()
        {
            return $"{CollisionType} at position ({Point.x:F2}, {Point.y:F2}, {Point.z:F2}), TriggerObjectID = {TriggerObjectID}, HitObjectID = {HitObjectID}";
        }
    }
}
