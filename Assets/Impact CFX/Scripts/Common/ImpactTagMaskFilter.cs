using System;

namespace ImpactCFX
{
    /// <summary>
    /// Used to filter tag values.
    /// </summary>
    [Serializable]
    public struct ImpactTagMaskFilter
    {
        /// <summary>
        /// The tags to compare against.
        /// </summary>
        public ImpactTagMask TagMask;

        /// <summary>
        /// Do the tags need to match exactly? If false, only 1 tag needs to match.
        /// </summary>
        public bool Exact;

        /// <summary>
        /// Compares the given tags.
        /// </summary>
        /// <param name="tagMask">The tags to compare.</param>
        /// <returns>Returns true if the tags match based on the exact field, false otherwise.</returns>
        public bool CompareTagMask(ImpactTagMask tagMask)
        {
            if (Exact)
                return tagMask == TagMask;
            else
                return (tagMask & TagMask) != 0;
        }
    }
}
