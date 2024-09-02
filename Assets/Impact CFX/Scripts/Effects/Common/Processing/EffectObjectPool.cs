using ImpactCFX.Pooling;

namespace ImpactCFX
{
    /// <summary>
    /// Common implementation for an object pool used by effects with a template object.
    /// </summary>
    /// <typeparam name="T">The type of object to be pooled.</typeparam>
    public abstract class EffectObjectPool<T> : ObjectPool<T>
        where T : PooledEffectObjectBase
    {
        /// <summary>
        /// The template used to create all objects for this object pool.
        /// </summary>
        public T Template { get; private set; }

        /// <summary>
        /// Initializes the pool.
        /// </summary>
        /// <param name="template">The template object.</param>
        public void InitializeWithTemplate(T template)
        {
            Template = template;
            base.Initialize(template.GetPoolID(), template.ObjectPoolConfig.PoolSize, template.ObjectPoolConfig.Stealing);
        }

        protected override T createPooledObjectInstance(int index)
        {
            T pooledObjectInstance = Instantiate(Template, this.transform);
            pooledObjectInstance.gameObject.name = Template.name + "_" + index;

            pooledObjectInstance.SetParentPoolGameObject(this.gameObject);
            pooledObjectInstance.ReturnToPool();

            return pooledObjectInstance;
        }
    }
}