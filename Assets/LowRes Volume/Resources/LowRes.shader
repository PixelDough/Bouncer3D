Shader "Hidden/PixelDough/PostProcessing/LowRes"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    TEXTURE2D(_MainTex);

    int _ReferenceResolution;
    float2 _ScreenSizeGivenReferenceResolution;

    SamplerState pointClampSampler;
    SamplerState linearClampSampler;

    SamplerState sampler_linear_MainTex;

    float2 ClampRasterizationRTUV(float2 uv)
    {
        return uv;
    }

    float4 ComputeLowRes(float2 screenPosition)
    {
        screenPosition = ClampRasterizationRTUV(screenPosition);
        float4 color = _MainTex.Sample(linearClampSampler, screenPosition * _ScreenSize.zw);

        return color;
    }

    struct PostProcessVaryings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    struct FullScreenTrianglePostProcessAttributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    PostProcessVaryings FullScreenTrianglePostProcessVertexProgram(FullScreenTrianglePostProcessAttributes input)
    {
        PostProcessVaryings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    float4 LowResFragmentProgram (PostProcessVaryings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
        float4 color = LOAD_TEXTURE2D_X(_MainTex, uv * _ScreenSize.xy);
        
        // Blend between the original and the grayscale color
        color.rgb = ComputeLowRes((round(uv * _ScreenSizeGivenReferenceResolution.xy) / _ScreenSizeGivenReferenceResolution.xy) * _ScreenSize.xy);
        
        return color;
    }
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex FullScreenTrianglePostProcessVertexProgram
            #pragma fragment LowResFragmentProgram
            ENDHLSL
        }
    }
    Fallback Off
}
