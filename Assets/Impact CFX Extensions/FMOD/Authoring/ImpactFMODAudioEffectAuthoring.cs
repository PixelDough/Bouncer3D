using FMODUnity;
using UnityEngine;

namespace ImpactCFX.FMOD
{
    [CreateAssetMenu(fileName = "New Impact FMOD Audio Effect", menuName = "Impact CFX/FMOD Audio Effect", order = 2)]
    public class ImpactFMODAudioEffectAuthoring : ImpactEffectAuthoringBase
    {
        [Tooltip("The FMOD event to call for Collisions.")]
        [SerializeField]
        private EventReference collisionEvent;
        [Tooltip("The FMOD event to call for Sliding.")]
        [SerializeField]
        private EventReference slideEvent;
        [Tooltip("The FMOD event to call for Rolling.")]
        [SerializeField]
        private EventReference rollEvent;

        [Tooltip("The velocity magnitude range to use when calculating collision intensity.")]
        [SerializeField]
        private Range velocityReferenceRange = new Range(1, 10);
        [Tooltip("How much the normal should affect the collision intensity.")]
        [SerializeField]
        private float collisionNormalInfluence = 1;

        public FMODAudioEffect GetFMODAudioEffect()
        {
            return new FMODAudioEffect()
            {
                EffectID = GetID(),

                CollisionEventGUID = collisionEvent.Guid,
                SlideEventGUID = slideEvent.Guid,
                RollEventGUID = rollEvent.Guid,

                VelocityReferenceRange = velocityReferenceRange,
                CollisionNormalInfluence = collisionNormalInfluence
            };
        }
    }
}