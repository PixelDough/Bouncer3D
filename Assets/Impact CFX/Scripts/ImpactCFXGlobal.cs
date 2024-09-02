using System.Collections.Generic;

namespace ImpactCFX
{
    /// <summary>
    /// Global class for managing and interfacing with the active Impact CFX Manager instance.
    /// </summary>
    public static class ImpactCFXGlobal
    {
        private static ImpactCFXManager instance;
        private static bool hasInstance => instance != null && !instance.Equals(null);

        /// <summary>
        /// Sets the active Impact CFX Manager instance.
        /// </summary>
        public static void SetInstance(ImpactCFXManager impactCFXManager)
        {
            instance = impactCFXManager;
        }

        /// <summary>
        /// Clears the active Impact CFX Manager instance.
        /// </summary>
        public static void ClearInstance()
        {
            instance = null;
        }

        /// <summary>
        /// Clears the active Impact CFX Manager instance if it matches the given instance.
        /// This is to account for switching between different instances.
        /// </summary>
        public static void ClearInstance(ImpactCFXManager impactCFXManager)
        {
            if (instance == impactCFXManager)
                instance = null;
        }

        /// <summary>
        /// Registers a list of materials from a registry with the impact manager to convert them into data structures suitable for use in jobs.
        /// </summary>
        /// <param name="materials">The material registry containing the materials to be registered.</param>
        public static void RegisterMaterials(ImpactMaterialRegistry materials)
        {
            if (hasInstance)
            {
                instance.RegisterMaterials(materials);
            }
        }

        /// <summary>
        /// Registers a list of materials with the impact manager to convert them into data structures suitable for use in jobs.
        /// </summary>
        /// <param name="materials">The materials being registered.</param>
        public static void RegisterMaterials(IEnumerable<ImpactMaterialAuthoring> materials)
        {
            if (hasInstance)
            {
                instance.RegisterMaterials(materials);
            }
        }

        /// <summary>
        /// Registers a single material with the impact manager to convert it into data structures suitable for use in jobs.
        /// </summary>
        /// <param name="material">The material being registered.</param>
        public static void RegisterMaterial(ImpactMaterialAuthoring material)
        {
            if (hasInstance)
            {
                instance.RegisterMaterial(material);
            }
        }

        /// <summary>
        /// Completely disposes of and destroys all material and effect data and objects currently registered in the impact manager.
        /// </summary>
        public static void ClearAllRegistered()
        {
            if (hasInstance)
            {
                instance.ClearAllRegistered();
            }
        }

        /// <summary>
        /// Finds all Impact Objects in the active scene and forces them to register their materials.
        /// </summary>
        public static void FindObjectsAndRegisterMaterials()
        {
            if (hasInstance)
            {
                instance.FindObjectsAndRegisterMaterials();
            }
        }

        /// <summary>
        /// Does the impact manager have the capacity to queue a collision of the given type?
        /// This is useful to abort early to save processing.
        /// </summary>
        /// <param name="collisionType">The collision type.</param>
        /// <param name="materialCount">The number of materials that will need to be processed for the collision.</param>
        public static bool CanQueueCollision(CollisionType collisionType, int materialCount)
        {
            if (hasInstance)
            {
                return instance.CanQueueCollision(collisionType, materialCount);
            }

            return true;
        }

        /// <summary>
        /// Queues a collision between 2 objects when the other object has an impact object component.
        /// </summary>
        /// <param name="triggerObject">The object that triggered the collision. This object will be responsible for playing effects.</param>
        /// <param name="hitObject">The object being collided with.</param>
        /// <param name="contactPoint">The contact point of the collision.</param>
        /// <param name="collisionType">The type of collision.</param>
        /// <param name="triggerObjectMaterialCount">The number of materials to get for the object that triggered the collision.</param>
        /// <param name="hitObjectMaterialCount">The number of materials to get for the object that was hit.</param>
        /// <param name="collisionVelocityMethod">The method to use for calculating the collision velocity.</param>
        /// <param name="triggerFallbackMaterial">Optional fallback material to use if no material can be found on the trigger object.</param>
        /// <param name="hitFallbackTags">Optional fallback tags to use if no material can be found on the hit object.</param>
        public static void QueueCollision(
            IImpactObject triggerObject,
            IImpactObject hitObject,
            ImpactContactPoint contactPoint,
            CollisionType collisionType,
            int triggerObjectMaterialCount,
            int hitObjectMaterialCount,
            CollisionVelocityMethod collisionVelocityMethod,
            ImpactMaterialAuthoring triggerFallbackMaterial = default,
            ImpactTagMask hitFallbackTags = default)
        {
            if (hasInstance)
            {
                instance.QueueCollision(triggerObject, hitObject, contactPoint, collisionType, triggerObjectMaterialCount, hitObjectMaterialCount, collisionVelocityMethod, triggerFallbackMaterial, hitFallbackTags);
            }
        }

        /// <summary>
        /// Queues a collision using basic data for the object that was hit.
        /// </summary>
        /// <param name="triggerObject">The object that triggered the collision. This object will be responsible for playing effects.</param>
        /// <param name="hitObjectData">Basic data for the object being collided with.</param>
        /// <param name="contactPoint">The contact point of the collision.</param>
        /// <param name="collisionType">The type of collision.</param>
        /// <param name="triggerObjectMaterialCount">The number of materials to get for the object that triggered the collision.</param>
        /// <param name="collisionVelocityMethod">The method to use for calculating the collision velocity.</param>
        /// <param name="triggerFallbackMaterial">Optional fallback material to use if no material can be found on the trigger object.</param>
        public static void QueueCollision(
            IImpactObject triggerObject,
            ImpactObjectData hitObjectData,
            ImpactContactPoint contactPoint,
            CollisionType collisionType,
            int triggerObjectMaterialCount,
            CollisionVelocityMethod collisionVelocityMethod,
            ImpactMaterialAuthoring triggerFallbackMaterial = default)
        {
            if (hasInstance)
            {
                instance.QueueCollision(triggerObject, hitObjectData, contactPoint, collisionType, triggerObjectMaterialCount, collisionVelocityMethod, triggerFallbackMaterial);
            }
        }

        /// <summary>
        /// Queues a collision using basic data for the object triggering the collision.
        /// </summary>
        /// <param name="triggerObjectData">Basic data for the object that triggered the collision. This object will be responsible for playing effects.</param>
        /// <param name="hitObject">The object being collided with.</param>
        /// <param name="contactPoint">The contact point of the collision.</param>
        /// <param name="collisionType">The type of collision.</param>
        /// <param name="collisionVelocityMethod">The method to use for calculating the collision velocity.</param>
        /// <param name="hitFallbackTags">Optional fallback tags to use if no material can be found on the hit object.</param>
        public static void QueueCollision(
            ImpactObjectData triggerObjectData,
            IImpactObject hitObject,
            ImpactContactPoint contactPoint,
            CollisionType collisionType,
            int hitObjectMaterialCount,
            CollisionVelocityMethod collisionVelocityMethod,
            ImpactTagMask hitFallbackTags = default)
        {
            if (hasInstance)
            {
                instance.QueueCollision(triggerObjectData, hitObject, contactPoint, collisionType, hitObjectMaterialCount, collisionVelocityMethod, hitFallbackTags);
            }
        }

        /// <summary>
        /// Queues a collision using basic data for both the object triggering the collision and the object that was hit.
        /// </summary>
        /// <param name="triggerObjectData">Basic data for the object that triggered the collision. This object will be responsible for playing effects.</param>
        /// <param name="hitObjectData">Basic data for the object being collided with.</param>
        /// <param name="contactPoint">The contact point of the collision.</param>
        /// <param name="collisionType">The type of collision.</param>
        /// <param name="collisionVelocityMethod">The method to use for calculating the collision velocity.</param>
        public static void QueueCollision(
            ImpactObjectData triggerObjectData,
            ImpactObjectData hitObjectData,
            ImpactContactPoint contactPoint,
            CollisionType collisionType,
            CollisionVelocityMethod collisionVelocityMethod)
        {
            if (hasInstance)
            {
                instance.QueueCollision(triggerObjectData, hitObjectData, contactPoint, collisionType, collisionVelocityMethod);
            }
        }

        /// <summary>
        /// Try to get the effect processor of the given type.
        /// </summary>
        /// <typeparam name="T">The type of effect processor.</typeparam>
        /// <param name="effectProcessor">The effect processor that was found, if any.</param>
        /// <returns>True if an effect processor matching the given type was found. False otherwise.</returns>
        public static bool TryGetEffectProcessor<T>(out T effectProcessor) where T : ImpactEffectProcessorBase
        {
            if (hasInstance)
            {
                return instance.TryGetEffectProcessor<T>(out effectProcessor);
            }

            effectProcessor = default(T);
            return false;
        }

        /// <summary>
        /// Try to get the material processor of the given type.
        /// </summary>
        /// <typeparam name="T">The type of effect processor.</typeparam>
        /// <param name="materialProcessor">The material processor that was found, if any.</param>
        /// <returns>True if an material processor matching the given type was found. False otherwise.</returns>
        public static bool TryGetMaterialProcessor<T>(out T materialProcessor) where T : ImpactMaterialProcessorBase
        {
            if (hasInstance)
            {
                return instance.TryGetMaterialProcessor<T>(out materialProcessor);
            }

            materialProcessor = default(T);
            return false;
        }

        /// <summary>
        /// Resets all effect processors controlled by the active manager instance.
        /// </summary>
        public static void ResetAllEffectProcessors()
        {
            if (hasInstance)
            {
                instance.ResetAllEffectProcessors();
            }
        }
    }
}