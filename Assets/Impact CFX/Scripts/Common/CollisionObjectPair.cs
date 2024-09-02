using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// A pair of objects involved in a collision.
    /// </summary>
    public struct CollisionObjectPair
    {
        /// <summary>
        /// The object that triggered the collision message.
        /// </summary>
        public GameObject TriggerObject;

        /// <summary>
        /// The object that was hit.
        /// </summary>
        public GameObject HitObject;

        public CollisionObjectPair(GameObject triggerObject, GameObject hitObject)
        {
            TriggerObject = triggerObject;
            HitObject = hitObject;
        }
    }
}
