Shader "PostProcessing/Ntsc"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    TEXTURE2D(_MainTex);

    float _NtscHorizontalCarrierFrequency;
    int _NtscKernelRadius;
    float _NtscKernelWidthRatio;
    float _NtscSharpness;
    float _NtscLinePhaseShift;
    float _NtscFlickerPercent;
    float _NtscFlickerScaleX;
    float _NtscFlickerScaleY;
    int _NtscFlickerUseTimeScale;
    float _NtscTimeUnscaled;

    //float4 _ScreenSize;
    float4 _ScreenSizeRasterizationRTScaled;

    uniform float _TimeUnscaled;

    
    SamplerState pointClampSampler;
    SamplerState linearClampSampler;

    SamplerState sampler_linear_MainTex;

    /*float4 FetchFrameBuffer(float2 uv)
    {
        float4 color = SAMPLE_TEXTURE2D_LOD(_FrameBufferTexture, s_point_clamp_sampler, uv, 0);
        return color;
    }*/

    float2 ClampRasterizationRTUV(float2 uv)
    {
        return uv;
        //return clamp(uv, _RasterizationRTScaledClampBoundsUV.xy, _RasterizationRTScaledClampBoundsUV.zw);
    }

    float3 FCCYIQFromSRGB(float3 srgb)
    {
        float3 yiq = float3(
            srgb.r * 0.30 + srgb.g * 0.59 + srgb.b * 0.11,
            srgb.r * 0.599 + srgb.g * -0.2773 + srgb.b * -0.3217,
            srgb.r * 0.213 + srgb.g * -0.5251 + srgb.b * 0.3121
        );

        return yiq;
    }

    float3 SRGBFromFCCYIQ(float3 yiq)
    {
        float3 srgb = float3(
            yiq.x + yiq.y * 0.9469 + yiq.z * 0.6236,
            yiq.x + yiq.y * -0.2748 + yiq.z * -0.6357,
            yiq.x + yiq.y * -1.1 + yiq.z * 1.7
        );

        return srgb;
    }
    
    float3 QuadratureAmplitudeModulation(float3 colorYIQ, float2 screenPosition)
    {
        float Y = colorYIQ.x;
        float I = colorYIQ.y;
        float Q = colorYIQ.z;

        float lineNumber = floor(screenPosition.y);
        float carrier_phase =
            _NtscHorizontalCarrierFrequency * screenPosition.x +
            _NtscLinePhaseShift * lineNumber;
        float s = sin(carrier_phase);
        float c = cos(carrier_phase);

        float modulated = I * s + Q * c;
        float3 premultiplied = float3(Y, 2 * s * modulated, 2 * c * modulated);
        
        return premultiplied;
    }

    float Gaussian(int positionX, int kernelRadiusInverse)
    {
        return exp2(-.5 * ((float)positionX * (float)positionX * (kernelRadiusInverse * kernelRadiusInverse)));
    }

    float3 ComputeGaussianInYIQ(float2 screenPosition)
    {
        float3 colorTotal = 0;
        float weightTotal = 0.0;
        
        for (int x = -_NtscKernelRadius; x <= _NtscKernelRadius; ++x)
        {
            // Convert from framebuffer normalized position to pixel position
            float2 positionCurrentPixels = screenPosition + float2(x * 16 * (_NtscKernelWidthRatio * _NtscHorizontalCarrierFrequency), 0);
            
            positionCurrentPixels = ClampRasterizationRTUV(positionCurrentPixels);
            float3 colorCurrent = FCCYIQFromSRGB(_MainTex.Sample(linearClampSampler, positionCurrentPixels * _ScreenSize.zw).rgb);

            // Use the original offset value in order to keep it the same distance across resolutions
            colorCurrent = QuadratureAmplitudeModulation(colorCurrent, positionCurrentPixels);

            float weightCurrent = Gaussian(x, 1.0 / (_NtscKernelRadius * 1920));

            colorTotal += colorCurrent * weightCurrent;
            weightTotal += weightCurrent;
        }

        colorTotal /= weightTotal;        
        
        return colorTotal;
    }

    float4 ComputeAnalogSignal(float2 screenPosition)
    {
        float4 time = (_NtscFlickerUseTimeScale == 1) ? _Time : _NtscTimeUnscaled;
        float2 offsetFlicker = (_NtscFlickerScaleX * float2(sign(sin(time.y * (60 * _NtscFlickerPercent)) * sign(sin(screenPosition.y * _NtscFlickerScaleY))), 0));
        screenPosition += offsetFlicker;
        
        screenPosition = ClampRasterizationRTUV(screenPosition);
        float4 color = _MainTex.Sample(linearClampSampler, screenPosition * _ScreenSize.zw);

        color.rgb = FCCYIQFromSRGB(color.rgb);

        float oldY = color.r;
        
        // Compute the Gaussian effect
        color.rgb = ComputeGaussianInYIQ(screenPosition);

        color.r = oldY + (_NtscSharpness * (oldY - color.r));

        color.rgb = SRGBFromFCCYIQ(color.rgb);

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

    float4 NtscFragmentProgram (PostProcessVaryings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
        float4 color = LOAD_TEXTURE2D_X(_MainTex, uv * _ScreenSize.xy);
        
        // Blend between the original and the grayscale color
        color.rgb = ComputeAnalogSignal(uv * _ScreenSize.xy);
        
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
            #pragma fragment NtscFragmentProgram
            ENDHLSL
        }
    }
    Fallback Off
}