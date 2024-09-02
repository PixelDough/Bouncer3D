using System;

namespace ImpactCFX
{
    /// <summary>
    /// Container for a value that can be overriden from it's automatically set value.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    public class OverrideValue<T>
    {
        public T Value;
        public bool Override;
    }

    /// <summary>
    /// Container for an overridable integer value.
    /// </summary>
    [Serializable]
    public class OverrideValueInt : OverrideValue<int> { }
}
