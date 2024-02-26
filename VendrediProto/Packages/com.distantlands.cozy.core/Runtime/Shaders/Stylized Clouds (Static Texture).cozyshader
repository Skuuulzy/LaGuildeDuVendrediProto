// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/BiRP/Stylized Clouds (Single Texture)"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent-50" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		Stencil
		{
			Ref 221
			Comp Always
			Pass Replace
		}
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D CZY_CloudTexture;
		uniform float3 CZY_TexturePanDirection;
		uniform float CZY_WindSpeed;
		uniform float _UnderwaterRenderingEnabled;
		uniform float _FullySubmerged;
		uniform sampler2D _UnderwaterMask;
		uniform float CZY_ClippingThreshold;
		uniform float _Cutoff = 0.5;


		float HLSL20_g79( bool enabled, bool submerged, float textureSample )
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
			float3 break15_g77 = CZY_TexturePanDirection;
			float mulTime18_g77 = _Time.y * ( CZY_WindSpeed * 0.005 );
			float TIme16_g77 = mulTime18_g77;
			float2 appendResult11_g77 = (float2(( break15_g77.x * 0.0025 * TIme16_g77 ) , ( TIme16_g77 * 0.0025 * break15_g77.z )));
			float2 uv_TexCoord10_g77 = i.uv_texcoord + appendResult11_g77;
			float cos17_g77 = cos( ( TIme16_g77 * break15_g77.y ) );
			float sin17_g77 = sin( ( TIme16_g77 * break15_g77.y ) );
			float2 rotator17_g77 = mul( uv_TexCoord10_g77 - float2( 0.5,0.5 ) , float2x2( cos17_g77 , -sin17_g77 , sin17_g77 , cos17_g77 )) + float2( 0.5,0.5 );
			float4 tex2DNode25_g77 = tex2D( CZY_CloudTexture, rotator17_g77 );
			float4 CloudTex6_g77 = tex2DNode25_g77;
			o.Emission = ( CloudTex6_g77 * CloudTex6_g77 ).rgb;
			float CloudAlpha3_g77 = ( tex2DNode25_g77.r * tex2DNode25_g77.g * tex2DNode25_g77.b * tex2DNode25_g77.a );
			bool enabled20_g79 =(bool)_UnderwaterRenderingEnabled;
			bool submerged20_g79 =(bool)_FullySubmerged;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float textureSample20_g79 = tex2Dlod( _UnderwaterMask, float4( ase_screenPosNorm.xy, 0, 0.0) ).r;
			float localHLSL20_g79 = HLSL20_g79( enabled20_g79 , submerged20_g79 , textureSample20_g79 );
			o.Alpha = ( CloudAlpha3_g77 * ( 1.0 - localHLSL20_g79 ) );
			clip( CZY_ClippingThreshold - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows exclude_path:deferred nofog 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "EmptyShaderGUI"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-695.2959,-681.1561;Float;False;True;-1;2;EmptyShaderGUI;0;0;Unlit;Distant Lands/Cozy/BiRP/Stylized Clouds (Single Texture);False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;-50;True;TransparentCutout;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;True;221;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;4;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;1211;-1104,-608;Inherit;False;Stylized Clouds (Single Texture);0;;77;0a9a75a643e85274a80056aa53978903;0;0;3;COLOR;0;FLOAT;31;FLOAT;32
WireConnection;0;2;1211;0
WireConnection;0;9;1211;31
WireConnection;0;10;1211;32
ASEEND*/
//CHKSM=84738334D4A87CBE2722C511C0CA7F67BA075827