// WaterCausticsModules
// Copyright (c) 2021 Masataka Hakozaki

#if WCE_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_2022_3_OR_NEWER
using UnityEngine.Rendering.RendererUtils;
#endif

namespace MH.WaterCausticsModules {
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu ("")]
    public class DEMO_DrawEffectPass : MonoBehaviour {
        public string m_ShaderTag = "WCE_DEMO_UnderWaterEffect";
        public RenderPassEvent m_RenderEventAboveWater = RenderPassEvent.AfterRenderingSkybox;
        public RenderPassEvent m_RenderEventUnderWater = RenderPassEvent.AfterRenderingTransparents;

        private void OnEnable () {
            WaterCausticsEffectFeature.onEnqueue -= enqueuePass;
            WaterCausticsEffectFeature.onEnqueue += enqueuePass;
        }

        private void OnDisable () {
            WaterCausticsEffectFeature.onEnqueue -= enqueuePass;
        }

        private WCE_DEMO_DrawEffectPass _pass;
        private void enqueuePass (ScriptableRenderer renderer, Camera cam) {
            _pass ??= new WCE_DEMO_DrawEffectPass ();
            var camY = cam.transform.position.y;
            float surfY = Shader.GetGlobalFloat ("_WCECF_SurfaceY");
            var evt = surfY < camY ? m_RenderEventAboveWater : m_RenderEventUnderWater;
            _pass.Setup (evt, m_ShaderTag);
            renderer.EnqueuePass (_pass);
        }

        // ----------------------------------------------------------- ScriptableRenderPass
        internal class WCE_DEMO_DrawEffectPass : ScriptableRenderPass {
            private string _shaderTag;
            private ShaderTagId _shaderTagId;

            internal WCE_DEMO_DrawEffectPass () {
                base.profilingSampler = new ProfilingSampler (nameof (WCE_DEMO_DrawEffectPass));
            }

            internal void Setup (RenderPassEvent evt, string shaderTag) {
                this.renderPassEvent = evt;
                if (_shaderTag != shaderTag) {
                    _shaderTag = shaderTag;
                    _shaderTagId = new ShaderTagId (shaderTag);
                }
            }

            public override void Execute (ScriptableRenderContext context, ref RenderingData renderingData) {
                CommandBuffer cmd = CommandBufferPool.Get ();
                using (new ProfilingScope (cmd, base.profilingSampler)) {
                    context.ExecuteCommandBuffer (cmd);
                    cmd.Clear ();
#if UNITY_2022_3_OR_NEWER
                    var desc = new RendererListDesc (_shaderTagId, renderingData.cullResults, renderingData.cameraData.camera) {
                        sortingCriteria = SortingCriteria.None,
                        renderQueueRange = RenderQueueRange.all,
                        layerMask = ~0,
                    };
                    var rendererList = context.CreateRendererList (desc);
                    cmd.DrawRendererList (rendererList);
#else
                    var drawingSettings = CreateDrawingSettings (_shaderTagId, ref renderingData, SortingCriteria.None);
                    var filteringSettings = new FilteringSettings (RenderQueueRange.all, layerMask: ~0);
                    context.DrawRenderers (renderingData.cullResults, ref drawingSettings, ref filteringSettings);
#endif
                }
                context.ExecuteCommandBuffer (cmd);
                CommandBufferPool.Release (cmd);
            }
        }

    }
}
#endif // end of WCE_URP
