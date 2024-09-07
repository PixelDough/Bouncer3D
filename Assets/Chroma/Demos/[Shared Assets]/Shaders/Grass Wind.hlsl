#ifndef GRASS_LIB_INCLUDED
#define GRASS_LIB_INCLUDED

void GrassWind_half(half2 UV, out half2 Out) {
    half2 nUV = UV + _Time.x * _WindSpeed;
    half2 n = _WindNoise.Sample(SamplerState_Linear_Repeat, nUV).xy;
    half s = (sin(_Time.y * 1.6 + 1) - cos(_Time.y + n.x * 2)) * 0.5 + n.y * 0.5;
    n *= s * _WindIntensity;
    Out = n;
}

#endif
