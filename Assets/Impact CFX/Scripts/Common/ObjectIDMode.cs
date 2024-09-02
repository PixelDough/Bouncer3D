using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Defines behavior for getting object/asset IDs.
    /// </summary>
    public enum ObjectIDMode
    {
        /// <summary>
        /// The GetInstanceID() function will be used.
        /// </summary>
        [Tooltip("The GetInstanceID() function will be used.")]
        InstanceID = 0,

        /// <summary>
        /// The ID will be derived from the object's name.
        /// </summary>
        [Tooltip("The ID will be derived from the object's name.")]
        ObjectName = 1
    }
}
