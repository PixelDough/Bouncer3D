// Constants
#define HUE 0.0
#define SATURATION 40.0
#define BRIGHTNESS 1.0

#define F_COL (1.0 / 4.0)
#define F_LUMA_LP (1.0 / 6.0)

#define FIR_SIZE 29

const float pi = 3.14159265358979323846;
const float tau = 6.28318530717958647692;

// YIQ to RGB conversion matrix
float3x3 yiq2rgb = float3x3(
    1.000, 0.956, 0.621,
    1.000, -0.272, -0.647,
    1.000, -1.106, 1.703
);

float3x3 rgb2yiq = float3x3(
    1.000, 1.000, 1.000,
    0.956, -0.272, -1.106,
    0.621, -0.647, 1.703
);

// Functions
float2x2 rotate(float a)
{
    float cosa = cos(a);
    float sina = sin(a);
    return float2x2(
        cosa, -sina,
        sina,  cosa
    );
}

float sinc(float x)
{
    return (abs(x) < 1e-5) ? 1.0 : sin(x * pi) / (x * pi);
}

float WindowBlackman(float a, int N, int i)
{
    float a0 = (1.0 - a) / 2.0;
    float a1 = 0.5;
    float a2 = a / 2.0;

    float ratio = float(i) / float(N - 1);
    float wnd = a0 - a1 * cos(2.0 * pi * ratio) + a2 * cos(4.0 * pi * ratio);

    return wnd;
}

float Lowpass(float Fc, float Fs, int N, int i)
{
    float wc = Fc / Fs;
    float wnd = WindowBlackman(0.16, N, i);
    float x = 2.0 * wc * (float(i) - float(N) / 2.0);
    return 2.0 * wc * wnd * sinc(x);
}

void CompositeArtifacting_float(
    float2 uv,
    float2 textureSize,
    UnityTexture2D _texture,
    UnitySamplerState _sampler,
    out float4 OutColor)
{
    // Texture sampling parameters
    float Fs = textureSize.x;
    float Flumlp = Fs * F_LUMA_LP;

    // Texel size in UV coordinates
    float2 texelSize = float2(1.0 / textureSize.x, 1.0 / textureSize.y);

    // Sample the luma component at the current UV
    float luma = _texture.Sample(_sampler, uv).r;

    // Initialize chroma vector
    float2 chroma = float2(0.0, 0.0);

    // Filtering out unwanted high-frequency content from the chroma (IQ) signal
    for (int i = 0; i < FIR_SIZE; i++)
    {
        int tpidx = FIR_SIZE - i - 1;
        float lp = Lowpass(Flumlp, Fs, FIR_SIZE, tpidx);

        // Compute the sample UV coordinates with pixel offset
        float sampleOffset = (float(i) - float(FIR_SIZE) / 2.0) * texelSize.x;
        float2 sampleUV = uv + float2(sampleOffset, 0.0);

        // Wrap UV coordinates to stay within [0,1]
        sampleUV = frac(sampleUV);

        // Sample the texture at sampleUV
        float4 sampleTexel = _texture.Sample(_sampler, sampleUV);

        // Accumulate the chroma components (g and b channels)
        chroma += sampleTexel.gb * lp;
    }

    // Apply rotation to chroma vector
    chroma = mul(chroma, rotate(tau * HUE));

    // Compute the YIQ vector
    float3 yiq;
    yiq.x = BRIGHTNESS * luma;
    yiq.yz = chroma * SATURATION;

    // Convert YIQ to RGB
    float3 color = mul(yiq2rgb, yiq);
    
    OutColor = float4(chroma, 0, 1.0);
}