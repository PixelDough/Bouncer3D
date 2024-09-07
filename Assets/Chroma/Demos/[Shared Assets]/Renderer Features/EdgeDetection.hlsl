#ifndef EDGE_DETECTION_INCLUDED
#define EDGE_DETECTION_INCLUDED

float4 _CameraColorTexture_TexelSize;

/*
TEXTURE2D (_CameraDepthTexture);
SAMPLER (sampler_CameraDepthTexture);
TEXTURE2D (_CameraNormalsTexture);
SAMPLER (sampler_CameraNormalsTexture);
*/

void EdgeDetection_float(float4 In, float2 UV, float OutlineThickness, float DepthSensitivity, float NormalsSensitivity,
                         float4 EdgeColor, out float4 Out) {
    const float halfScaleFloor = floor(OutlineThickness * 0.5);
    const float halfScaleCeil = ceil(OutlineThickness * 0.5);
    float2 texel = (1.0) / float2(_CameraColorTexture_TexelSize.z, _CameraColorTexture_TexelSize.w);

    float2 uvSamples[4];

    float depthSamples[4];
    float3 normalSamples[4];

    uvSamples[0] = UV - float2(texel.x, texel.y) * halfScaleFloor;
    uvSamples[1] = UV + float2(texel.x, texel.y) * halfScaleCeil;
    uvSamples[2] = UV + float2(texel.x * halfScaleCeil, -texel.y * halfScaleFloor);
    uvSamples[3] = UV + float2(-texel.x * halfScaleFloor, texel.y * halfScaleCeil);

    for (int i = 0; i < 4; i++)
    {
        depthSamples[i] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uvSamples[i]).r;
        normalSamples[i] = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uvSamples[i]).xyz;
    }

    // Depth
    const float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
    const float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
    float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
    const float depthThreshold = (1 / DepthSensitivity) * depthSamples[0];
    edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

    // Normals
    const float3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
    const float3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
    float edgeNormal = sqrt(
        dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
    edgeNormal = edgeNormal > (1 / NormalsSensitivity) ? 1 : 0;

    const float edge = max(edgeDepth, edgeNormal);

    Out = ((1 - edge) * In) + (edge * lerp(In, EdgeColor, EdgeColor.a));
}
#endif  // EDGE_DETECTION_INCLUDED
