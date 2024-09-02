using ImpactCFX.Pooling;
using UnityEngine;

namespace ImpactCFX.Particles
{
    [AddComponentMenu("Impact CFX/Particle/Impact Particle Effect Processor")]
    [DisallowMultipleComponent]
    public class ImpactParticleEffectProcessor : ImpactPooledEffectProcessor<ImpactParticleEffectAuthoring, ParticleEffect, ParticleEffectResult, ParticlePool, ImpactParticlesBase>
    {
        protected override ParticleEffect getEffectForPooledEffect(ImpactParticleEffectAuthoring effectAuthoring)
        {
            return effectAuthoring.GetParticleEffect();
        }

        protected override ObjectPoolJob<ParticleEffectResult> getObjectPoolJobBase()
        {
            return new ObjectPoolJob<ParticleEffectResult>();
        }

        protected override ImpactEffectProcessorJob<ParticleEffect, ParticleEffectResult> getEffectProcessorJobBase()
        {
            return new ImpactEffectProcessorJob<ParticleEffect, ParticleEffectResult>();
        }

        public override void PlayPooledEffect(ParticleEffectResult effectResult,
            ImpactParticlesBase pooledObject,
            CollisionResultData collisionResultData)
        {
            base.PlayPooledEffect(effectResult, pooledObject, collisionResultData);

            if (effectResult.IsUpdate)
            {
                pooledObject.UpdateParticlesBaseCollision(collisionResultData);
            }
            else
            {
                pooledObject.EmitParticlesBase(collisionResultData);
            }
        }
    }
}