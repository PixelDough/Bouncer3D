using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Compiles all of the common data created from a collision.
    /// </summary>
    public struct CollisionResultData
    {
        /// <summary>
        /// The contact point.
        /// </summary>
        public Vector3 Point;

        /// <summary>
        /// The contact normal.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The collision velocity.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// How much influence the impact material has at the contact point.
        /// </summary>
        public float MaterialComposition;

        /// <summary>
        /// The type of collision.
        /// </summary>
        public CollisionType CollisionType;

        /// <summary>
        /// The object that triggered the collision.
        /// </summary>
        public GameObject TriggerObject;

        /// <summary>
        /// The object being collided with.
        /// </summary>
        public GameObject HitObject;

        public CollisionResultData(
            CollisionInputData collisionData,
            ImpactVelocityData velocityData,
            MaterialCompositionData materialCompositionData,
            CollisionObjectPair collisionObjectPair)
        {
            Point = collisionData.Point;
            Normal = collisionData.Normal;
            Velocity = velocityData.ImpactVelocity;

            MaterialComposition = materialCompositionData.Composition;

            CollisionType = collisionData.CollisionType;

            TriggerObject = collisionObjectPair.TriggerObject;
            HitObject = collisionObjectPair.HitObject;
        }

        public override string ToString()
        {
            return $"{CollisionType} at position ({Point.x:F2}, {Point.y:F2}, {Point.z:F2}), TriggerObject = {ImpactCFXLogger.GetGameObjectString(TriggerObject)}, HitObject = {ImpactCFXLogger.GetGameObjectString(HitObject)}, Velocity = {Velocity.magnitude:F2}";
        }
    }
}
