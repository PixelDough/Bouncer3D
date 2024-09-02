namespace ImpactCFX
{
    /// <summary>
    /// Whether the collision was created using 3D or 2D physics.
    /// </summary>
    public enum PhysicsType
    {
        /// <summary>
        /// The collision was created using an unknown physics type.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The collision was created using 3D physics.
        /// </summary>
        Physics3D = 1,
        /// <summary>
        /// The collision was created using 2D physics.
        /// </summary>
        Physics2D = 2,

    }
}