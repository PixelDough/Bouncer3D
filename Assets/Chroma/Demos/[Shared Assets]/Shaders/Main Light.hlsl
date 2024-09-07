#ifndef DUSTYROOM_MAIN_LIGHT_INCLUDED
#define DUSTYROOM_MAIN_LIGHT_INCLUDED

void MainLight_half(float3 WorldPosition, out half3 Direction, out half3 Color, out half DistanceAttenuation,
                    out half ShadowAttenuation) {
#ifdef SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = 1;
    DistanceAttenuation = 1;
    ShadowAttenuation = 1;
#else

#if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
    half4 clipPos = TransformWorldToHClip(WorldPosition);
    half4 shadowCoord =  ComputeScreenPos(clipPos);
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPosition);
#endif

    Light light = GetMainLight(shadowCoord);

    // Including does not help.
    // "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Input.hlsl"
#if BUILTIN_TARGET_API
    mainLight.direction = half3(0.5, 0.5, 0);
    mainLight.distanceAttenuation = 1;
    mainLight.shadowAttenuation = 1.0;
    mainLight.color = 1;
#endif

    Direction = light.direction;
    Color = light.color;
    DistanceAttenuation = light.distanceAttenuation;
    ShadowAttenuation = light.shadowAttenuation;

#endif
}

void NDotL_half(half3 Normal, half3 LightDirection, out half Shading) {
    const half nDotL = saturate(dot(Normal, LightDirection) * 0.5 + 0.5);
    Shading = nDotL;
}

#endif  // DUSTYROOM_MAIN_LIGHT_INCLUDED
