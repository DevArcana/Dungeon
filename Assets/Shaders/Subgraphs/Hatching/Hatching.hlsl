#ifndef HATCHING_HLSL
#define HATCHING_HLSL

void Hatching_half(half3 LighterHatch, half3 DarkerHatch, half Intensity, half Ambient, half OverbrightTolerance, out half3 Color) {
    Intensity = max(Ambient, Intensity);
    Intensity = min(Intensity, 1.0);
    half3 overbright = max(0, Intensity - (1.0 - OverbrightTolerance));
    Intensity = (Intensity) * 6;
    half3 weightsA = saturate((Intensity) + half3(-0, -1, -2));
    half3 weightsB = saturate((Intensity) + half3(-3, -4, -5));

    weightsA.xy -= weightsA.yz;
    weightsA.z -= weightsB.x;
    weightsB.xy -= weightsB.yz;

    DarkerHatch = DarkerHatch * weightsA;
    LighterHatch = LighterHatch * weightsB;

    Color = overbright + LighterHatch.r + LighterHatch.g + LighterHatch.b + DarkerHatch.r + DarkerHatch.g + DarkerHatch.b;
}

#endif