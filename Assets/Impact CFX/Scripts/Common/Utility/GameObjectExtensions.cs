using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Extensions for working with GameObjects.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Either gets a component on the given game object or adds one.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="gameObject">The game object to get or add the component to.</param>
        /// <param name="checkParents">Should we look for the component in parent objects?</param>
        /// <returns>A reference to the existing or new component.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject, bool checkParents) where T : Component
        {
            T existing = checkParents ? gameObject.GetComponentInParent<T>() : gameObject.GetComponent<T>();
            if (existing != null)
                return existing;

            return gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Checks if both of the given transforms have the same parent.
        /// </summary>
        /// <param name="a">The first transform.</param>
        /// <param name="b">The second transform.</param>
        /// <param name="parent">Optional parent to check for. If null, the root parent of both transforms will be checked.</param>
        public static bool ShareParent(Transform a, Transform b, Transform parent = null)
        {
            if (parent == null)
            {
                return a.root == b.root;
            }
            else
            {
                bool aHasParent = HasParent(a, parent);
                bool bHasParent = HasParent(b, parent);

                return aHasParent && bHasParent;
            }
        }

        /// <summary>
        /// Checks if the given transform has the given parent.
        /// </summary>
        /// <param name="a">The transform.</param>
        /// <param name="parent">Parent to check for.</param>
        /// <returns>True if the transform has the parent, false otherwise.</returns>
        public static bool HasParent(Transform a, Transform parent)
        {
            if (a.parent != null)
            {
                if (a.parent == parent)
                    return true;
                else
                    return HasParent(a.parent, parent);
            }

            return false;
        }
    }
}

