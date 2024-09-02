using UnityEngine;

namespace ImpactCFX
{
    /// <summary>
    /// Wraps data for a 3D or 2D collision.
    /// </summary>
    public struct ImpactCollision
    {
        private Collision collision3D;
        private Collision2D collision2D;

        private PhysicsType physicsType;

        /// <summary>
        /// The number of contact points.
        /// </summary>
        public int ContactCount
        {
            get
            {
                if (physicsType == PhysicsType.Physics3D)
                    return collision3D.contactCount;
                else if (physicsType == PhysicsType.Physics2D)
                    return collision2D.contactCount;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Create a wrapper for a 3D collision.
        /// </summary>
        public ImpactCollision(Collision source)
        {
            collision3D = source;
            collision2D = null;

            physicsType = PhysicsType.Physics3D;
        }

        /// <summary>
        /// Create a wrapper for a 2D collision.
        /// </summary>
        public ImpactCollision(Collision2D source)
        {
            collision3D = null;
            collision2D = source;

            physicsType = PhysicsType.Physics2D;
        }

        /// <summary>
        /// Get the contact point at the given index.
        /// </summary>
        public ImpactContactPoint GetContact(int index)
        {
            if (physicsType == PhysicsType.Physics3D)
                return new ImpactContactPoint(collision3D.GetContact(index), collision3D.relativeVelocity);
            else if (physicsType == PhysicsType.Physics2D)
                return new ImpactContactPoint(collision2D.GetContact(index), collision2D.relativeVelocity);
            else
                return default(ImpactContactPoint);
        }
    }
}
