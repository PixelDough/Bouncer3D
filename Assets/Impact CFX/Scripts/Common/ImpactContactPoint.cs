using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Wraps data for a single 3D or 2D contact point.
    /// </summary>
    public struct ImpactContactPoint
    {
        /// <summary>
        /// The position of the contact point.
        /// </summary>
        public Vector3 Point;

        /// <summary>
        /// The normal of the contact.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The specific collider object that triggered the collision.
        /// </summary>
        public GameObject TriggerObject;

        /// <summary>
        /// The collider object being collided with.
        /// </summary>
        public GameObject HitObject;

        /// <summary>
        /// The collision velocity provided by the collision message.
        /// </summary>
        public Vector3 CollisionMessageVelocity;

        private Collider thisCollider3D;
        private Collider2D thisCollider2D;

        private Collider otherCollider3D;
        private Collider2D otherCollider2D;

        private PhysicsType physicsType;

        public ImpactContactPoint(ContactPoint contactPoint3D, Vector3 collisionMessageVelocity)
        {
            Point = contactPoint3D.point;
            Normal = contactPoint3D.normal;

            thisCollider3D = contactPoint3D.thisCollider;
            thisCollider2D = default(Collider2D);

            otherCollider3D = contactPoint3D.otherCollider;
            otherCollider2D = default(Collider2D);

            TriggerObject = thisCollider3D.gameObject;
            HitObject = otherCollider3D.gameObject;

            physicsType = PhysicsType.Physics3D;

            CollisionMessageVelocity = collisionMessageVelocity;
        }

        public ImpactContactPoint(ContactPoint2D contactPoint2D, Vector3 collisionMessageVelocity)
        {
            Point = contactPoint2D.point;
            Normal = contactPoint2D.normal;

            thisCollider2D = contactPoint2D.otherCollider;
            thisCollider3D = default(Collider);

            otherCollider2D = contactPoint2D.collider;
            otherCollider3D = default(Collider);

            TriggerObject = thisCollider2D.gameObject;
            HitObject = otherCollider2D.gameObject;

            physicsType = PhysicsType.Physics2D;

            CollisionMessageVelocity = collisionMessageVelocity;
        }

        public ImpactContactPoint(RaycastHit raycastHit3D, bool invert, GameObject sourceObject = null)
        {
            Point = raycastHit3D.point;
            Normal = raycastHit3D.normal;

            if (invert)
            {
                thisCollider3D = raycastHit3D.collider;
                otherCollider3D = default(Collider);

                TriggerObject = thisCollider3D.gameObject;
                HitObject = sourceObject;
            }
            else
            {
                thisCollider3D = default(Collider);
                otherCollider3D = raycastHit3D.collider;

                TriggerObject = sourceObject;
                HitObject = otherCollider3D.gameObject;
            }

            thisCollider2D = default(Collider2D);
            otherCollider2D = default(Collider2D);

            physicsType = PhysicsType.Physics3D;

            CollisionMessageVelocity = Vector3.zero;
        }

        public ImpactContactPoint(RaycastHit2D raycastHit2D, bool invert, GameObject sourceObject = null)
        {
            Point = raycastHit2D.point;
            Normal = raycastHit2D.normal;

            if (invert)
            {
                thisCollider2D = raycastHit2D.collider;
                otherCollider2D = default(Collider2D);

                TriggerObject = thisCollider2D.gameObject;
                HitObject = sourceObject;
            }
            else
            {
                thisCollider2D = default(Collider2D);
                otherCollider2D = raycastHit2D.collider;

                TriggerObject = sourceObject;
                HitObject = otherCollider2D.gameObject;
            }

            thisCollider3D = default(Collider);
            otherCollider3D = default(Collider);

            physicsType = PhysicsType.Physics2D;

            CollisionMessageVelocity = Vector3.zero;
        }

        public ImpactContactPoint(Vector3 point, Vector3 normal, GameObject triggerObject, Collider hitCollider3D, Vector3 collisionMessageVelocity)
        {
            Point = point;
            Normal = normal;

            TriggerObject = triggerObject;
            HitObject = hitCollider3D.gameObject;

            thisCollider3D = default(Collider);
            thisCollider2D = default(Collider2D);

            this.otherCollider3D = hitCollider3D;
            otherCollider2D = default(Collider2D);

            physicsType = PhysicsType.Physics3D;

            CollisionMessageVelocity = collisionMessageVelocity;
        }

        public ImpactContactPoint(Vector3 point, Vector3 normal, GameObject triggerObject, Collider2D hitCollider2D, Vector3 collisionMessageVelocity)
        {
            Point = point;
            Normal = normal;

            TriggerObject = triggerObject;
            HitObject = hitCollider2D.gameObject;

            thisCollider3D = default(Collider);
            thisCollider2D = default(Collider2D);

            this.otherCollider2D = hitCollider2D;
            otherCollider3D = default(Collider);

            physicsType = PhysicsType.Physics3D;

            CollisionMessageVelocity = collisionMessageVelocity;
        }

        public ImpactContactPoint(Vector3 point, Vector3 normal, GameObject triggerObject, PhysicsType physicsType)
        {
            Point = point;
            Normal = normal;

            TriggerObject = triggerObject;
            HitObject = null;

            thisCollider3D = default(Collider);
            thisCollider2D = default(Collider2D);

            otherCollider3D = default(Collider);
            otherCollider2D = default(Collider2D);

            this.physicsType = physicsType;

            CollisionMessageVelocity = Vector3.zero;
        }

        /// <summary>
        /// Get the ID of the physics material belonging to the collider that triggered the collision.
        /// </summary>
        /// <returns>The ID of the physics material. Will return 0 if there is no material.</returns>
        public int GetTriggerObjectPhysicsMaterialID()
        {
            if (physicsType == PhysicsType.Physics3D && thisCollider3D != null)
            {
                PhysicMaterial physicMaterial = thisCollider3D.sharedMaterial;
                if (physicMaterial != null)
                {
                    return physicMaterial.GetInstanceID();
                }
            }
            else if (physicsType == PhysicsType.Physics2D && thisCollider2D != null)
            {
                PhysicsMaterial2D physicsMaterial2D = thisCollider2D.sharedMaterial;
                if (physicsMaterial2D != null)
                {
                    return physicsMaterial2D.GetInstanceID();
                }
            }

            return 0;
        }

        /// <summary>
        /// Get the ID of the physics material belonging to the collider being collided with.
        /// </summary>
        /// <returns>The ID of the physics material. Will return 0 if there is no material.</returns>
        public int GetHitObjectPhysicsMaterialID()
        {
            if (physicsType == PhysicsType.Physics3D && otherCollider3D != null)
            {
                PhysicMaterial physicMaterial = otherCollider3D.sharedMaterial;
                if (physicMaterial != null)
                {
                    return physicMaterial.GetInstanceID();
                }
            }
            else if (physicsType == PhysicsType.Physics2D && otherCollider2D != null)
            {
                PhysicsMaterial2D physicsMaterial2D = otherCollider2D.sharedMaterial;
                if (physicsMaterial2D != null)
                {
                    return physicsMaterial2D.GetInstanceID();
                }
            }

            return 0;
        }

        /// <summary>
        /// Get the ID the object that triggered the collision.
        /// </summary>
        /// <returns>The ID of the object that triggered the collision. Will return 0 if the object is not known.</returns>
        public int GetTriggerObjectID()
        {
            if (TriggerObject != null)
                return TriggerObject.GetInstanceID();
            else
                return 0;
        }

        /// <summary>
        /// Get the ID the object that was hit.
        /// </summary>
        /// <returns>The ID the object that was hit. Will return 0 if the object is not known.</returns>
        public int GetHitObjectID()
        {
            if (HitObject != null)
                return HitObject.GetInstanceID();
            else
                return 0;
        }
    }
}
