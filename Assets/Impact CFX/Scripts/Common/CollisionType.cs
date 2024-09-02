namespace ImpactCFX
{
    /// <summary>
    /// The type of collision.
    /// </summary>
    public enum CollisionType
    {
        /// <summary>
        /// A single-event collision.
        /// </summary>
        Collision = 0,
        /// <summary>
        /// A continuous collision for sliding.
        /// </summary>
        Slide = 1,
        /// <summary>
        /// A continuous collision for rolling.
        /// </summary>
        Roll = 2
    }

    /// <summary>
    /// Extension methods for the CollisionType enum.
    /// </summary>
    public static class CollisionTypeExtensions
    {
        /// <summary>
        /// Is the collision type a looped collision type?
        /// </summary>
        public static bool IsLooping(this CollisionType collisionType)
        {
            return collisionType != CollisionType.Collision;
        }

        /// <summary>
        /// Does the collision type require a contact point ID?
        /// </summary>
        public static bool RequiresContactPointID(this CollisionType collisionType)
        {
            return collisionType != CollisionType.Collision;
        }
    }
}
