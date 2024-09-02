namespace ImpactCFX
{
    /// <summary>
    /// Holds basic impact material data that is suitable for jobs and burst.
    /// </summary>
    public struct ImpactMaterialData
    {
        public static ImpactMaterialData Default => new ImpactMaterialData();

        /// <summary>
        /// The ID of the impact material.
        /// </summary>
        public int MaterialID;

        /// <summary>
        /// The impact material's tags.
        /// </summary>
        public ImpactTagMask MaterialTags;

        /// <summary>
        /// The fallback tags specified by the material.
        /// </summary>
        public ImpactTagMask FallbackTags;
    }
}

