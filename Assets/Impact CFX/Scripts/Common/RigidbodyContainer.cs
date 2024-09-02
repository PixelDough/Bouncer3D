using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Wrapper class for 3D and 2D rigidbodies that handles tracking the rigidbody state.
    /// </summary>
    public class RigidbodyContainer
    {
        private GameObject gameObject;
        private Rigidbody rigidbody3D;
        private Rigidbody2D rigidbody2D;
        private PhysicsType physicsType;

        private RigidbodyStateData previousRigidbodyState;

        private Vector3 currentVelocity
        {
            get
            {
                if (physicsType == PhysicsType.Physics2D)
                    return rigidbody2D.velocity;
                else if (physicsType == PhysicsType.Physics3D)
                    return rigidbody3D.velocity;

                ImpactCFXLogger.LogMissingRigidbody(gameObject);
                return Vector3.zero;
            }
        }

        private Vector3 currentAngularVelocity
        {
            get
            {
                if (physicsType == PhysicsType.Physics2D)
                    return new Vector3(0, 0, rigidbody2D.angularVelocity * Mathf.Deg2Rad);
                else if (physicsType == PhysicsType.Physics3D)
                    return rigidbody3D.angularVelocity;

                ImpactCFXLogger.LogMissingRigidbody(gameObject);
                return Vector3.zero;
            }
        }

        private Vector3 currentWorldCenterOfMass
        {
            get
            {
                if (physicsType == PhysicsType.Physics2D)
                    return rigidbody2D.worldCenterOfMass;
                else if (physicsType == PhysicsType.Physics3D)
                    return rigidbody3D.worldCenterOfMass;

                ImpactCFXLogger.LogMissingRigidbody(gameObject);
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Creates a wrapper for the given GameObject that has either a Rigidbody or Rigidbody2D component.
        /// </summary>
        /// <param name="gameObject">A GameObject that has either a Rigidbody or Rigidbody2D component</param>
        public RigidbodyContainer(GameObject gameObject)
        {
            this.gameObject = gameObject;

            Rigidbody r3D = gameObject.GetComponentInParent<Rigidbody>();
            if (r3D != null)
            {
                rigidbody3D = r3D;
                physicsType = PhysicsType.Physics3D;
                return;
            }

            Rigidbody2D r2D = gameObject.GetComponentInParent<Rigidbody2D>();
            if (r2D != null)
            {
                rigidbody2D = r2D;
                physicsType = PhysicsType.Physics2D;
                return;
            }

            physicsType = PhysicsType.Unknown;
            ImpactCFXLogger.LogMissingRigidbody(gameObject);
        }

        /// <summary>
        /// Syncs the cached rigidbody data with the current rigidbody state.
        /// </summary>
        public void SyncRigidbodyData()
        {
            previousRigidbodyState.LinearVelocity = currentVelocity;
            previousRigidbodyState.AngularVelocity = currentAngularVelocity;
            previousRigidbodyState.CenterOfMass = currentWorldCenterOfMass;
        }

        /// <summary>
        /// Gets the full rigidbody data with both previous and current states.
        /// </summary>
        public RigidbodyData GetRigidbodyData()
        {
            return new RigidbodyData(previousRigidbodyState, GetCurrentRigidbodyState());
        }

        /// <summary>
        /// Gets the current rigidbody state, using the values directly from the rigidbody.
        /// </summary>
        public RigidbodyStateData GetCurrentRigidbodyState()
        {
            return new RigidbodyStateData(currentVelocity, currentAngularVelocity, currentWorldCenterOfMass);
        }

        /// <summary>
        /// Gets the previous state of the rigidbody from the last time it was synced (i.e. the previous frame).
        /// </summary>
        public RigidbodyStateData GetPreviousRigidbodyState()
        {
            return previousRigidbodyState;
        }

        /// <summary>
        /// Gets the point's relative position.
        /// </summary>
        /// <param name="point">The point to get the relative position of.</param>
        /// <returns>The point in local space.</returns>
        public Vector3 GetContactPointRelativePosition(Vector3 point)
        {
            return gameObject.transform.worldToLocalMatrix.MultiplyPoint3x4(point);
        }
    }
}