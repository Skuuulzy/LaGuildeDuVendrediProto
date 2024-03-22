Shader "Hidden/Universal Render Pipeline/Terrain/Lit (Base Pass)"
{
    Properties
    {
        _BaseColor("Color", Color) = (1,1,1,1)
        _MainTex("Albedo(RGB), Smoothness(A)", 2D) = "white" {}
        _MetallicTex ("Metallic (R)", 2D) = "black" {}
        _HeatTex("HeatTex (RGB)", 2D) = "white" {}
        [Toggle(URP_ENABLED)] URP_ENABLED("URP Enabled", Float) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Geometry-100" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 200

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            Name "ForwardLit"
            // Lightmode matches the ShaderPassName set in LightweightPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Lightweight Pipeline
            Tags{"LightMode" = "LightweightForward"}

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #define _METALLICSPECGLOSSMAP 1
            #define _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A 1

            // -------------------------------------
            // Lightweight Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

            #pragma vertex SplatmapVert
            #pragma fragment SplatmapFragment

            #pragma shader_feature_local _NORMALMAP
            // Sample normal in pixel shader when doing instancing
            #pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
            #define TERRAIN_SPLAT_BASEPASS 1

            #pragma shader_feature URP_ENABLED
            #ifdef URP_ENABLED
              #include "./TerrainLitInput.hlsl"
              #include "./TerrainLitPasses.hlsl"
            #else
              float4 SplatmapVert() : SV_POSITION { return float4(0,0,0,1); }
              float4 SplatmapFragment() : SV_TARGET { return float4(0,0,0,1); }
            #endif
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #ifdef URP_ENABLED
              #include "Packages/com.unity.render-pipelines.lightweight/Shaders/Terrain/TerrainLitInput.hlsl"
              #include "Packages/com.unity.render-pipelines.lightweight/Shaders/Terrain/TerrainLitPasses.hlsl"
            #else
              float4 ShadowPassVertex() : SV_POSITION { return float4(0,0,0,1); }
              float4 ShadowPassFragment() : SV_TARGET { return float4(0,0,0,1); }
            #endif
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

            #ifdef URP_ENABLED
              #include "Packages/com.unity.render-pipelines.lightweight/Shaders/Terrain/TerrainLitInput.hlsl"
              #include "Packages/com.unity.render-pipelines.lightweight/Shaders/Terrain/TerrainLitPasses.hlsl"
            #else
              float4 DepthOnlyVertex() : SV_POSITION { return float4(0,0,0,1); }
              float4 DepthOnlyFragment() : SV_TARGET { return float4(0,0,0,1); }
            #endif
            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex TerrainVertexMeta
            #pragma fragment TerrainFragmentMeta

            #define _METALLICSPECGLOSSMAP 1
            #define _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A 1

            #ifdef URP_ENABLED            
              #include "Packages/com.unity.render-pipelines.lightweight/Shaders/Terrain/TerrainLitInput.hlsl"
              #include "Packages/com.unity.render-pipelines.lightweight/Shaders/Terrain/TerrainLitMetaPass.hlsl"
            #else
              float4 TerrainVertexMeta() : SV_POSITION { return float4(0,0,0,1); }
              float4 TerrainFragmentMeta() : SV_TARGET { return float4(0,0,0,1); }
            #endif

            ENDHLSL
        }

        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
        UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
    }
    FallBack "Hidden/InternalErrorShader"
    //CustomEditor "LitShaderGUI"
}
