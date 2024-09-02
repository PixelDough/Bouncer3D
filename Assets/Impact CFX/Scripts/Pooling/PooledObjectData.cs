namespace ImpactCFX.Pooling
{
    /// <summary>
    /// Data for a pooled object that is suitable for jobs.
    /// </summary>
    public struct PooledObjectData
    {
        /// <summary>
        /// Is the object available?
        /// </summary>
        public bool IsAvailable;
        /// <summary>
        /// The current priority of the object.
        /// </summary>
        public float Priority;
        /// <summary>
        /// The last frame at which this object was retrieved.
        /// </summary>
        public int LastRetrievedFrame;
        /// <summary>
        /// The contact point ID associated with this object.
        /// </summary>
        public long ContactPointID;

        /// <summary>
        /// The index of the request that most recently retrieved this object. Used when an object is stolen.
        /// </summary>
        public int LastRequestIndex;
    }
}
