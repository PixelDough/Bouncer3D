namespace ImpactCFX.Pooling
{
    /// <summary>
    /// Interface for any object that can be pooled.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// The last frame at which this object was retrieved.
        /// </summary>
        int LastRetrievedFrame { get; }

        /// <summary>
        /// The current priority of the object.
        /// </summary>
        float Priority { get; }

        /// <summary>
        /// The contact point ID associated with this object.
        /// </summary>
        long ContactPointID { get; }

        /// <summary>
        /// Does this object need to be updated?
        /// </summary>
        bool NeedsUpdate { get; }

        /// <summary>
        /// Retrieves this object from the pool.
        /// </summary>
        void RetrieveFromPool(float priority, long contactPointID);

        /// <summary>
        /// Performs any necessary update logic for the pooled object.
        /// </summary>
        void UpdatePooledObject();

        /// <summary>
        /// Returns this object to its parent pool.
        /// </summary>
        void ReturnToPool();

        /// <summary>
        /// Is the object available?
        /// </summary>
        bool IsAvailable();

        /// <summary>
        /// Disposes of this poolable object.
        /// </summary>
        void Destroy();
    }
}
