namespace ImpactCFX
{
    /// <summary>
    /// General interface for results of effects that is suitable for jobs.
    /// </summary>
    public interface IEffectResult
    {
        /// <summary>
        /// Results can be marked by an effect as invalid to save processing.
        /// </summary>
        bool IsEffectValid { get; set; }

        /// <summary>
        /// The index of the original collision data that created this result.
        /// </summary>
        int CollisionIndex { get; set; }

        /// <summary>
        /// The index of the original material composition data that created this result.
        /// </summary>
        int MaterialCompositionIndex { get; set; }
    }
}