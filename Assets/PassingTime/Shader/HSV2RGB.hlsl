#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

float3 HUE2RGB(float H)
{
    float R = abs(H * 6.0f - 3.0f) - 1.0f;
    float G = 2.0f - abs(H * 6.0f - 2.0f);
    float B = 2.0f - abs(H * 6.0f - 4.0f);

    return saturate(float3(R, G, B));
}

void HSV2RGB_float(float H, float S, float V, out float3 Out)
{
    float3 RGB = HUE2RGB(H);
    Out = ((RGB - (float3)1.0f) * S + (float3)1.0f) * V;
}

void HSV2RGB_half(float H, float S, float V, out float3 Out)
{
    float3 RGB = HUE2RGB(H);
    Out = ((RGB - (float3)1.0f) * S + (float3)1.0f) * V;
}

float3 bump3y (float3 x, float3 yoffset)
{
    float3 y = 1 - x * x;
    y = saturate(y-yoffset);
    return y;
}

void spectral_zucconi6_float(float w, out float3 Out)
{
    // w: [400, 700]
    // x: [0,   1]
    float x = saturate((w - 400.0)/ 300.0);
    const float3 c1 = float3(3.54585104, 2.93225262, 2.41593945);
    const float3 x1 = float3(0.69549072, 0.49228336, 0.27699880);
    const float3 y1 = float3(0.02312639, 0.15225084, 0.52607955);
    const float3 c2 = float3(3.90307140, 3.21182957, 3.96587128);
    const float3 x2 = float3(0.11748627, 0.86755042, 0.66077860);
    const float3 y2 = float3(0.84897130, 0.88445281, 0.73949448);
    Out =
    bump3y(c1 * (x - x1), y1) +
    bump3y(c2 * (x - x2), y2) ;
}

void spectral_zucconi6_half(float w, out float3 Out)
{
    // w: [400, 700]
    // x: [0,   1]
    float x = saturate((w - 400.0)/ 300.0);
    const float3 c1 = float3(3.54585104, 2.93225262, 2.41593945);
    const float3 x1 = float3(0.69549072, 0.49228336, 0.27699880);
    const float3 y1 = float3(0.02312639, 0.15225084, 0.52607955);
    const float3 c2 = float3(3.90307140, 3.21182957, 3.96587128);
    const float3 x2 = float3(0.11748627, 0.86755042, 0.66077860);
    const float3 y2 = float3(0.84897130, 0.88445281, 0.73949448);
    Out =
    bump3y(c1 * (x - x1), y1) +
    bump3y(c2 * (x - x2), y2) ;
}

void Thin_half(float3 L, float3 V, float3 N, float N1, float N2, float distance, out float3 Out)
{
    // Reminder:
    //     thetaL = angle from L to N
    //     thetaR = angle from reflected L inside material to N
    // From Snell's Law:
    //     N1 * sin(thetaL) = N2 * sin(thetaR)
    float cos_thetaL = dot(N, L);
    float thetaL = acos(cos_thetaL);
    float sin_thetaR = (N1 / N2) * sin(thetaL);
    float thetaR = asin(sin_thetaR);

    float u = N2 * 2 * distance * abs(cos(thetaR));
    float3 color = 0;
    for (int n = 1; n <= 8; n++)
    {
        // Constructive interference
        float wavelength = u / n;
        float3 c;
        spectral_zucconi6_half(wavelength, c);
        color += c;
    }
    Out = saturate(color);
}

void Thin_float(float3 L, float3 V, float3 N, float N1, float N2, float distance, out float3 Out)
{
    // Reminder:
    //     thetaL = angle from L to N
    //     thetaR = angle from reflected L inside material to N
    // From Snell's Law:
    //     N1 * sin(thetaL) = N2 * sin(thetaR)
    float cos_thetaL = dot(N, L);
    float thetaL = acos(cos_thetaL);
    float sin_thetaR = (N1 / N2) * sin(thetaL);
    float thetaR = asin(sin_thetaR);

    float u = N2 * 2 * distance * abs(cos(thetaR));
    float3 color = 0;
    for (int n = 1; n <= 8; n++)
    {
        // Constructive interference
        float wavelength = u / n;
        float3 c;
        spectral_zucconi6_half(wavelength, c);
        color += c;
    }
    Out = saturate(color);
}

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _SHADOWS_SOFT

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = half3(1, 0.95, 0.8);
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void MainLight_half(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = half3(1, 0.95, 0.8);
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}
#endif