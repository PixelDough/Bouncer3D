using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Base class for creating impact effect assets.
    /// </summary>
    public abstract class ImpactEffectAuthoringBase : ScriptableObject
    {
        [SerializeField]
        [Tooltip("How the ID for this effect is determined.")]
        private ObjectID effectID;

        /// <summary>
        /// Gets the ID of this effect.
        /// </summary>
        public int GetID()
        {
            return effectID.GetIDForObject(this);
        }

        /// <summary>
        /// Optional method to ensure that this effect contains valid data.
        /// </summary>
        public virtual bool Validate()
        {
            return true;
        }
    }
}