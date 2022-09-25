// WaterCausticsModules
// Copyright (c) 2021 Masataka Hakozaki

Shader "WaterCausticsModules/Effect" {

    SubShader {
        LOD 0
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Transparent-50" }

        Pass {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend One One
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Offset [_OffsetFactor], [_OffsetUnits]
            Cull [_CullMode]
            Stencil {
                Ref [_StencilRef]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilPass]
                Fail [_StencilFail]
                ZFail [_StencilZFail]
            }

            HLSLPROGRAM

            #pragma target 2.0
            
            #define REQUIRE_OPAQUE_TEXTURE 1
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            #if VERSION_GREATER_EQUAL(11, 0)
                #pragma multi_compile_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #else
                #pragma multi_compile_fragment _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile_fragment _ _MAIN_LIGHT_SHADOWS_CASCADE
            #endif
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile_fragment _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fog

            #include "WaterCausticsEffectCommon.hlsl"
            #pragma multi_compile_local_fragment _ _RECEIVE_SHADOWS_OFF


            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 clipPos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    half fog : TEXCOORD3;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float _WCE_Scale;
                float _WCE_WaterSurfaceY;
                float _WCE_WaterSurfaceAttenCoef;
                float _WCE_WaterSurfaceAttenOffset;
                float _WCE_DepthAttenCoef;
                half _WCE_IntensityMainLit;
                half _WCE_IntensityAddLit;
                float2 _WCE_ColorShift;
                half _WCE_LitSaturation;
                half _WCE_MulOpaqueIntensity;
                half _WCE_NormalAttenIntensity;
                half _WCE_NormalAttenPower;
                half _WCE_TransparentBackside;
                int _WCE_ClipOutsideVolume;
                int _WCE_UseImageMask;
            CBUFFER_END

            CBUFFER_START(FrequentlyUpdateVariables)
                float4x4 _WCE_WldObjMatrixOfVolume;
            CBUFFER_END

            TEXTURE2D(_WCE_CausticsTex);
            TEXTURE2D(_WCE_ImageMaskTex);
            SAMPLER(sampler_WCE_CausticsTex);
            SAMPLER(sampler_WCE_ImageMaskTex);


            v2f vert(appdata v) {
                v2f o = (v2f)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float3 posO = v.vertex.xyz;
                float3 posW = TransformObjectToWorld(posO);
                float4 posC = TransformWorldToHClip(posW);
                o.clipPos = posC;
                o.worldPos = posW;
                float3 normWS = TransformObjectToWorldNormal(v.normal);
                float3 viewDirWS = _WorldSpaceCameraPos.xyz - posW;
                o.normalWS = dot(viewDirWS, normWS) >= 0 ? normWS : - normWS;
                float4 posSC = ComputeScreenPos(posC);
                o.screenPos = posSC;
                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    o.fog = ComputeFogIntensity(ComputeFogFactor(posC.z));
                #endif
                return o;
            }


            half4 frag(v2f IN) : SV_Target {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                float3 WorldPos = IN.worldPos;

                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    [branch] if (IN.fog < 0.001)return half4(0, 0, 0, 0);
                    _WCE_IntensityMainLit *= IN.fog;
                    _WCE_IntensityAddLit *= IN.fog;
                #endif
                [branch] if (_WCE_ClipOutsideVolume != 0 || _WCE_UseImageMask != 0) {
                    float3 posO = mul(_WCE_WldObjMatrixOfVolume, float4(WorldPos, 1)).xyz;
                    [branch] if (_WCE_ClipOutsideVolume != 0 && (abs(posO.x) > 0.5 || abs(posO.y) > 0.5 || abs(posO.z) > 0.5)) {
                        return half4(0, 0, 0, 0);
                    }
                    [branch] if (_WCE_UseImageMask != 0) {
                        half imageMask = _WCE_ImageMaskTex.Sample(sampler_WCE_ImageMaskTex, posO.xz + 0.5).r;
                        [branch] if (imageMask < 0.0001) {
                            return half4(0, 0, 0, 0);
                        }
                        _WCE_IntensityMainLit *= imageMask;
                        _WCE_IntensityAddLit *= imageMask;
                    }
                }

                half3 c = WCE_waterCausticsEmission(WorldPos, IN.normalWS, _WCE_CausticsTex, sampler_WCE_CausticsTex, _WCE_Scale,
                _WCE_WaterSurfaceY, _WCE_WaterSurfaceY + _WCE_WaterSurfaceAttenOffset, _WCE_WaterSurfaceAttenCoef, _WCE_DepthAttenCoef, _WCE_IntensityMainLit, _WCE_IntensityAddLit,
                _WCE_ColorShift, _WCE_LitSaturation, _WCE_NormalAttenIntensity, _WCE_NormalAttenPower, _WCE_TransparentBackside);

                [branch] if (_WCE_MulOpaqueIntensity > 0) {
                    float2 screenPos = IN.screenPos.xy / IN.screenPos.w;
                    c *= 1 - (1 - SHADERGRAPH_SAMPLE_SCENE_COLOR(screenPos)) * _WCE_MulOpaqueIntensity;
                }
                
                return half4(c, 1);
            }

            ENDHLSL

        }
    }

    Fallback "Hidden/InternalErrorShader"
}
