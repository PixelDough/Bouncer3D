using System;
using Unity.Burst;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Utility class for logging common Impact CFX debug messages.
    /// </summary>
    public static class ImpactCFXLogger
    {
        public const string DEBUG_HEADER_REGISTER = "REGISTER";
        public const string DEBUG_HEADER_TRIGGER = "TRIGGER";
        public const string DEBUG_HEADER_QUEUE = "QUEUE";
        public const string DEBUG_HEADER_PROCESSING = "PROCESSING";
        public const string DEBUG_HEADER_OBJECTPOOL = "OBJECTPOOL";
        public const string DEBUG_HEADER_PLAY = "PLAY";
        public const string DEBUG_HEADER_CLEAR = "CLEAR";

        private static int lastLoggedFrame;

        /// <summary>
        /// Log an error for a material that could not be found.
        /// </summary>
        /// <param name="materialID">The ID of the material that could not be found.</param>
        [BurstDiscard]
        public static void LogImpactMaterialNotFound(int materialID)
        {
            if (materialID != 0)
                Debug.LogError($"Could not find Impact Material with ID: {materialID}. " +
                    $"Please ensure that all of your Impact Materials are registered with an Impact Material Registry, by individual Impact Objects, or through your own scripts.");
        }

        /// <summary>
        /// Log an error for an object that is missing a Rigidbody.
        /// </summary>
        /// <param name="gameObject">The game object the rigidbody is missing from.</param>
        [BurstDiscard]
        public static void LogMissingRigidbody(GameObject gameObject)
        {
            Debug.LogError($"Unable to find Rigidbody or Rigidbody2D component on object '{gameObject.name}'. " +
                "Please ensure that you have a Rigidbody or Rigidbody2D component on your Impact Object Rigidbody objects.");
        }

        /// <summary>
        /// Logs a general debugging message.
        /// </summary>
        [BurstDiscard]
        public static void LogDebug(string header, string message, bool logFrameCount)
        {
            if (logFrameCount)
            {
                Debug.Log($"[{Time.frameCount}] [{header.ToUpper()}] {message}");
                lastLoggedFrame = Time.frameCount;
            }
            else
            {
                Debug.Log($"[{lastLoggedFrame}] [{header.ToUpper()}] {message}");
            }

        }

        /// <summary>
        /// Logs a trigger being invoked.
        /// </summary>
        [BurstDiscard]
        public static void LogTriggerInvoked(Type triggerType, string triggerMethod, GameObject gameObject)
        {
            LogDebug(DEBUG_HEADER_TRIGGER, $"[{triggerType.Name.ToUpper()}.{triggerMethod.ToUpper()}] invoked on {gameObject.name}.", true);
        }

        /// <summary>
        /// Logs a trigger aborting.
        /// </summary>
        [BurstDiscard]
        public static void LogTriggerAbort(Type triggerType, string reason, GameObject gameObject)
        {
            LogDebug(DEBUG_HEADER_TRIGGER, $"[{triggerType.Name.ToUpper()}] trigger aborted for {gameObject.name}: {reason}.", true);
        }

        /// <summary>
        /// Logs an effect being marked as invalid.
        /// </summary>
        [BurstDiscard]
        public static void LogEffectInvalid(Type effectType, int effectID, string reason)
        {
            LogDebug(DEBUG_HEADER_PROCESSING, $"[{effectType.Name.ToUpper()}] Invalid result for effect ID [{effectID}]: {reason}.", false);
        }

        /// <summary>
        /// Logs an effect result.
        /// </summary>
        [BurstDiscard]
        public static void LogEffectResult(Type effectType, int effectID, IEffectResult result)
        {
            LogDebug(DEBUG_HEADER_PROCESSING, $"[{effectType.Name.ToUpper()}] Final result for effect ID [{effectID}]: IsValid = {result.IsEffectValid}, {result}", false);
        }

        /// <summary>
        /// Logs an effect being played.
        /// </summary>
        [BurstDiscard]
        public static void LogEffectPlay(Type effectType, CollisionResultData collisionResultData, string message)
        {
            LogDebug(DEBUG_HEADER_PLAY, $"[{effectType.Name.ToUpper()}] playing {collisionResultData} :: {message}.", true);
        }

        /// <summary>
        /// Logs an effect being updated (for sliding and rolling).
        /// </summary>
        [BurstDiscard]
        public static void LogEffectUpdate(Type effectType, CollisionResultData collisionResultData, string message)
        {
            LogDebug(DEBUG_HEADER_PLAY, $"[{effectType.Name.ToUpper()}] updating {collisionResultData} :: {message}.", true);
        }

        /// <summary>
        /// Logs a material being registered with the Impact CFX Manager.
        /// </summary>
        public static void LogImpactMaterialRegistered(ImpactMaterialAuthoring material)
        {
            LogDebug(DEBUG_HEADER_REGISTER, $"Registering material '{material.name}' with ID {material.GetID()}.", true);
        }

        /// <summary>
        /// Logs an effect being registered with the Impact CFX Manager.
        /// </summary>
        public static void LogImpactEffectRegistered(ImpactEffectAuthoringBase effect)
        {
            LogDebug(DEBUG_HEADER_REGISTER, $"Registering effect '{effect.name}' with ID {effect.GetID()}.", true);
        }

        /// <summary>
        /// Logs a general message for clearing data and objects.
        /// </summary>
        public static void LogClear(string message)
        {
            LogDebug(DEBUG_HEADER_CLEAR, message, true);
        }

        /// <summary>
        /// Gets a string representing the given Impact Object.
        /// If the Impact Object is null, the fallback is used.
        /// </summary>
        [BurstDiscard]
        public static object GetImpactObjectString(IImpactObject impactObject, GameObject fallback)
        {
            if (impactObject != null)
                return impactObject.ToString();
            else
                return GetGameObjectString(fallback);
        }

        /// <summary>
        /// Gets a string representing the given Game Object, returning "NULL" if the object is null.
        /// </summary>
        [BurstDiscard]
        public static object GetGameObjectString(GameObject gameObject)
        {
            if (gameObject != null)
                return gameObject.name;
            else
                return "NULL";

        }


    }
}
