using System;

namespace ImpactCFX
{
    /// <summary>
    /// Wrapper for object/asset IDs.
    /// </summary>
    [Serializable]
    public struct ObjectID
    {
        /// <summary>
        /// The ID to use for the object.
        /// </summary>
        public ObjectIDMode ObjectIDMode;

        /// <summary>
        /// Gets the ID for the given object based on the object ID mode.
        /// </summary>
        public int GetIDForObject(UnityEngine.Object o)
        {
            return o.GetIDForObject(ObjectIDMode);
        }
    }

    public static class ObjectIDExtensions
    {
        /// <summary>
        /// Gets the appropriate ID for the given object using the object ID mode.
        /// </summary>
        /// <param name="o">The object to get an ID for.</param>
        /// <param name="objectIDMode">The object ID mode.</param>
        /// <returns>An integer ID for the object.</returns>
        public static int GetIDForObject(this UnityEngine.Object o, ObjectIDMode objectIDMode)
        {
            if (objectIDMode == ObjectIDMode.InstanceID)
                return o.GetInstanceID();
            else if (objectIDMode == ObjectIDMode.ObjectName)
                return o.name.GetHashCode();

            return 0;
        }
    }
}
