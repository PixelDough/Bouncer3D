namespace ImpactCFX.Pooling
{
    /// <summary>
    /// Interface for data that requests an object from an object pool.
    /// </summary>
    public interface IObjectPoolRequest
    {
        /// <summary>
        /// Is the request valid? If not, it will be skipped.
        /// </summary>
        bool IsObjectPoolRequestValid { get; }
        /// <summary>
        /// The ID of the template object this request is being made for.
        /// </summary>
        int TemplateID { get; }
        /// <summary>
        /// The priority of the request.
        /// </summary>
        float Priority { get; }
        /// <summary>
        /// The contact point ID of the request.
        /// </summary>
        long ContactPointID { get; }
        /// <summary>
        /// Should the contact point ID be checked? This is required for sliding and rolling effects, but can be false for other effects to save processing.
        /// </summary>
        bool CheckContactPointID { get; }

        /// <summary>
        /// The index of an object that can be retrieved. Will be -1 if no object was found.
        /// </summary>
        int ObjectIndex { set; get; }
        /// <summary>
        /// Whether or not the result is a new object or an update to an already active object. Used for sliding and rolling effects.
        /// </summary>
        bool IsUpdate { set; get; }
    }
}
