using UnityEditor;
using UnityEngine;

namespace Chroma {
[CustomEditor(typeof(SyncGradients))]
public class SyncGradientsEditor : Editor {
    private SyncGradients _syncGradients;

    private void OnEnable() {
        _syncGradients = (SyncGradients)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var canApply = GUI.enabled;

        if (_syncGradients.materials.Length == 0) {
            EditorGUILayout.HelpBox("No materials assigned.", MessageType.Warning);
            canApply = false;
        }

        if (string.IsNullOrEmpty(_syncGradients.referenceName)) {
            EditorGUILayout.HelpBox("No property name assigned.", MessageType.Warning);
            canApply = false;
        }

        // Check if each material has the property.
        {
            var materials = _syncGradients.materials;
            var referenceName = _syncGradients.referenceName;
            var hasProperty = true;
            Material material = null;
            foreach (var m in materials) {
                material = m;
                if (!material.HasProperty(referenceName)) {
                    hasProperty = false;
                    break;
                }
            }

            if (!hasProperty) {
                var m = $"The property \"{referenceName}\" is not found in the material \"{material.name}\".";
                EditorGUILayout.HelpBox(m, MessageType.Error);
                canApply = false;
            }
        }

        GUI.enabled = canApply;
        var tooltip = canApply ? "Apply the gradient to all materials." : "Please fix the above issues.";
        if (GUILayout.Button(new GUIContent("Apply", tooltip), GUILayout.Height(30))) {
            var encodedGradient = DrawerUtils.Serialize(_syncGradients.gradient);
            var namePrefix = DrawerUtils.TextureNamePrefix(_syncGradients.referenceName);
            var fullAssetName = namePrefix + encodedGradient;

            foreach (var material in _syncGradients.materials) {
                if (!AssetDatabase.Contains(material)) continue;
                var path = AssetDatabase.GetAssetPath(material);

                var filterMode = _syncGradients.gradient.mode == GradientMode.Blend
                    ? FilterMode.Bilinear
                    : FilterMode.Point;
                var textureAsset =
                    DrawerUtils.GetOrCreateSubAssetTexture(path, namePrefix, filterMode, _syncGradients.resolution,
                                                           hdr: false);
                Undo.RecordObject(textureAsset, "Change Material Gradient");
                textureAsset.name = fullAssetName;
                DrawerUtils.Bake(_syncGradients.gradient, textureAsset);

                material.SetTexture(_syncGradients.referenceName, textureAsset);
                EditorUtility.SetDirty(material);
            }
        }
    }
}
}