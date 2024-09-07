using UnityEngine.Rendering.Universal;

namespace Dustyroom {
public class ColorRemap : GenericRendererFeature {
    public ColorRemap() {
        requirements = ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth |
                       ScriptableRenderPassInput.Normal;
        injectionPoint = InjectionPoint.BeforeRenderingPostProcessing;
        fetchColorBuffer = true;
    }
}
}