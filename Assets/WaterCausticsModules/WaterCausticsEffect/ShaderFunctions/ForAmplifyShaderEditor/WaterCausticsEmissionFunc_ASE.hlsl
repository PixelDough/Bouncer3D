// WaterCausticsModules
// Copyright (c) 2021 Masataka Hakozaki

#ifndef WCECF_FOR_ASE_INCLUDED
#define WCECF_FOR_ASE_INCLUDED

#define WCE_USE_SAMPLER2D_INSTEAD_TEXTURE2D
#include "../../Effect/Shaders/WaterCausticsEffectCommon.hlsl"

// Custom Function (for Amplify Shader Editor)
half3 WCECF_Emission(float3 WorldPos, half3 NormalWS, float2 ScreenUV, sampler2D CausticsTex, float2 TexRotSinCos, int3 TexChannels, bool UseTiling, int TilingSeed, float TilingRot, float TilingHard, float Density, float SurfaceY, float SurfFadeStart, float SurfFadeCoef, float DepthFadeStart, float DepthFadeCoef, half MainLitIntensity, half AddLitIntensity, half ShadowIntensity, float2 ColorShift, half LitSaturation, half NormalAtten, half NormalAttenRate, half TransparentBack, half BacksideShadow) {

    SurfFadeStart = max(SurfFadeStart, 0);
    TilingSeed = UseTiling ? max(TilingSeed, 0) : - 1;
    TilingHard = clamp(TilingHard, 0.75, 0.999);

    half3 e = WCE_EffectCore(WorldPos, NormalWS, ScreenUV, CausticsTex, TexRotSinCos, TexChannels, TilingSeed, TilingRot, TilingHard, Density, SurfaceY, SurfFadeStart, SurfFadeCoef, DepthFadeStart, DepthFadeCoef, MainLitIntensity, AddLitIntensity, ShadowIntensity, ColorShift, LitSaturation, NormalAtten, NormalAttenRate, TransparentBack, BacksideShadow);

    return e;
}

#endif

