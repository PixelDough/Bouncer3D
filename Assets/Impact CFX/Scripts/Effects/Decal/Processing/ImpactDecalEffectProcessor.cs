using ImpactCFX.Pooling;
using UnityEngine;

namespace ImpactCFX.Decals
{
    [AddComponentMenu("Impact CFX/Decal/Impact Decal Effect Processor")]
    [DisallowMultipleComponent]
    public class ImpactDecalEffectProcessor : ImpactPooledEffectProcessor<ImpactDecalEffectAuthoring, DecalEffect, DecalEffectResult, DecalPool, ImpactDecalBase>
    {
        protected override DecalEffect getEffectForPooledEffect(ImpactDecalEffectAuthoring effectAuthoring)
        {
            return effectAuthoring.GetDecalEffect();
        }

        protected override ImpactEffectProcessorJob<DecalEffect, DecalEffectResult> getEffectProcessorJobBase()
        {
            return new ImpactEffectProcessorJob<DecalEffect, DecalEffectResult>();
        }

        protected override ObjectPoolJob<DecalEffectResult> getObjectPoolJobBase()
        {
            return new ObjectPoolJob<DecalEffectResult>();
        }

        public override void PlayPooledEffect(
            DecalEffectResult effectResult,
            ImpactDecalBase pooledObject,
            CollisionResultData collisionResultData)
        {
            base.PlayPooledEffect(effectResult, pooledObject, collisionResultData);

            if (effectResult.IsUpdate)
            {
                pooledObject.UpdateDecalBaseCollision(collisionResultData);
            }
            else
            {
                pooledObject.PlaceDecalBase(collisionResultData, effectResult);
            }
        }

    }
}