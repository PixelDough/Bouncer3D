namespace ImpactCFX
{
    /// <summary>
    /// Holds impact material data suitable for jobs retrieved at a single collision point.
    /// </summary>
    public struct MaterialCompositionData
    {
        /// <summary>
        /// Gets an empty set of data with no material. Can be used to denote an invalid or unknown material.
        /// </summary>
        public static MaterialCompositionData Default => new MaterialCompositionData(new ImpactMaterialData(), 1);

        /// <summary>
        /// The basic impact material data.
        /// </summary>
        public ImpactMaterialData MaterialData;

        /// <summary>
        /// How much influence the material has at the point where it was retrieved.
        /// </summary>
        public float Composition;

        public MaterialCompositionData(ImpactMaterialData materialData, float composition)
        {
            MaterialData = materialData;
            Composition = composition;
        }
    }
}