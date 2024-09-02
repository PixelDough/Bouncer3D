namespace ImpactCFX
{
    /// <summary>
    /// Utility class for manipulating bitmasks.
    /// </summary>
    public static class Bitmask
    {
        /// <summary>
        /// Sets the bit at the given position to 1.
        /// </summary>
        /// <param name="bitmask">The bitmask to change.</param>
        /// <param name="pos">The index of the bit</param>
        /// <returns>The updated bitmask.</returns>
        public static int SetBit(this int bitmask, int pos)
        {
            return bitmask | (1 << pos);
        }

        /// <summary>
        /// Sets the bit at the given position to 0.
        /// </summary>
        /// <param name="bitmask">The bitmask to change.</param>
        /// <param name="pos">The index of the bit to unset.</param>
        /// <returns>The updated bitmask.</returns>
        public static int UnsetBit(this int bitmask, int pos)
        {
            return bitmask & ~(1 << pos);
        }

        /// <summary>
        /// Checks if the bit at the given position is set.
        /// </summary>
        /// <param name="bitmask">The bitmask to change.</param>
        /// <param name="pos">The index of the bit to unset.</param>
        /// <returns>If the bit at the given position is set.</returns>
        public static bool IsBitSet(this int bitmask, int pos)
        {
            return (bitmask & (1 << pos)) != 0;
        }
    }
}

