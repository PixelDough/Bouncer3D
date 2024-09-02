namespace ImpactCFX.FMOD
{
    /// <summary>
    /// Constants used for FMOD parameters.
    /// </summary>
    public static class ImpactFMODParameters
    {
        /// <summary>
        /// A 0 to 1 value representing the intensity of the collision effect.
        /// This is normalized relative to the Velocity Reference Range property of the FMOD audio effect.
        /// This takes into account Collision Normal Influence.
        /// </summary>
        public const string Intensity = "ImpactCFX_Intensity";

        /// <summary>
        /// The raw intensity of the collision without a specified value range.
        /// This takes into account Collision Normal Influence.
        /// </summary>
        public const string RawIntensity = "ImpactCFX_RawIntensity";

        /// <summary>
        /// A 0 to 1 value representing the influence of the material at the contact point.
        /// </summary>
        public const string CompositionValue = "ImpactCFX_CompositionValue";

        /// <summary>
        /// The type of collision. 0 = Collision, 1 = Slide, 2 = Roll
        /// </summary>
        public const string CollisionType = "ImpactCFX_CollisionType";
    }
}
