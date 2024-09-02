using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImpactCFX
{
    [CreateAssetMenu(fileName = "New Impact Material", menuName = "Impact CFX/Material", order = 1)]
    public class ImpactMaterialAuthoring : ScriptableObject
    {
        [Serializable]
        public class EffectSet
        {
            [Tooltip("Editor-only name for this effect set for easier identification when editing.")]
            public string Name;

            [Header("Filters")]
            [Tooltip("Tags that must be present for this effect set to be used.")]
            public ImpactTagMaskFilter IncludeTags;
            [Tooltip("Tags that, when present, will cause this effect set to be ignored.")]
            public ImpactTagMaskFilter ExcludeTags;

            [Space()]
            public List<ImpactEffectAuthoringBase> Effects = new List<ImpactEffectAuthoringBase>();
        }

        [Tooltip("The Impact Tags associated with this material.")]
        [SerializeField]
        private ImpactTagMask materialTags;

        [Tooltip("The list of all effects associated with this material.")]
        [SerializeField]
        private List<EffectSet> effectSets = new List<EffectSet>();

        [Tooltip("The Impact Tags to use if the kind of object being collided with is not known.")]
        [SerializeField]
        private ImpactTagMask fallbackTags;

        [Tooltip("How the ID for this material is determined.")]
        [SerializeField]
        private ObjectID materialID;

        public IReadOnlyList<EffectSet> EffectSets => effectSets.AsReadOnly();

        /// <summary>
        /// Gets the ID of this material.
        /// </summary>
        public int GetID()
        {
            return materialID.GetIDForObject(this);
        }

        /// <summary>
        /// Convenience method for getting material data.
        /// </summary>
        public ImpactMaterialData GetMaterialData()
        {
            return new ImpactMaterialData()
            {
                MaterialID = GetID(),
                MaterialTags = this.materialTags,
                FallbackTags = this.fallbackTags
            };
        }

        /// <summary>
        /// Convenience method for getting material composition data.
        /// </summary>
        public MaterialCompositionData GetMaterialComposition(float composition = 1)
        {
            return new MaterialCompositionData(GetMaterialData(), composition);
        }
    }
}