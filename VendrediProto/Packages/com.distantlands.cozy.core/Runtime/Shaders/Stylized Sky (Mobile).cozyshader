// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/BiRP/Stylized Sky (Mobile)"
{
	Properties
	{
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent-100" "IsEmissive" = "true"  }
		Cull Front
		Stencil
		{
			Ref 220
			Comp Always
			Pass Replace
		}
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 CZY_HorizonColor;
		uniform float CZY_FilterSaturation;
		uniform float CZY_FilterValue;
		uniform float4 CZY_FilterColor;
		uniform float4 CZY_ZenithColor;
		uniform float CZY_Power;
		uniform float3 CZY_SunDirection;
		uniform float CZY_SunFlareFalloff;
		uniform float4 CZY_SunFlareColor;
		uniform float4 CZY_SunColor;
		uniform float CZY_SunSize;
		uniform sampler2D CZY_GalaxyVariationMap;
		uniform sampler2D CZY_StarMap;
		uniform float4 CZY_StarColor;


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

		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 hsvTorgb2_g6 = RGBToHSV( CZY_HorizonColor.rgb );
			float3 hsvTorgb3_g6 = HSVToRGB( float3(hsvTorgb2_g6.x,saturate( ( hsvTorgb2_g6.y + CZY_FilterSaturation ) ),( hsvTorgb2_g6.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g6 = ( float4( hsvTorgb3_g6 , 0.0 ) * CZY_FilterColor );
			float4 HorizonColor76_g5 = temp_output_10_0_g6;
			float3 hsvTorgb2_g7 = RGBToHSV( CZY_ZenithColor.rgb );
			float3 hsvTorgb3_g7 = HSVToRGB( float3(hsvTorgb2_g7.x,saturate( ( hsvTorgb2_g7.y + CZY_FilterSaturation ) ),( hsvTorgb2_g7.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g7 = ( float4( hsvTorgb3_g7 , 0.0 ) * CZY_FilterColor );
			float4 ZenithColor78_g5 = temp_output_10_0_g7;
			float2 temp_output_17_0_g5 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult9_g5 = dot( temp_output_17_0_g5 , temp_output_17_0_g5 );
			float SimpleGradient33_g5 = dotResult9_g5;
			float GradientPos44_g5 = ( 1.0 - saturate( pow( saturate( (0.0 + (SimpleGradient33_g5 - 0.0) * (2.0 - 0.0) / (1.0 - 0.0)) ) , CZY_Power ) ) );
			float4 lerpResult65_g5 = lerp( HorizonColor76_g5 , ZenithColor78_g5 , saturate( GradientPos44_g5 ));
			float4 SimpleSkyGradient114_g5 = lerpResult65_g5;
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult28_g5 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult39_g5 = dot( normalizeResult28_g5 , CZY_SunDirection );
			float SunDot69_g5 = dotResult39_g5;
			float3 hsvTorgb2_g9 = RGBToHSV( CZY_SunFlareColor.rgb );
			float3 hsvTorgb3_g9 = HSVToRGB( float3(hsvTorgb2_g9.x,saturate( ( hsvTorgb2_g9.y + CZY_FilterSaturation ) ),( hsvTorgb2_g9.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g9 = ( float4( hsvTorgb3_g9 , 0.0 ) * CZY_FilterColor );
			half4 SunFlare121_g5 = abs( ( saturate( pow( abs( (SunDot69_g5*0.5 + 0.5) ) , CZY_SunFlareFalloff ) ) * temp_output_10_0_g9 ) );
			float4 SunRender41_g5 = ( CZY_SunColor * ( ( 1.0 - SunDot69_g5 ) > ( pow( CZY_SunSize , 3.0 ) * 0.0007 ) ? 0.0 : 1.0 ) );
			float2 Pos8_g5 = i.uv_texcoord;
			float mulTime85_g5 = _Time.y * 0.005;
			float cos82_g5 = cos( mulTime85_g5 );
			float sin82_g5 = sin( mulTime85_g5 );
			float2 rotator82_g5 = mul( Pos8_g5 - float2( 0.5,0.5 ) , float2x2( cos82_g5 , -sin82_g5 , sin82_g5 , cos82_g5 )) + float2( 0.5,0.5 );
			float mulTime81_g5 = _Time.y * -0.02;
			float simplePerlin2D97_g5 = snoise( (Pos8_g5*5.0 + mulTime81_g5) );
			simplePerlin2D97_g5 = simplePerlin2D97_g5*0.5 + 0.5;
			float StarPlacementPattern108_g5 = saturate( ( min( tex2D( CZY_GalaxyVariationMap, (Pos8_g5*5.0 + mulTime85_g5) ).r , tex2D( CZY_GalaxyVariationMap, (rotator82_g5*2.0 + 0.0) ).r ) * simplePerlin2D97_g5 * (0.2 + (SimpleGradient33_g5 - 0.0) * (0.0 - 0.2) / (1.0 - 0.0)) ) );
			float2 panner104_g5 = ( 1.0 * _Time.y * float2( 0.0007,0 ) + Pos8_g5);
			float mulTime94_g5 = _Time.y * 0.001;
			float cos92_g5 = cos( 0.01 * _Time.y );
			float sin92_g5 = sin( 0.01 * _Time.y );
			float2 rotator92_g5 = mul( Pos8_g5 - float2( 0.5,0.5 ) , float2x2( cos92_g5 , -sin92_g5 , sin92_g5 , cos92_g5 )) + float2( 0.5,0.5 );
			float4 StarPattern101_g5 = saturate( ( ( ( StarPlacementPattern108_g5 * min( tex2D( CZY_StarMap, (panner104_g5*4.0 + mulTime94_g5) ).r , tex2D( CZY_StarMap, (rotator92_g5*0.1 + 0.0) ).r ) ) * ( 1.0 - HorizonColor76_g5.r ) ) * CZY_StarColor ) );
			o.Emission = ( SimpleSkyGradient114_g5 + SunFlare121_g5 + SunRender41_g5 + HorizonColor76_g5 + StarPattern101_g5 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "EmptyShaderGUI"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;104.1543,85.88161;Float;False;True;-1;2;EmptyShaderGUI;0;0;Unlit;Distant Lands/Cozy/BiRP/Stylized Sky (Mobile);False;False;False;False;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;False;False;Front;0;False;;7;False;;False;0;False;;0;False;;True;0;Translucent;0.5;True;False;-100;False;Opaque;;Transparent;All;12;all;True;True;True;True;0;False;;True;220;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;598;-158.202,154.0788;Inherit;False;Stylized Sky (Mobile);0;;5;688f603026dc18c468fc058bac44ec60;0;0;2;COLOR;0;FLOAT;141
WireConnection;0;2;598;0
ASEEND*/
//CHKSM=242AC7C25612CE33D45C37E0AD702C8BE828589E