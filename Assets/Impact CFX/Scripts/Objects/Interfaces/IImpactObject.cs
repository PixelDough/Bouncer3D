using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Interface for all impact objects.
    /// </summary>
    public interface IImpactObject
    {
        /// <summary>
        /// Gets a unique identifier for this object.
        /// </summary>
        int GetID();

        /// <summary>
        /// Gets the priority for this object.
        /// </summary>
        int GetPriority();

        /// <summary>
        /// Gets the rigidbody data for this object.
        /// </summary>
        RigidbodyData GetRigidbodyData();

        /// <summary>
        /// Gets the game object assoicated with this object.
        /// </summary>
        GameObject GetGameObject();

        /// <summary>
        /// Gets the local position of a contact point relative to the object's transform.
        /// </summary>
        Vector3 GetContactPointRelativePosition(Vector3 point);
    }
}

