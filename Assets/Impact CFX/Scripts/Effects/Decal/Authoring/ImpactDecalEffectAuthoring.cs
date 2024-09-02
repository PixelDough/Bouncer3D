using UnityEngine;

namespace ImpactCFX.Decals
{
    [CreateAssetMenu(fileName = "New Impact Decal Effect", menuName = "Impact CFX/Decal Effect", order = 2)]
    public class ImpactDecalEffectAuthoring : ImpactPooledEffectAuthoringBase
    {
        [Tooltip("The minimum velocity required to place a decal.")]
        [SerializeField]
        private float minimumVelocity = 2;
        [Tooltip("How much the normal should affect the collision intensity.")]
        [SerializeField]
        private float collisionNormalInfluence = 1;
        [Tooltip("The decal prefab to place.")]
        [SerializeField]
        private ImpactDecalBase decalPrefab;

        [Tooltip("Should decals be placed on collision?")]
        [SerializeField]
        private bool createOnCollision = true;
        [Tooltip("Should decals be placed when sliding?")]
        [SerializeField]
        private bool createOnSlide;
        [Tooltip("Should decals be placed when rolling?")]
        [SerializeField]
        private bool createOnRoll;

        [Tooltip("For sliding and rolling, how often a decal should be placed.")]
        [SerializeField]
        private Range creationInterval = new Range(0.5f, 0.5f);
        [Tooltip("For sliding and rolling, the type of interval to use for placement.")]
        [SerializeField]
        private EffectIntervalType creationIntervalType = EffectIntervalType.Distance;

        public override bool Validate()
        {
            return decalPrefab != null;
        }

        public DecalEffect GetDecalEffect()
        {
            return new DecalEffect()
            {
                MinimumVelocity = this.minimumVelocity,
                CollisionNormalInfluence = this.collisionNormalInfluence,
                CreateOnCollision = this.createOnCollision,
                CreateOnSlide = this.createOnSlide,
                CreateOnRoll = this.createOnRoll,
                CreationInterval = this.creationInterval,
                CreationIntervalType = this.creationIntervalType
            };
        }

        public override PooledEffectObjectBase GetTemplateObject()
        {
            return decalPrefab;
        }
    }
}

