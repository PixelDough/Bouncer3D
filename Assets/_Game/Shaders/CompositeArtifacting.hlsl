// NTSC Shader for Unity Shader Graph
// Converted and adapted for Shader Graph use

// Include necessary Unity shader libraries if needed
// (In Shader Graph, including libraries is not necessary)

// Function to convert sRGB to FCC YIQ color space
float3 FCCYIQFromSRGB(float3 srgb)
{
    float3 yiq = float3(
        srgb.r * 0.30 + srgb.g * 0.59 + srgb.b * 0.11,
        srgb.r * 0.599 + srgb.g * -0.2773 + srgb.b * -0.3217,
        srgb.r * 0.213 + srgb.g * -0.5251 + srgb.b * 0.3121
    );

    return yiq;
}

// Function to convert FCC YIQ to sRGB color space
float3 SRGBFromFCCYIQ(float3 yiq)
{
    float3 srgb = float3(
        yiq.x + yiq.y * 0.9469 + yiq.z * 0.6236,
        yiq.x + yiq.y * -0.2748 + yiq.z * -0.6357,
        yiq.x + yiq.y * -1.1 + yiq.z * 1.7
    );

    return srgb;
}

// Quadrature Amplitude Modulation function
float3 QuadratureAmplitudeModulation(
    float3 colorYIQ,
    float2 screenPosition,
    float _NtscHorizontalCarrierFrequency,
    float _NtscLinePhaseShift
)
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
    float3 premultiplied = float3(Y, 2.0 * s * modulated, 2.0 * c * modulated);
    
    return premultiplied;
}

// Gaussian function for weighting
float Gaussian(int positionX, float kernelRadiusInverse)
{
    float x = (float)positionX;
    return exp(-0.5 * (x * x * kernelRadiusInverse * kernelRadiusInverse));
}

// Compute Gaussian in YIQ color space
float3 ComputeGaussianInYIQ(
    float2 screenPosition,
    UnityTexture2D _MainTex,
    UnitySamplerState _SamplerState,
    float _NtscHorizontalCarrierFrequency,
    float _NtscLinePhaseShift,
    int _NtscKernelRadius,
    float _NtscKernelWidthRatio,
    float2 resolution
)
{
    float3 colorTotal = float3(0.0, 0.0, 0.0);
    float weightTotal = 0.0;
    float kernelRadiusInverse = 1.0 / (_NtscKernelRadius * resolution.x);
    
    for (int x = -_NtscKernelRadius; x <= _NtscKernelRadius; ++x)
    {
        // Calculate current position in pixels
        float2 positionCurrentPixels = screenPosition + float2(
            x * 4.0 * (_NtscKernelWidthRatio * _NtscHorizontalCarrierFrequency),
            0.0);

        // Convert to UV coordinates
        float2 uv = positionCurrentPixels / resolution;
        uv = clamp(uv, 0.0, 1.0);

        float3 colorCurrent = FCCYIQFromSRGB(_MainTex.Sample(_SamplerState, uv).rgb);

        // Apply Quadrature Amplitude Modulation
        colorCurrent = QuadratureAmplitudeModulation(
            colorCurrent,
            positionCurrentPixels,
            _NtscHorizontalCarrierFrequency,
            _NtscLinePhaseShift
        );

        float weightCurrent = Gaussian(x, kernelRadiusInverse);

        colorTotal += colorCurrent * weightCurrent;
        weightTotal += weightCurrent;
    }

    colorTotal /= weightTotal;        
    
    return colorTotal;
}

// Compute the analog signal
float3 ComputeAnalogSignal(
    float2 screenPosition,
    UnityTexture2D _MainTex,
    UnitySamplerState _SamplerState,
    float _NtscFlickerUseTimeScale,
    float _NtscTimeUnscaled,
    float _NtscFlickerPercent,
    float _NtscFlickerScaleX,
    float _NtscFlickerScaleY,
    float _NtscSharpness,
    float _NtscHorizontalCarrierFrequency,
    float _NtscLinePhaseShift,
    int _NtscKernelRadius,
    float _NtscKernelWidthRatio,
    float time,
    float2 resolution
)
{
    float currentTime = (_NtscFlickerUseTimeScale == 1) ? time : _NtscTimeUnscaled;
    float2 offsetFlicker = _NtscFlickerScaleX * float2(
        sign(sin(currentTime * (60.0 * _NtscFlickerPercent)) * sign(sin(screenPosition.y * _NtscFlickerScaleY))), 0.0);
    screenPosition += offsetFlicker;

    // Clamp UV coordinates to [0,1]
    float2 uv = screenPosition / resolution;
    uv = clamp(uv, 0.0, 1.0);

    // Sample the texture
    float4 color = _MainTex.Sample(_SamplerState, uv);

    // Convert to YIQ color space
    float3 yiqColor = FCCYIQFromSRGB(color.rgb);

    float oldY = yiqColor.x;
    
    // Compute the Gaussian effect
    yiqColor = ComputeGaussianInYIQ(
        screenPosition,
        _MainTex,
        _SamplerState,
        _NtscHorizontalCarrierFrequency,
        _NtscLinePhaseShift,
        _NtscKernelRadius,
        _NtscKernelWidthRatio,
        resolution
    );

    // Apply sharpness adjustment
    yiqColor.x = oldY + (_NtscSharpness * (oldY - yiqColor.x));

    // Convert back to sRGB color space
    float3 finalColor = SRGBFromFCCYIQ(yiqColor);

    return finalColor;
}

// Main Shader Function for Shader Graph
void CompositeArtifacting_float(
    float2 uv,
    float2 resolution,
    UnityTexture2D _MainTex,
    UnitySamplerState _SamplerState,
    float horizontalCarrierFrequency,
    int kernelRadius,
    float kernelWidthRatio,
    float sharpness,
    float linePhaseShift,
    float flickerPercent,
    float flickerScaleX,
    float flickerScaleY,
    int flickerUseTimeScale,
    float timeUnscaled,
    float time,
    out float4 OutColor
)
{
    // Convert UV to screen position in pixels
    float2 screenPosition = uv * resolution;

    // Compute the analog signal
    float3 color = ComputeAnalogSignal(
        screenPosition,
        _MainTex,
        _SamplerState,
        flickerUseTimeScale,
        timeUnscaled,
        flickerPercent,
        flickerScaleX,
        flickerScaleY,
        sharpness,
        horizontalCarrierFrequency,
        linePhaseShift,
        kernelRadius,
        kernelWidthRatio,
        time,
        resolution
    );

    OutColor = float4(color, 1.0);
}
