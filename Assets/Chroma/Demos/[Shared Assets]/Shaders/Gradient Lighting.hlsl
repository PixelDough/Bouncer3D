#ifndef DUSTYROOM_CUSTOM_LIGHTING_INCLUDED
#define DUSTYROOM_CUSTOM_LIGHTING_INCLUDED

#include "../../../Core/Chroma.hlsl"

#ifndef SHADERGRAPH_PREVIEW
#ifndef BUILTIN_TARGET_API
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#if (SHADERPASS != SHADERPASS_FORWARD)
    #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
#endif
#endif
#endif

struct CustomLightingData
{
    // Position and orientation
    float3 positionWS;
    float3 normalWS;
    float3 viewDirectionWS;
    float4 shadowCoord;

    // Surface attributes
    UnityTexture2D gradient;
    UnitySamplerState gradientSampler;

    float smoothness;
    float ambientOcclusion;

    // Baked lighting
    float3 bakedGI;
    float4 shadowMask;
    float fogFactor;
};

#ifndef SHADERGRAPH_PREVIEW
float3 CustomGlobalIllumination(CustomLightingData d, float3 albedo) {
    const float3 indirectDiffuse = albedo * d.bakedGI * d.ambientOcclusion;

    float3 reflectVector = reflect(-d.viewDirectionWS, d.normalWS);
    // This is a rim light term, making reflections stronger along the edges of view
    const float fresnel = Pow4(abs(1 - saturate(dot(d.viewDirectionWS, d.normalWS))));
    const float perceptualRoughness = RoughnessToPerceptualRoughness(1 - d.smoothness);
    // Samples the baked reflections cubemap (located in URP/ShaderLibrary/Lighting.hlsl).
    const float3 reflection = GlossyEnvironmentReflection(reflectVector, perceptualRoughness, d.ambientOcclusion);
    const float3 indirectSpecular = reflection * fresnel;

    return indirectDiffuse + indirectSpecular;
}

float3 CustomLightHandling(CustomLightingData d, Light light) {
    float3 radiance = light.color * (light.distanceAttenuation * light.shadowAttenuation);
    const float nDotL = saturate(dot(d.normalWS, light.direction) * 0.5 + 0.5);
    float3 g = SAMPLE_GRADIENT_HDR_X(d.gradient, d.gradientSampler, nDotL).rgb;
    const float3 color = g * radiance;
    return color;
}
#endif

float3 CalculateCustomLighting(CustomLightingData d) {
#ifdef SHADERGRAPH_PREVIEW
    float3 lightDir = float3(0.5, 0.5, 0);
    const float nDotL = saturate(dot(d.normalWS, lightDir) * 0.5 + 0.5);
    return SAMPLE_TEXTURE2D(d.gradient, d.gradientSampler, nDotL).rgb;
#else

    Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, d.shadowMask);

    // Including does not help.
    // "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Input.hlsl"
#if BUILTIN_TARGET_API
    mainLight.direction = half3(0.5, 0.5, 0);
    mainLight.distanceAttenuation = 1;
    mainLight.shadowAttenuation = 1.0;
    mainLight.color = 1;
#endif

    MixRealtimeAndBakedGI(mainLight, d.normalWS, d.bakedGI);

    float3 color = CustomLightHandling(d, mainLight);
    color += CustomGlobalIllumination(d, color);

#ifdef _ADDITIONAL_LIGHTS
    uint numAdditionalLights = GetAdditionalLightsCount();
    for (uint i = 0; i < numAdditionalLights; i++) {
        Light light = GetAdditionalLight(i, d.positionWS, d.shadowMask);
        color += CustomLightHandling(d, light);
    }
#endif

    color = MixFog(color, d.fogFactor);

    return color;
#endif
}

void CalculateCustomLighting_float(float3 Position, float3 Normal, float3 ViewDirection, UnityTexture2D Gradient,
                                   UnitySamplerState GradientSampler, float Smoothness, float AmbientOcclusion,
                                   float2 LightmapUV, out float3 Color) {
    CustomLightingData d;
    d.positionWS = Position;
    d.normalWS = Normal;
    d.viewDirectionWS = ViewDirection;
    d.gradient = Gradient;
    d.gradientSampler = GradientSampler;
    d.smoothness = Smoothness;
    d.ambientOcclusion = AmbientOcclusion;

#ifdef SHADERGRAPH_PREVIEW
    // In preview, there's no shadows or bakedGI
    d.shadowCoord = 0;
    d.bakedGI = 0;
    d.shadowMask = 0;
    d.fogFactor = 0;
#else
    // Calculate the main light shadow coord
    // There are two types depending on if cascades are enabled
    float4 positionCS = TransformWorldToHClip(Position);
#if SHADOWS_SCREEN
    d.shadowCoord = ComputeScreenPos(positionCS);
#else
    d.shadowCoord = TransformWorldToShadowCoord(Position);
#endif

    // The following URP functions and macros are all located in URP/ShaderLibrary/Lighting.hlsl
    // Technically, OUTPUT_LIGHTMAP_UV, OUTPUT_SH and ComputeFogFactor should be called in the vertex function of the
    // shader. However, as of 2021.1, we do not have access to custom interpolators in the shader graph.

    // The lightmap UV is usually in TEXCOORD1
    // If lightmaps are disabled, OUTPUT_LIGHTMAP_UV does nothing
    float2 lightmapUV;
    OUTPUT_LIGHTMAP_UV(LightmapUV, unity_LightmapST, lightmapUV);
    // Samples spherical harmonics, which encode light probe data
    float3 vertexSH;
#if UNITY_VERSION >= 60000008
    float4 probeOcclusion;
    OUTPUT_SH4(d.positionWS, d.normalWS, d.viewDirectionWS, vertexSH, probeOcclusion);
#elif UNITY_VERSION >= 202317
    OUTPUT_SH4(d.positionWS, d.normalWS, d.viewDirectionWS, vertexSH);
#elif UNITY_VERSION >= 202310
    OUTPUT_SH(d.positionWS, d.normalWS, d.viewDirectionWS, vertexSH);
#else
    OUTPUT_SH(d.normalWS, vertexSH);
#endif
    // This function calculates the final baked lighting from light maps or probes
    d.bakedGI = SAMPLE_GI(lightmapUV, vertexSH, d.normalWS);
    // This function calculates the shadow mask if baked shadows are enabled
    d.shadowMask = SAMPLE_SHADOWMASK(lightmapUV);
    // This returns 0 if fog is turned off
    // It is not the same as the fog node in the shader graph
    d.fogFactor = ComputeFogFactor(positionCS.z);
#endif

    Color = CalculateCustomLighting(d);
}

#endif  // DUSTYROOM_CUSTOM_LIGHTING_INCLUDED
