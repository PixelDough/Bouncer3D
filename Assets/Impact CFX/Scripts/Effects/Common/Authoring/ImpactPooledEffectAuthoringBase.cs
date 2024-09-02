namespace ImpactCFX
{
    /// <summary>
    /// Base class for creating impact effect assets for effects that need to use object pooling.
    /// </summary>
    public abstract class ImpactPooledEffectAuthoringBase : ImpactEffectAuthoringBase
    {
        /// <summary>
        /// Gets the template object for the object pool created for this effect.
        /// </summary>
        /// <returns>The template object prefab.</returns>
        public abstract PooledEffectObjectBase GetTemplateObject();
    }
}