using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PixelDough.LowResVolume
{
    [System.Serializable]
    public class LowResRenderer : ScriptableRendererFeature
    {
        private LowResPass pass;

        public override void Create()
        {
            pass = new LowResPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }
    }
}
