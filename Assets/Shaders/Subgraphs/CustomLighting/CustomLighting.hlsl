#ifndef CUSTOMLIGHTING_HLSL
#define CUSTOMLIGHTING_HLSL

#ifndef SHADERGRAPH_PREVIEW
    #if VERSION_GREATER_EQUAL(9, 0)
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #if (SHADERPASS != SHADERPASS_FORWARD)
            #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
        #endif
    #else
        #ifndef SHADERPASS_FORWARD
            #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
        #endif
    #endif
#endif

void MainLight_half (half3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAttenuation, out half ShadowAttenuation){
 
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(half3(1,1,-0.4));
    Color = half4(1,1,1,1);
    DistanceAttenuation = 1;
    ShadowAttenuation = 1;
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    Light mainLight = GetMainLight(shadowCoord);
 
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAttenuation = mainLight.distanceAttenuation;
    ShadowAttenuation = mainLight.shadowAttenuation;
#endif
 
}

#endif