// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/BiRP/Stylized Fog (Physical Height)"
{
	Properties
	{
		_FogVariationTexture("Fog Variation Texture", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "HeightFog"  "Queue" = "Transparent+1" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		ZWrite Off
		ZTest Always
		Stencil
		{
			Ref 222
			Comp NotEqual
			Pass Replace
		}
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
		};

		uniform float4 CZY_LightColor;
		uniform float4 CZY_FogColor1;
		uniform float4 CZY_FogColor2;
		uniform float CZY_FogDepthMultiplier;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform sampler2D _FogVariationTexture;
		uniform float3 CZY_VariationWindDirection;
		uniform float CZY_VariationScale;
		uniform float CZY_VariationAmount;
		uniform float CZY_VariationDistance;
		uniform float CZY_FogColorStart1;
		uniform float4 CZY_FogColor3;
		uniform float CZY_FogColorStart2;
		uniform float4 CZY_FogColor4;
		uniform float CZY_FogColorStart3;
		uniform float4 CZY_FogColor5;
		uniform float CZY_FogColorStart4;
		uniform float CZY_LightFlareSquish;
		uniform float3 CZY_SunDirection;
		uniform half CZY_LightIntensity;
		uniform half CZY_LightFalloff;
		uniform float CZY_FilterSaturation;
		uniform float CZY_FilterValue;
		uniform float4 CZY_FilterColor;
		uniform float4 CZY_SunFilterColor;
		uniform float3 CZY_MoonDirection;
		uniform float4 CZY_FogMoonFlareColor;
		uniform float4 CZY_HeightFogColor;
		uniform float CZY_HeightFogBase;
		uniform float CZY_HeightFogTransition;
		uniform float CZY_HeightFogBaseVariationScale;
		uniform float CZY_HeightFogBaseVariationAmount;
		uniform float CZY_HeightFogIntensity;
		uniform float _UnderwaterRenderingEnabled;
		uniform float _FullySubmerged;
		uniform sampler2D _UnderwaterMask;
		uniform float CZY_FogSmoothness;
		uniform float CZY_FogOffset;
		uniform float CZY_FogIntensity;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		float2 UnStereo( float2 UV )
		{
			#if UNITY_SINGLE_PASS_STEREO
			float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex ];
			UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
			#endif
			return UV;
		}


		float3 InvertDepthDir72_g81( float3 In )
		{
			float3 result = In;
			#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
			result *= float3(1,1,-1);
			#endif
			return result;
		}


		float3 InvertDepthDir72_g76( float3 In )
		{
			float3 result = In;
			#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
			result *= float3(1,1,-1);
			#endif
			return result;
		}


		float3 InvertDepthDir72_g84( float3 In )
		{
			float3 result = In;
			#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
			result *= float3(1,1,-1);
			#endif
			return result;
		}


		float HLSL20_g86( bool enabled, bool submerged, float textureSample )
		{
			if(enabled)
			{
				if(submerged) return 1.0;
				else return textureSample;
			}
			else
			{
				return 0.0;
			}
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 UV22_g82 = ase_screenPosNorm.xy;
			float2 localUnStereo22_g82 = UnStereo( UV22_g82 );
			float2 break64_g81 = localUnStereo22_g82;
			float clampDepth69_g81 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
			#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g81 = ( 1.0 - clampDepth69_g81 );
			#else
				float staticSwitch38_g81 = clampDepth69_g81;
			#endif
			float3 appendResult39_g81 = (float3(break64_g81.x , break64_g81.y , staticSwitch38_g81));
			float4 appendResult42_g81 = (float4((appendResult39_g81*2.0 + -1.0) , 1.0));
			float4 temp_output_43_0_g81 = mul( unity_CameraInvProjection, appendResult42_g81 );
			float3 temp_output_46_0_g81 = ( (temp_output_43_0_g81).xyz / (temp_output_43_0_g81).w );
			float3 In72_g81 = temp_output_46_0_g81;
			float3 localInvertDepthDir72_g81 = InvertDepthDir72_g81( In72_g81 );
			float4 appendResult49_g81 = (float4(localInvertDepthDir72_g81 , 1.0));
			float4 temp_output_112_0_g75 = mul( unity_CameraToWorld, appendResult49_g81 );
			float preDepth115_g75 = distance( temp_output_112_0_g75 , float4( _WorldSpaceCameraPos , 0.0 ) );
			float2 UV22_g77 = ase_screenPosNorm.xy;
			float2 localUnStereo22_g77 = UnStereo( UV22_g77 );
			float2 break64_g76 = localUnStereo22_g77;
			float clampDepth69_g76 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
			#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g76 = ( 1.0 - clampDepth69_g76 );
			#else
				float staticSwitch38_g76 = clampDepth69_g76;
			#endif
			float3 appendResult39_g76 = (float3(break64_g76.x , break64_g76.y , staticSwitch38_g76));
			float4 appendResult42_g76 = (float4((appendResult39_g76*2.0 + -1.0) , 1.0));
			float4 temp_output_43_0_g76 = mul( unity_CameraInvProjection, appendResult42_g76 );
			float3 temp_output_46_0_g76 = ( (temp_output_43_0_g76).xyz / (temp_output_43_0_g76).w );
			float3 In72_g76 = temp_output_46_0_g76;
			float3 localInvertDepthDir72_g76 = InvertDepthDir72_g76( In72_g76 );
			float4 appendResult49_g76 = (float4(localInvertDepthDir72_g76 , 1.0));
			float lerpResult4_g75 = lerp( preDepth115_g75 , ( preDepth115_g75 * (( 1.0 - CZY_VariationAmount ) + (tex2D( _FogVariationTexture, (( (mul( unity_CameraToWorld, appendResult49_g76 )).xz + ( (CZY_VariationWindDirection).xz * _Time.y ) )*( 0.1 / CZY_VariationScale ) + 0.0) ).r - 0.0) * (1.0 - ( 1.0 - CZY_VariationAmount )) / (1.0 - 0.0)) ) , ( 1.0 - saturate( ( preDepth115_g75 / CZY_VariationDistance ) ) ));
			float newFogDepth19_g75 = lerpResult4_g75;
			float temp_output_21_0_g75 = ( CZY_FogDepthMultiplier * sqrt( newFogDepth19_g75 ) );
			float temp_output_1_0_g80 = temp_output_21_0_g75;
			float4 lerpResult28_g80 = lerp( CZY_FogColor1 , CZY_FogColor2 , saturate( ( temp_output_1_0_g80 / CZY_FogColorStart1 ) ));
			float4 lerpResult41_g80 = lerp( saturate( lerpResult28_g80 ) , CZY_FogColor3 , saturate( ( ( CZY_FogColorStart1 - temp_output_1_0_g80 ) / ( CZY_FogColorStart1 - CZY_FogColorStart2 ) ) ));
			float4 lerpResult35_g80 = lerp( lerpResult41_g80 , CZY_FogColor4 , saturate( ( ( CZY_FogColorStart2 - temp_output_1_0_g80 ) / ( CZY_FogColorStart2 - CZY_FogColorStart3 ) ) ));
			float4 lerpResult113_g80 = lerp( lerpResult35_g80 , CZY_FogColor5 , saturate( ( ( CZY_FogColorStart3 - temp_output_1_0_g80 ) / ( CZY_FogColorStart3 - CZY_FogColorStart4 ) ) ));
			float4 temp_output_43_0_g75 = lerpResult113_g80;
			float3 hsvTorgb30_g75 = RGBToHSV( temp_output_43_0_g75.rgb );
			float3 ase_worldPos = i.worldPos;
			float3 appendResult59_g75 = (float3(1.0 , CZY_LightFlareSquish , 1.0));
			float3 normalizeResult50_g75 = normalize( ( ( ase_worldPos * appendResult59_g75 ) - _WorldSpaceCameraPos ) );
			float dotResult52_g75 = dot( normalizeResult50_g75 , CZY_SunDirection );
			half LightMask66_g75 = saturate( pow( abs( ( (dotResult52_g75*0.5 + 0.5) * CZY_LightIntensity ) ) , CZY_LightFalloff ) );
			float temp_output_26_0_g75 = ( (temp_output_43_0_g75).a * saturate( temp_output_21_0_g75 ) );
			float3 hsvTorgb2_g79 = RGBToHSV( ( CZY_LightColor * hsvTorgb30_g75.z * saturate( ( LightMask66_g75 * ( 1.5 * temp_output_26_0_g75 ) ) ) ).rgb );
			float3 hsvTorgb3_g79 = HSVToRGB( float3(hsvTorgb2_g79.x,saturate( ( hsvTorgb2_g79.y + CZY_FilterSaturation ) ),( hsvTorgb2_g79.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g79 = ( float4( hsvTorgb3_g79 , 0.0 ) * CZY_FilterColor );
			float3 normalizeResult65_g75 = normalize( half3(0,0,0) );
			float3 normalizeResult64_g75 = normalize( CZY_MoonDirection );
			float dotResult62_g75 = dot( normalizeResult65_g75 , normalizeResult64_g75 );
			half MoonMask75_g75 = saturate( pow( abs( ( saturate( (dotResult62_g75*1.0 + 0.0) ) * CZY_LightIntensity ) ) , ( CZY_LightFalloff * 3.0 ) ) );
			float3 hsvTorgb2_g78 = RGBToHSV( ( temp_output_43_0_g75 + ( hsvTorgb30_g75.z * saturate( ( temp_output_26_0_g75 * MoonMask75_g75 ) ) * CZY_FogMoonFlareColor ) ).rgb );
			float3 hsvTorgb3_g78 = HSVToRGB( float3(hsvTorgb2_g78.x,saturate( ( hsvTorgb2_g78.y + CZY_FilterSaturation ) ),( hsvTorgb2_g78.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g78 = ( float4( hsvTorgb3_g78 , 0.0 ) * CZY_FilterColor );
			float2 UV22_g85 = ase_screenPosNorm.xy;
			float2 localUnStereo22_g85 = UnStereo( UV22_g85 );
			float2 break64_g84 = localUnStereo22_g85;
			float clampDepth69_g84 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
			#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g84 = ( 1.0 - clampDepth69_g84 );
			#else
				float staticSwitch38_g84 = clampDepth69_g84;
			#endif
			float3 appendResult39_g84 = (float3(break64_g84.x , break64_g84.y , staticSwitch38_g84));
			float4 appendResult42_g84 = (float4((appendResult39_g84*2.0 + -1.0) , 1.0));
			float4 temp_output_43_0_g84 = mul( unity_CameraInvProjection, appendResult42_g84 );
			float3 temp_output_46_0_g84 = ( (temp_output_43_0_g84).xyz / (temp_output_43_0_g84).w );
			float3 In72_g84 = temp_output_46_0_g84;
			float3 localInvertDepthDir72_g84 = InvertDepthDir72_g84( In72_g84 );
			float4 appendResult49_g84 = (float4(localInvertDepthDir72_g84 , 1.0));
			float4 temp_output_18_0_g83 = mul( unity_CameraToWorld, appendResult49_g84 );
			float mulTime63_g83 = _Time.y * 0.01;
			float eyeDepth31_g83 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float temp_output_121_0_g75 = ( ( 1.0 - saturate( ( ( temp_output_18_0_g83.y - CZY_HeightFogBase ) / ( CZY_HeightFogTransition + ( ( 1.0 - tex2D( _FogVariationTexture, ((temp_output_18_0_g83).xz*( 1.0 / CZY_HeightFogBaseVariationScale ) + mulTime63_g83) ).r ) * CZY_HeightFogBaseVariationAmount ) ) ) ) ) * saturate( ( eyeDepth31_g83 * 0.01 * CZY_HeightFogIntensity ) ) * CZY_HeightFogColor.a );
			float4 lerpResult108_g75 = lerp( ( ( temp_output_10_0_g79 * CZY_SunFilterColor ) + temp_output_10_0_g78 ) , CZY_HeightFogColor , temp_output_121_0_g75);
			o.Emission = lerpResult108_g75.rgb;
			bool enabled20_g86 =(bool)_UnderwaterRenderingEnabled;
			bool submerged20_g86 =(bool)_FullySubmerged;
			float textureSample20_g86 = tex2Dlod( _UnderwaterMask, float4( ase_screenPosNorm.xy, 0, 0.0) ).r;
			float localHLSL20_g86 = HLSL20_g86( enabled20_g86 , submerged20_g86 , textureSample20_g86 );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float finalAlpha36_g75 = temp_output_26_0_g75;
			float lerpResult104_g75 = lerp( finalAlpha36_g75 , ( saturate( ( 1.0 - ( temp_output_112_0_g75.y * 0.001 ) ) ) * finalAlpha36_g75 ) , ( 1.0 - saturate( ( preDepth115_g75 / ( _ProjectionParams.z * 1.0 ) ) ) ));
			float ModifiedFogAlpha40_g75 = saturate( lerpResult104_g75 );
			o.Alpha = ( ( 1.0 - localHLSL20_g86 ) * max( temp_output_121_0_g75 , saturate( ( ( 1.0 - saturate( ( ( ase_worldPos.y * ( 0.1 / ( ( CZY_FogSmoothness * length( ase_objectScale ) ) * 10.0 ) ) ) + ( 1.0 - CZY_FogOffset ) ) ) ) * CZY_FogIntensity * ModifiedFogAlpha40_g75 ) ) ) );
		}

		ENDCG
	}
	CustomEditor "EmptyShaderGUI"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;799.9159,-507.1588;Float;False;True;-1;2;EmptyShaderGUI;0;0;Unlit;Distant Lands/Cozy/BiRP/Stylized Fog (Physical Height);False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Front;2;False;;7;False;;False;0;False;;0;False;;True;0;Custom;0.5;True;False;1;True;Custom;HeightFog;Transparent;All;12;all;True;True;True;True;0;False;;True;222;False;;255;False;;255;False;;6;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;6;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;297;274.0955,-417.3223;Inherit;False;Stylized Fog (Physical Height);0;;75;6863d88adda26194cbbb00d58f08515c;0;0;2;COLOR;0;FLOAT;123
WireConnection;0;2;297;0
WireConnection;0;9;297;123
ASEEND*/
//CHKSM=7F7EB7162FBFFFB339647887B089DBC706AD0173