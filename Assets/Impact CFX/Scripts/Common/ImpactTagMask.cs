using System;

namespace ImpactCFX
{
    /// <summary>
    /// Bitmask for holding many impact tag values.
    /// </summary>
    [Serializable]
    public struct ImpactTagMask
    {
        /// <summary>
        /// Gets an empty tag mask with a value of 0.
        /// </summary>
        public static ImpactTagMask Empty => new ImpactTagMask(0);

        /// <summary>
        /// The raw bitmask value.
        /// </summary>
        public int Value;

        public ImpactTagMask(int value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is ImpactTagMask mask &&
                   Value == mask.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(ImpactTagMask a, ImpactTagMask b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(ImpactTagMask a, ImpactTagMask b)
        {
            return a.Value != b.Value;
        }

        public static int operator &(ImpactTagMask a, ImpactTagMask b)
        {
            return a.Value & b.Value;
        }
    }
}