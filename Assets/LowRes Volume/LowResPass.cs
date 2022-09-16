using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PixelDough.LowResVolume
{
    [System.Serializable]
    public class LowResPass : ScriptableRenderPass
    {
        // A variable to hold a reference to the corresponding volume component
        private LowResVolume m_VolumeComponent;

        // The postprocessing material
        private Material m_Material;

        // The ids of the shader variables
        static class ShaderIDs
        {
            internal static readonly int Input = Shader.PropertyToID("_MainTex");
            internal static readonly int ReferenceResolution = Shader.PropertyToID("_ReferenceResolution");
            internal static readonly int ScreenSizeGivenReferenceResolution =
                Shader.PropertyToID("_ScreenSizeGivenReferenceResolution");
        }

        private RenderTargetIdentifier source;
        private RenderTargetIdentifier destinationA;
        private RenderTargetIdentifier destinationB;
        private RenderTargetIdentifier latestDest;

        readonly int temporaryRTIdA = Shader.PropertyToID("_TempRT");
        readonly int temporaryRTIdB = Shader.PropertyToID("_TempRTB");

        public LowResPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            if (m_Material == null)
            {
                Debug.Log("Creating Low Res Material");
                m_Material = CoreUtils.CreateEngineMaterial("Hidden/PixelDough/PostProcessing/LowRes");
            }
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // if (renderingData.cameraData.isSceneViewCamera)
            //     return;

            if (m_Material == null)
            {
                Debug.Log("Creating Low Res Material");
                m_Material = CoreUtils.CreateEngineMaterial("Hidden/PixelDough/PostProcessing/LowRes");
            }

            CommandBuffer cmd = CommandBufferPool.Get("LowRes Volume");
            cmd.Clear();

            #region From Setup

            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            var renderer = renderingData.cameraData.renderer;
            source = renderer.cameraColorTarget;

            cmd.GetTemporaryRT(temporaryRTIdA, descriptor, FilterMode.Bilinear);
            destinationA = new RenderTargetIdentifier(temporaryRTIdA);
            cmd.GetTemporaryRT(temporaryRTIdB, descriptor, FilterMode.Bilinear);
            destinationB = new RenderTargetIdentifier(temporaryRTIdB);

            #endregion


            var stack = VolumeManager.instance.stack;

            void BlitTo(Material mat, int pass = 0)
            {
                var first = latestDest;
                var last = first == destinationA ? destinationB : destinationA;
                Blit(cmd, first, last, mat, pass);

                latestDest = last;
            }

            latestDest = source;

            m_VolumeComponent = stack.GetComponent<LowResVolume>();
            if (m_VolumeComponent.IsActive())
            {
                // set material properties
                if (m_Material != null)
                {
                    m_Material.SetInt(ShaderIDs.ReferenceResolution, m_VolumeComponent.referenceResolution.value);

                    Vector2 screenSize = new Vector2(Screen.width, Screen.height);
                    float aspectRatio = screenSize.y / screenSize.x;
                    float rasterizedScreenWidth = m_VolumeComponent.referenceResolution.value / aspectRatio;
                    m_Material.SetVector(ShaderIDs.ScreenSizeGivenReferenceResolution,
                        new Vector4(rasterizedScreenWidth, m_VolumeComponent.referenceResolution.value));
                }

                cmd.SetGlobalVector("_ScreenSize",
                    new Vector4(Screen.width, Screen.height, 1.0f / (float) Screen.width,
                        1.0f / (float) Screen.height));
                
                // set source texture
                cmd.SetGlobalTexture(ShaderIDs.Input, source);

                //cmd.Blit(source, destination, m_Material, 0);
                //CoreUtils.DrawFullScreen(cmd, m_Material, destination);

                BlitTo(m_Material);
            }

            Blit(cmd, latestDest, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}