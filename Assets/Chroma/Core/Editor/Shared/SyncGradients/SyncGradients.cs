using UnityEngine;

namespace Chroma {
[CreateAssetMenu(fileName = "Synchronized Gradient List", menuName = "Chroma/Gradient Synchronizer", order = 6000)]
public class SyncGradients : ScriptableObject {
    [Tooltip("The gradient to apply to the materials.")]
    public Gradient gradient;

    [Tooltip("The name of the shader property of the gradient. This is the \"Reference\" name in Shader Graph and " +
             "the \"Property Name\" in the code shaders, e.g. \"_Gradient_Shading\".")]
    public string referenceName;

    [Tooltip("The resolution of the gradient texture. Higher resolution will result in smoother gradients, but " +
             "will also use more memory.")]
    public int resolution = 256;

    [Tooltip("The materials to apply the gradient to.")]
    public Material[] materials;
}
}