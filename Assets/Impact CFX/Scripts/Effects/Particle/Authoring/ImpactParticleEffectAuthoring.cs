using UnityEngine;

namespace ImpactCFX.Particles
{
    [CreateAssetMenu(fileName = "New Impact Particle Effect", menuName = "Impact CFX/Particle Effect", order = 2)]
    public class ImpactParticleEffectAuthoring : ImpactPooledEffectAuthoringBase
    {
        [Tooltip("The template particle prefab.")]
        [SerializeField]
        private ImpactParticlesBase particlePrefab;
        [Tooltip("Whether the particles are one-shot or looping.")]
        [SerializeField]
        private ParticleEffectType particleEffectType;

        [Tooltip("The minimum velocity required to emit particles.")]
        [SerializeField]
        private float minimumVelocity = 2;
        [Tooltip("How much the normal should affect the collision intensity.")]
        [SerializeField]
        private float collisionNormalInfluence = 1;

        [Tooltip("Should particles be emitted when sliding? Only available for Looping effects.")]
        [SerializeField]
        private bool emitOnSlide;
        [Tooltip("Should particles be emitted when rolling? Only available for Looping effects.")]
        [SerializeField]
        private bool emitOnRoll;

        public override bool Validate()
        {
            return particlePrefab != null;
        }

        public ParticleEffect GetParticleEffect()
        {
            return new ParticleEffect()
            {
                MinimumVelocity = this.minimumVelocity,
                CollisionNormalInfluence = this.collisionNormalInfluence,
                ParticleEffectType = this.particleEffectType,
                EmitOnSlide = this.particleEffectType == ParticleEffectType.Looped && this.emitOnSlide,
                EmitOnRoll = this.particleEffectType == ParticleEffectType.Looped && this.emitOnRoll
            };
        }

        public override PooledEffectObjectBase GetTemplateObject()
        {
            return particlePrefab;
        }
    }
}