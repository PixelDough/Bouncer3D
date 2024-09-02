using FMODUnity;
using Impact.Interactions;
using Impact.Utility;
using UnityEngine;

namespace Impact.Integration.FMOD

{    /// <summary>
     /// Interaction for playing audio with FMOD.
     /// </summary>
    [CreateAssetMenu(fileName = "New FMOD Audio Interaction", menuName = "Impact/FMOD/FMOD Audio Interaction", order = 0)]
    public class FMODAudioInteraction : ImpactInteractionBase
    {
        private const string interactionResultPoolKey = "FMODAudioInteractionResult";

        [SerializeField]
        [EventRef]
        private string _collisionEvent = "";
        [SerializeField]
        [EventRef]
        private string _slideEvent = "";
        [SerializeField]
        [EventRef]
        private string _rollEvent = "";

        [SerializeField]
        private Range _velocityRange = new Range(2, 9);
        [SerializeField]
        private float _collisionNormalInfluence = 1;

        /// <summary>
        /// The name of the FMOD event to use for collisions.
        /// </summary>
        public string CollisionEvent
        {
            get { return _collisionEvent; }
            set { _collisionEvent = value; }
        }

        /// <summary>
        /// The name of the FMOD event to use for sliding.
        /// </summary>
        public string SlideEvent
        {
            get { return _slideEvent; }
            set { _slideEvent = value; }
        }

        /// <summary>
        /// The name of the FMOD event to use for rolling.
        /// </summary>
        public string RollEvent
        {
            get { return _rollEvent; }
            set { _rollEvent = value; }
        }

        /// <summary>
        /// The range input velocities will be compared to when calculating the collision intensity.
        /// </summary>
        public Range VelocityRange
        {
            get { return _velocityRange; }
            set { _velocityRange = value; }
        }

        /// <summary>
        /// How much the collision normal affects the collision intensity.
        /// </summary>
        public float CollisionNormalInfluence
        {
            get { return _collisionNormalInfluence; }
            set { _collisionNormalInfluence = value; }
        }

        public override IInteractionResult GetInteractionResult<T>(T interactionData)
        {
            //Immediately break out if intensity is less than the velocity range minimum, since any result would be invalid anyways.
            float intensity = ImpactInteractionUtilities.GetCollisionIntensity(interactionData, CollisionNormalInfluence);
            if (intensity < VelocityRange.Min)
                return null;

            long key = 0;
            if (!ImpactInteractionUtilities.GetKeyAndValidate(interactionData, this, out key))
                return null;

            FMODAudioInteractionResult result;

            if (ImpactManagerInstance.TryGetInteractionResultFromPool(interactionResultPoolKey, out result))
            {
                result.OriginalData = InteractionDataUtilities.ToInteractionData(interactionData);

                if (interactionData.InteractionType == InteractionData.InteractionTypeSimple)
                {
                    result.Intensity = 1;
                    result.Event = _collisionEvent;
                }
                else
                {

                    result.Event = getEvent(interactionData.InteractionType);
                    result.Intensity = VelocityRange.Normalize(intensity);
                    result.Key = key;
                }

                return result;
            }

            return null;
        }

        private string getEvent(int interactionType)
        {
            if (interactionType == InteractionData.InteractionTypeCollision)
                return _collisionEvent;
            else if (interactionType == InteractionData.InteractionTypeSlide)
                return _slideEvent;
            else if (interactionType == InteractionData.InteractionTypeRoll)
                return _rollEvent;

            return null;
        }

        public override void Preload()
        {
            ImpactManagerInstance.CreateInteractionResultPool<FMODAudioInteractionResult>(interactionResultPoolKey);
        }
    }
}

