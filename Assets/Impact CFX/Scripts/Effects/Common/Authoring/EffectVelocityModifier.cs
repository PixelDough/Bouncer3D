using System;
using UnityEngine;

namespace ImpactCFX
{
    [Serializable]
    /// <summary>
    /// Class for editing effect modifiers based on velocity.
    /// </summary>
    public class EffectVelocityModifier
    {
        /// <summary>
        /// Is this modifier enabled?
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// The velocity range to compare the collision velocity against.
        /// </summary>
        [Tooltip("The range to compare the collision velocity against.")]
        public Range VelocityRange = new Range(1, 10);

        /// <summary>
        /// The curve used to evaluate the modifier.
        /// </summary>
        [Tooltip("The curve used to evaluate the modifier.")]
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// Evaluates the modifier multiplier using the given velocity.
        /// </summary>
        /// <param name="velocity">The velocity value.</param>
        /// <returns>A value based on evaluating the curve.</returns>
        public float Evaluate(float velocity)
        {
            return Curve.Evaluate(VelocityRange.Normalize(velocity));
        }
    }
}
