using UnityEngine.Rendering.Universal;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Dustyroom {
public class WaterReflection : GenericRendererFeature {
    public WaterReflection() {
        requirements = ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth;
        injectionPoint = InjectionPoint.BeforeRenderingPostProcessing;
        fetchColorBuffer = true;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
        base.SetupRenderPasses(renderer, in renderingData);

        // Feed the camera color image to the shader as "_MainTex".
        var material = base.passMaterial;
        if (material != null) {
            material.SetTexture("_MainTex", renderingData.cameraData.renderer.cameraColorTargetHandle);
        }
    }
}
}