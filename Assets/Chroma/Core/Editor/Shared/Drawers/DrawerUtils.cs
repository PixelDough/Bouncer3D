using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chroma {
public static class DrawerUtils {
    public static Texture2D LoadSubAsset(string path, string name) {
        var assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(path);
        Debug.Assert(assetsAtPath != null, $"[Chroma] Failed to load assets at path {path}");
        var subAsset = assetsAtPath.FirstOrDefault(asset => asset != null && asset.name.StartsWith(name));
        return subAsset as Texture2D;
    }

    public static void Bake(Gradient gradient, Texture2D texture) {
        if (gradient == null) return;

        for (var x = 0; x < texture.width; x++) {
            var color = gradient.Evaluate((float)x / (texture.width - 1));
            for (var y = 0; y < texture.height; y++) texture.SetPixel(x, y, color);
        }

        texture.Apply();
    }

    public static string TextureNamePrefix(string propertyName) {
        return $"z_{propertyName}Tex";
    }

    public static Texture2D GetOrCreateSubAssetTexture(string path, string name, FilterMode filterMode, int resolution,
                                                       bool hdr) {
        var textureAsset = LoadSubAsset(path, name);

        if (textureAsset != null && (hdr && textureAsset.format != TextureFormat.RGBAHalf ||
                                     !hdr && textureAsset.format == TextureFormat.RGBAHalf)) {
            AssetDatabase.RemoveObjectFromAsset(textureAsset);
        }

        if (textureAsset == null) {
            textureAsset = CreateEmptySubAssetTexture(path, name, filterMode, resolution, hdr);
        }

        // Force set filter mode for legacy materials.
        textureAsset.filterMode = filterMode;

        if (textureAsset.width != resolution) {
#if UNITY_2021_2_OR_NEWER
            textureAsset.Reinitialize(resolution, 1);
#else
            textureAsset.Resize(resolution, 1);
#endif
        }

        return textureAsset;
    }

    private static Texture2D CreateEmptySubAssetTexture(string path, string name, FilterMode filterMode, int resolution,
                                                        bool hdr) {
        var textureAsset = new Texture2D(resolution, 1, hdr ? TextureFormat.RGBAHalf : TextureFormat.ARGB32, false) {
            name = name, wrapMode = TextureWrapMode.Clamp, filterMode = filterMode
        };
        AssetDatabase.AddObjectToAsset(textureAsset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(path);
        return textureAsset;
    }

    public static string Serialize(Gradient gradient) {
        return gradient == null ? null : JsonUtility.ToJson(new GradientDrawer.GradientData(gradient));
    }

    public static Gradient Deserialize(MaterialProperty prop, string name) {
        if (prop == null) return null;

        var json = name.Substring(TextureNamePrefix(prop.name).Length);
        try {
            var gradientRepresentation = JsonUtility.FromJson<GradientDrawer.GradientData>(json);
            return gradientRepresentation?.ToGradient();
        }
        catch (Exception e) {
            Log.M($"Bypass decoding a gradient. Debug info: {json} - {e}");
            return null;
        }
    }
}
}