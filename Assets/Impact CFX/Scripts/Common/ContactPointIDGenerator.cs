namespace ImpactCFX
{
    /// <summary>
    /// Utility for generating an ID for contact points. Used to identify contact points for sliding and rolling.
    /// </summary>
    public static class ContactPointIDGenerator
    {
        /// <summary>
        /// Creates an ID using the given data.
        /// </summary>
        public static long CalculateContactPointID(int triggerObjectID, int hitObjectID, CollisionType collisionType, int tagMask, int effectID)
        {
            return cantorPairing(effectID,
                cantorPairing(tagMask,
                cantorPairing((int)collisionType,
                cantorPairing(triggerObjectID, hitObjectID))));
        }

        private static long cantorPairing(long k1, long k2)
        {
            return (k1 + k2) * (k1 + k2 + 1) / 2 + k2;
        }
    }
}
