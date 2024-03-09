// WaterCausticsModules
// Copyright (c) 2021 Masataka Hakozaki

#if WCE_URP
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MH.WaterCausticsModules {
    /*------------------------------------------------------------------------ 
    This Renderer Function is required to apply WaterCausticsEffect.
    -------------------------------------------------------------------------*/

#if UNITY_2021_2_OR_NEWER
    [DisallowMultipleRendererFeature ("WaterCausticsEffect (Renderer Feature)")]
#elif WCE_URP_10_8
    [DisallowMultipleRendererFeature]
#endif
    [HelpURL (Constant.URL_MANUAL)]
    public class WaterCausticsEffectFeature : ScriptableRendererFeature {
        static private WaterCausticsEffectFeature s_ins;
        static public event Action<Camera> onCamRender;
        static public event Action<ScriptableRenderer, Camera> onEnqueue;
        static private int s_lastFrame;
        static internal bool effective => (s_ins != null && s_ins.isActive && s_lastFrame >= Time.renderedFrameCount - 1);
        static internal void OnAddedByScript () => s_lastFrame = Time.renderedFrameCount;
        public override void Create () { }
        public override void AddRenderPasses (ScriptableRenderer renderer, ref RenderingData rendData) {
            s_ins = this;
            s_lastFrame = Time.renderedFrameCount;
            var cam = rendData.cameraData.camera;
            onCamRender?.Invoke (cam);
            onEnqueue?.Invoke (renderer, cam);
        }
    }
}
#endif
