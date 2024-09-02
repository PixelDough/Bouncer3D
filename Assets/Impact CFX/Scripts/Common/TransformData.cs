using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Holds transform data in a form suitable for jobs.
    /// </summary>
    public struct TransformData
    {
        /// <summary>
        /// The transform's position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The transform's rotation.
        /// </summary>
        public Quaternion Rotation;

        public TransformData(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
        }

        public TransformData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}