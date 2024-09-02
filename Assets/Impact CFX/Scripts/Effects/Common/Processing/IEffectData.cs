using Unity.Mathematics;

namespace ImpactCFX
{
    /// <summary>
    /// Interface for impact effect data that is suitable for jobs.
    /// </summary>
    /// <typeparam name="TResult">The type of result struct that this effect returns.</typeparam>
    public interface IEffectData<TResult> where TResult : struct, IEffectResult
    {
        /// <summary>
        /// Unique identifier for this effect.
        /// </summary>
        int EffectID { get; set; }

        /// <summary>
        /// Tags that must be present for the effect to be used.
        /// </summary>
        ImpactTagMaskFilter IncludeTags { get; set; }

        /// <summary>
        /// Tags that, if present, will make the effect not be used.
        /// </summary>
        ImpactTagMaskFilter ExcludeTags { get; set; }

        /// <summary>
        /// Gets an interaction result based on the given input collision data.
        /// </summary>
        /// <param name="collisionData">The collision data that will be used to calculate the result.</param>
        /// <param name="materialCompositionData">The material data of the object being collided with.</param>
        /// <param name="velocityData">The velocity of the collision.</param>
        /// <param name="random">Reference to a Random generator for any randomness required by the effect.</param>
        /// <returns>Data describing the result of the interaction.</returns>
        TResult GetResult(CollisionInputData collisionData, MaterialCompositionData materialCompositionData, ImpactVelocityData velocityData, ref Random random);
    }
}