namespace ImpactCFX
{
    /// <summary>
    /// Holds an index offset and length for a chunk of data in an array.
    /// </summary>
    public struct ArrayChunk
    {
        /// <summary>
        /// An invalid/null array chunk.
        /// </summary>
        public static ArrayChunk Default => new ArrayChunk(-1, -1);

        /// <summary>
        /// The starting index of the chunk.
        /// </summary>
        public int Offset;

        /// <summary>
        /// The length of the chunk.
        /// </summary>
        public int Length;

        public ArrayChunk(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }
    }
}
