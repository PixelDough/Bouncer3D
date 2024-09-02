using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImpactCFX.Triggers
{
    /// <summary>
    /// Base class for speculative collision triggers that attempt to catch collision events that OnCollisionEnter by itself might miss.
    /// </summary>
    public abstract class ImpactSpeculativeCollisionTriggerBase : ImpactCollisionTriggerBase
    {
        private struct SpeculativeCollisionContact : IEquatable<SpeculativeCollisionContact>
        {
            public int Lifetime;
            public Vector3 RelativeContactPoint;

            public bool IsAlive
            {
                get { return Lifetime > 0; }
            }

            public bool Equals(SpeculativeCollisionContact other)
            {
                return RelativeContactPoint.Equals(other.RelativeContactPoint);
            }
        }

        [Tooltip("The maximum number of collisions that can be triggered in a single frame.")]
        [SerializeField]
        private int maxCollisionsPerFrame = 2;
        [Tooltip("How close contact points need to be for them to be grouped as the same contact point. Increasing this value will lead to fewer contact points.")]
        [SerializeField]
        private float contactPointComparisonThreshold = 0.25f;
        [Tooltip("How many frames should a contact point be alive for before it is removed from the list of active contacts? Increasing this value can reduce the likelyhood of interactions happening in quick succession for the same contact point.")]
        [SerializeField]
        private int contactPointLifetime = 2;

        /// <summary>
        /// The maximum number of collisions that can be triggered in a single frame.
        /// </summary>
        public int MaxCollisionsPerFrame { get => maxCollisionsPerFrame; set => maxCollisionsPerFrame = value; }

        /// <summary>
        /// How close contact points need to be for them to be grouped as the same contact point. 
        /// Increasing this value will lead to fewer contact points.
        /// </summary>
        public float ContactPointComparisonThreshold { get => contactPointComparisonThreshold; set => contactPointComparisonThreshold = value; }

        /// <summary>
        /// How many frames should a contact point be alive for before it is removed from the list of active contacts? 
        /// Increasing this value can reduce the likelyhood of interactions happening in quick succession for the same contact point.
        /// </summary>
        public int ContactPointLifetime { get => contactPointLifetime; set => contactPointLifetime = value; }

        private List<SpeculativeCollisionContact> continousCollisionContacts = new List<SpeculativeCollisionContact>();

        private void FixedUpdate()
        {
            for (int i = 0; i < continousCollisionContacts.Count; i++)
            {
                if (!continousCollisionContacts[i].IsAlive)
                {
                    continousCollisionContacts.RemoveAt(i);
                    i--;
                }
                else
                {
                    SpeculativeCollisionContact c = continousCollisionContacts[i];
                    c.Lifetime -= 1;
                    continousCollisionContacts[i] = c;
                }
            }
        }

        protected void triggerSpeculativeCollision(ImpactCollision collision)
        {
            int collisions = 0;

            //Loop over all contacts of the collision
            for (int i = 0; i < collision.ContactCount; i++)
            {
                ImpactContactPoint contactPoint = collision.GetContact(i);
                IImpactObject triggerObject = getImpactObject(contactPoint.TriggerObject);

                if (triggerObject != null)
                {
                    SpeculativeCollisionContact c = new SpeculativeCollisionContact()
                    {
                        RelativeContactPoint = triggerObject.GetContactPointRelativePosition(contactPoint.Point),
                        Lifetime = contactPointLifetime
                    };

                    //The relative position of the contact point serves as a unique identifier
                    //So we can keep track of contact points between frames
                    //If there is no existing point with the same relative position (within a threshold), then it is a "new" contact point.
                    //We then process the point, basically mimicking an OnCollisionEnter message.
                    int existingIndex = continousCollisionContacts.FindIndex(e => (e.RelativeContactPoint - c.RelativeContactPoint).sqrMagnitude < contactPointComparisonThreshold);
                    if (existingIndex == -1)
                    {
                        continousCollisionContacts.Add(c);

                        if (collisions < maxCollisionsPerFrame)
                        {
                            triggerCollisionContact(contactPoint);
                            collisions++;
                        }
                    }
                    else
                    {
                        SpeculativeCollisionContact existingContact = continousCollisionContacts[existingIndex];
                        existingContact.Lifetime = contactPointLifetime;
                        continousCollisionContacts[existingIndex] = existingContact;
                    }
                }
            }
        }
    }
}

