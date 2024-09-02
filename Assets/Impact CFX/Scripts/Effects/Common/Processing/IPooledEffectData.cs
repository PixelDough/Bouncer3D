using ImpactCFX.Pooling;

namespace ImpactCFX
{
    /// <summary>
    /// Interface for impact pooled effect data that is suitable for jobs.
    /// </summary>
    /// <typeparam name="TResult">The type of result struct that this effect returns.</typeparam>
    public interface IPooledEffectData<TResult> : IEffectData<TResult> where TResult : struct, IEffectResult, IObjectPoolRequest
    {
        /// <summary>
        /// Unique identifier for the template object used by the effect.
        /// </summary>
        int TemplateID { get; set; }
    }
}