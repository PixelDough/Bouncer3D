using ImpactCFX.Pooling;
using UnityEngine;

namespace ImpactCFX.FMOD
{
    public class FMODAudioEffectProxyPool : ObjectPool<FMODAudioEffectProxy>
    {
        private EffectAttachMode effectAttachMode;
        private float audioFadeOutTime;

        public void Setup(EffectAttachMode effectAttachMode, float audioFadeOutTime)
        {
            this.effectAttachMode = effectAttachMode;
            this.audioFadeOutTime = audioFadeOutTime;
        }

        protected override FMODAudioEffectProxy createPooledObjectInstance(int index)
        {
            GameObject g = new GameObject("FMOD Audio Effect Proxy " + index);
            FMODAudioEffectProxy f = g.AddComponent<FMODAudioEffectProxy>();

            f.EffectAttachMode = effectAttachMode;
            f.AudioFadeOutTime = audioFadeOutTime;

            f.SetParentPoolGameObject(this.gameObject);
            f.ReturnToPool();

            return f;
        }
    }
}
