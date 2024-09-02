namespace ImpactCFX
{
    /// <summary>
    /// Container for basic Impact Object data used for queueing collisions
    /// </summary>
    public struct ImpactObjectData
    {
        /// <summary>
        /// The ID of the object.
        /// </summary>
        public int ID;

        /// <summary>
        /// The priority of the object.
        /// </summary>
        public int Priority;

        /// <summary>
        /// The object's rigidbody data.
        /// </summary>
        public RigidbodyData RigidbodyData;

        /// <summary>
        /// The material composition data of the object.
        /// </summary>
        public MaterialCompositionData MaterialComposition;

        public override string ToString()
        {
            return $"ImpactObjectData ({ID})";
        }
    }
}
