using Unity.Mathematics;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Convenience methods for triggering effects via raycasting.
    /// </summary>
    public static class ImpactRaycaster
    {
        #region 3D

        /// <summary>
        /// Queues an effect from a 3D raycast, playing an effect using the given Impact Material.
        /// </summary>
        /// <param name="raycastHit">The raycast hit produced by the raycast.</param>
        /// <param name="velocity">The velocity to assign to the raycast.</param>
        /// <param name="impactMaterial">The impact material to use for playing effects.</param>
        /// <param name="collisionType">The type of collision to assign to the raycast.</param>
        /// <param name="raycastID">An identifier for the raycast. Only necessary if the collision type is slide or roll.</param>
        /// <param name="hitObjectMaterialCount">The number of materials to get for the object that was hit.</param>
        /// <param name="sourceObject">An optional Game Object that the raycast is being sent from. This is treated as the "trigger" object.</param>
        /// <param name="fallbackTags">Optional fallback tags to use if no material can be found on the object hit by the raycast.</param>
        public static void QueueRaycast3D(
            RaycastHit raycastHit,
            Vector3 velocity,
            ImpactMaterialAuthoring impactMaterial,
            CollisionType collisionType,
            int raycastID,
            int hitObjectMaterialCount,
            GameObject sourceObject = default,
            ImpactTagMask fallbackTags = default)
        {
            IImpactObject hitObject = raycastHit.collider.GetComponentInParent<IImpactObject>();
            ImpactContactPoint contactPoint = new ImpactContactPoint(raycastHit, false, sourceObject);

            RigidbodyStateData rigidbodyState = new RigidbodyStateData(velocity, float3.zero, contactPoint.Point);
            ImpactObjectData triggerObjectData = new ImpactObjectData()
            {
                ID = raycastID,
                MaterialComposition = new MaterialCompositionData(impactMaterial.GetMaterialData(), 1),
                RigidbodyData = new RigidbodyData(rigidbodyState, rigidbodyState)
            };

            ImpactCFXGlobal.QueueCollision(triggerObjectData, hitObject, contactPoint, collisionType, hitObjectMaterialCount, CollisionVelocityMethod.RelativeVelocities, fallbackTags);
        }

        /// <summary>
        /// Queues an effect from a 3D raycast, playing an effect using the object that was hit by the raycast.
        /// </summary>
        /// <param name="raycastHit">The raycast hit produced by the raycast.</param>
        /// <param name="velocity">The velocity to assign to the raycast.</param>
        /// <param name="impactTagMask">The impact tags to assign to the raycast.</param>
        /// <param name="collisionType">The type of collision to assign to the raycast.</param>
        /// <param name="raycastID">An identifier for the raycast. Only necessary if the collision type is slide or roll.</param>
        /// <param name="hitObjectMaterialCount">The number of materials to get for the object that was hit.</param>
        /// <param name="sourceObject">An optional Game Object that the raycast is being sent from. This is treated as the "hit" object.</param>
        /// <param name="fallbackMaterial">An optional fallback material to use if no material can be found on the object hit by the raycast.</param>
        public static void QueueRaycastInverted3D(
            RaycastHit raycastHit,
            Vector3 velocity,
            ImpactTagMask impactTagMask,
            CollisionType collisionType,
            int raycastID,
            int hitObjectMaterialCount,
            GameObject sourceObject = default,
            ImpactMaterialAuthoring fallbackMaterial = default)
        {
            IImpactObject triggerObject = raycastHit.collider.GetComponentInParent<IImpactObject>();
            ImpactContactPoint contactPoint = new ImpactContactPoint(raycastHit, true, sourceObject);

            RigidbodyStateData rigidbodyState = new RigidbodyStateData(velocity, float3.zero, contactPoint.Point);
            ImpactObjectData hitObject = new ImpactObjectData()
            {
                ID = raycastID,
                MaterialComposition = new MaterialCompositionData(new ImpactMaterialData() { MaterialTags = impactTagMask }, 1),
                RigidbodyData = new RigidbodyData(rigidbodyState, rigidbodyState)
            };

            ImpactCFXGlobal.QueueCollision(triggerObject, hitObject, contactPoint, collisionType, hitObjectMaterialCount, CollisionVelocityMethod.RelativeVelocities, fallbackMaterial);
        }

        #endregion

        #region 2D

        /// <summary>
        /// Queues an effect from a 2D raycast, playing an effect using the given Impact Material.
        /// </summary>
        /// <param name="raycastHit">The raycast hit produced by the raycast.</param>
        /// <param name="velocity">The velocity to assign to the raycast.</param>
        /// <param name="impactMaterial">The impact material to use for playing effects.</param>
        /// <param name="collisionType">The type of collision to assign to the raycast.</param>
        /// <param name="raycastID">An identifier for the raycast. Only necessary if the collision type is slide or roll.</param>
        /// <param name="hitObjectMaterialCount">The number of materials to get for the object that was hit.</param>
        /// <param name="sourceObject">An optional Game Object that the raycast is being sent from. This is treated as the "trigger" object.</param>
        public static void QueueRaycast2D(
            RaycastHit2D raycastHit,
            Vector2 velocity,
            ImpactMaterialAuthoring impactMaterial,
            CollisionType collisionType,
            int raycastID,
            int hitObjectMaterialCount,
            GameObject sourceObject = default)
        {
            IImpactObject hitObject = raycastHit.collider.GetComponentInParent<IImpactObject>();
            ImpactContactPoint contactPoint = new ImpactContactPoint(raycastHit, false, sourceObject);

            RigidbodyStateData rigidbodyState = new RigidbodyStateData(new float3(velocity.x, velocity.y, 0), float3.zero, contactPoint.Point);
            ImpactObjectData raycastData = new ImpactObjectData()
            {
                ID = raycastID,
                MaterialComposition = new MaterialCompositionData(impactMaterial.GetMaterialData(), 1),
                RigidbodyData = new RigidbodyData(rigidbodyState, rigidbodyState)
            };

            ImpactCFXGlobal.QueueCollision(raycastData, hitObject, contactPoint, collisionType, hitObjectMaterialCount, CollisionVelocityMethod.RelativeVelocities);
        }

        /// <summary>
        /// Queues an effect from a 2D raycast, playing an effect using the object that was hit.
        /// </summary>
        /// <param name="raycastHit">The raycast hit produced by the raycast.</param>
        /// <param name="velocity">The velocity to assign to the raycast.</param>
        /// <param name="impactTagMask">The impact tags to assign to the raycast.</param>
        /// <param name="collisionType">The type of collision to assign to the raycast.</param>
        /// <param name="raycastID">An identifier for the raycast. Only necessary if the collision type is slide or roll.</param>
        /// <param name="hitObjectMaterialCount">The number of materials to get for the object that was hit.</param>
        /// <param name="sourceObject">An optional Game Object that the raycast is being sent from. This is treated as the "hit" object.</param>
        public static void QueueRaycastInverted2D(
            RaycastHit2D raycastHit,
            Vector2 velocity,
            ImpactTagMask impactTagMask,
            CollisionType collisionType,
            int raycastID,
            int hitObjectMaterialCount,
            GameObject sourceObject = default)
        {
            IImpactObject hitObject = raycastHit.collider.GetComponentInParent<IImpactObject>();
            ImpactContactPoint contactPoint = new ImpactContactPoint(raycastHit, true, sourceObject);

            RigidbodyStateData rigidbodyState = new RigidbodyStateData(new float3(velocity.x, velocity.y, 0), float3.zero, contactPoint.Point);
            ImpactObjectData raycastData = new ImpactObjectData()
            {
                ID = raycastID,
                MaterialComposition = new MaterialCompositionData(new ImpactMaterialData() { MaterialTags = impactTagMask }, 1),
                RigidbodyData = new RigidbodyData(rigidbodyState, rigidbodyState)
            };

            ImpactCFXGlobal.QueueCollision(hitObject, raycastData, contactPoint, collisionType, hitObjectMaterialCount, CollisionVelocityMethod.RelativeVelocities);
        }

        #endregion
    }
}