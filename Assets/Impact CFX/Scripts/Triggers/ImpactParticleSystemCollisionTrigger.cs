using System.Collections.Generic;
using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Special trigger for handling collisions with particle systems.
    /// </summary>
    [AddComponentMenu("Impact CFX/Impact Particle System Collision Trigger")]
    [DisallowMultipleComponent]
    public class ImpactParticleSystemCollisionTrigger : MonoBehaviour
    {
        [Tooltip("Is the trigger enabled?")]
        [SerializeField]
        private bool triggerEnabled = true;
        [Tooltip("The Impact Object the trigger will use.")]
        [SerializeField]
        private ImpactObjectBase impactObject;
        [Tooltip("Is this object a particle system?")]
        [SerializeField]
        private bool isParticleSystem;
        [Tooltip("The particle system associated with this trigger, if any.")]
        [SerializeField]
        private ParticleSystem particles;
        [SerializeField]
        [Tooltip("An optional cooldown between collisions.")]
        private float cooldown;
        [Tooltip("An optional threshold that the collision velocity must be greater than to trigger an effect.")]
        [SerializeField]
        private float threshold;
        [Tooltip("The number of materials to get for the object that was hit.")]
        [SerializeField]
        protected int otherMaterialCount = 1;

        /// <summary>
        /// Is the trigger enabled?
        /// </summary>
        public bool TriggerEnabled { get => triggerEnabled; set => triggerEnabled = value; }

        /// <summary>
        /// The Impact Object the trigger will use to trigger effects.
        /// </summary>
        public ImpactObjectBase ImpactObject { get => impactObject; set => impactObject = value; }

        /// <summary>
        /// An optional threshold that the collision velocity must be greater than to trigger an effect.
        /// </summary>
        public float Threshold { get => threshold; set => threshold = value; }

        /// <summary>
        /// An optional cooldown between collisions.
        /// </summary>
        public float Cooldown { get => cooldown; set => cooldown = value; }

        /// <summary>
        /// The particle system associated with this trigger, if any.
        /// </summary>
        public ParticleSystem Particles { get => particles; set => particles = value; }

        /// <summary>
        /// Is this object a particle system?
        /// </summary>
        public bool IsParticleSystem { get => isParticleSystem; set => isParticleSystem = value; }

        private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        private float lastCollisionTime;

        private void Reset()
        {
            impactObject = GetComponentInParent<ImpactObjectBase>();
            particles = GetComponent<ParticleSystem>();
            isParticleSystem = particles != null;
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!triggerEnabled || !ImpactCFXGlobal.CanQueueCollision(CollisionType.Collision, 1 + otherMaterialCount) || !checkCooldown())
                return;

            lastCollisionTime = Time.time;
            int numCollisionEvents = 0;

            //If this is a particle system, get the collision events from out particles
            if (isParticleSystem)
                numCollisionEvents = particles.GetCollisionEvents(other, collisionEvents);
            //This is a non-particle system object, get collision events from the other object
            else
            {
                ParticleSystem p = other.GetComponent<ParticleSystem>();
                numCollisionEvents = p.GetCollisionEvents(this.gameObject, collisionEvents);
            }

            //Process collision for each particle collision event
            for (int i = 0; i < numCollisionEvents; i++)
            {
                ParticleCollisionEvent e = collisionEvents[i];

                if (e.velocity.sqrMagnitude < threshold)
                    continue;

                IImpactObject hitObject = e.colliderComponent.GetComponentInParent<IImpactObject>();
                if (e.colliderComponent is Collider collider3D)
                {
                    ImpactContactPoint contactPoint = new ImpactContactPoint(e.intersection, e.normal, impactObject.gameObject, collider3D, e.velocity);
                    ImpactCFXGlobal.QueueCollision(impactObject, hitObject, contactPoint, CollisionType.Collision, 1, otherMaterialCount, CollisionVelocityMethod.CollisionMessage);
                }
                else if (e.colliderComponent is Collider2D collider2D)
                {
                    ImpactContactPoint contactPoint = new ImpactContactPoint(e.intersection, e.normal, impactObject.gameObject, collider2D, e.velocity);
                    ImpactCFXGlobal.QueueCollision(impactObject, hitObject, contactPoint, CollisionType.Collision, 1, otherMaterialCount, CollisionVelocityMethod.CollisionMessage);
                }
            }
        }

        private bool checkCooldown()
        {
            return Time.time - lastCollisionTime > cooldown;
        }
    }
}