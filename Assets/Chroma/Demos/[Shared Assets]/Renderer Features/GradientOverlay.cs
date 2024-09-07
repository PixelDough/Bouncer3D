using UnityEngine.Rendering.Universal;

namespace Dustyroom {
public class GradientOverlay : GenericRendererFeature {
    public GradientOverlay() {
        requirements = ScriptableRenderPassInput.Color;
        injectionPoint = InjectionPoint.BeforeRenderingPostProcessing;
        fetchColorBuffer = true;
    }
}
}