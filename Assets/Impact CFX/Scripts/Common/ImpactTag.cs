using System;

namespace ImpactCFX
{
    /// <summary>
    /// A value for a single impact tag.
    /// </summary>
    [Serializable]
    public struct ImpactTag
    {
        /// <summary>
        /// The index value of the tag.
        /// </summary>
        public int Value;

        public ImpactTag(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a tag mask with only this tag's bit set.
        /// </summary>
        public ImpactTagMask ToTagMask()
        {
            return new ImpactTagMask() { Value = 1 << Value };
        }

        public override bool Equals(object obj)
        {
            return obj is ImpactTag mask &&
                   Value == mask.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(ImpactTag a, ImpactTag b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(ImpactTag a, ImpactTag b)
        {
            return a.Value != b.Value;
        }
    }
}