using System.Collections.Generic;
using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Component that manages effects that are attached to an object.
    /// </summary>
    [AddComponentMenu("Impact CFX/Impact Attached Effect Manager")]
    public class ImpactAttachedEffectManager : MonoBehaviour
    {
        private List<PooledEffectObjectBase> attachedEffects = new List<PooledEffectObjectBase>();
        private bool suppressOnDestroyMessage;
        private bool preventAttachedEffectChange;

        /// <summary>
        /// Adds the effect to this object's list of attached effects. This does not change the parent of the effect.
        /// </summary>
        /// <param name="effectObject">The effect to add.</param>
        public void AddAttachedEffect(PooledEffectObjectBase effectObject)
        {
            if (!preventAttachedEffectChange)
                attachedEffects.Add(effectObject);
        }

        /// <summary>
        /// Removes the effect from this object's list of attached effects. This does not change the parent of the effect.
        /// </summary>
        /// <param name="effectObject">The effect to remove</param>
        public void RemoveAttachedEffect(PooledEffectObjectBase effectObject)
        {
            if (!preventAttachedEffectChange)
                attachedEffects.Remove(effectObject);
        }

        /// <summary>
        /// Releases all attached effects so that they are put back into their respective object pools.
        /// </summary>
        public void ReleaseAllAttachedEffects()
        {
            preventAttachedEffectChange = true;

            for (int i = 0; i < attachedEffects.Count; i++)
            {
                attachedEffects[i].ReturnToPool();
            }

            attachedEffects.Clear();
            preventAttachedEffectChange = false;
        }

        private void OnApplicationQuit()
        {
            suppressOnDestroyMessage = true;
        }

        private void OnDestroy()
        {
            if (!suppressOnDestroyMessage)
                ReleaseAllAttachedEffects();
        }
    }
}

