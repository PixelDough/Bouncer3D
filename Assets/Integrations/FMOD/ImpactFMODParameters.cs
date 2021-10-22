using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Impact.Integration.FMOD
{
    /// <summary>
    /// Contains constants used for FMOD parameters.
    /// </summary>
    public static class ImpactFMODParameters
    {
        /// <summary>
        /// A normalized (0 to 1) value representing the intensity of the collision interaction.
        /// This takes into account Collision Normal Influence.
        /// Normalization is relative to the Velocity Range property of the audio interaction.
        /// </summary>
        public const string Intensity = "Intensity";

        /// <summary>
        /// The real velocity of the collision interaction.
        /// </summary>
        public const string Velocity = "Velocity";

        /// <summary>
        /// If using material composition, a 0 to 1 value representing the influence of the material at the contact point.
        /// </summary>
        public const string CompositionValue = "CompositionValue";

        /// <summary>
        /// The type of collision interaction. Built-in values can be found in InteractionData:
        /// 0 = Collision, 1 = Slide, 2 = Roll, 3 = Simple
        /// </summary>
        public const string InteractionType = "InteractionType";
    }
}
