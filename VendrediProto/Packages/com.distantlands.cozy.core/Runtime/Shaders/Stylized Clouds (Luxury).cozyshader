// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/BiRP/Stylized Clouds (Luxury)"
{
	Properties
	{
		CZY_LuxuryVariationTexture("CZY_LuxuryVariationTexture", 2D) = "white" {}
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

		uniform float4 CZY_CloudColor;
		uniform float CZY_FilterSaturation;
		uniform float CZY_FilterValue;
		uniform float4 CZY_FilterColor;
		uniform float4 CZY_CloudFilterColor;
		uniform sampler2D CZY_CirrostratusTexture;
		uniform float CZY_CirrostratusMoveSpeed;
		uniform sampler2D CZY_LuxuryVariationTexture;
		uniform float CZY_CirrostratusMultiplier;
		uniform sampler2D CZY_AltocumulusTexture;
		uniform float CZY_MainCloudScale;
		uniform float CZY_WindSpeed;
		uniform float CZY_AltocumulusMultiplier;
		uniform sampler2D CZY_CirrusTexture;
		uniform float CZY_CirrusMoveSpeed;
		uniform float CZY_CirrusMultiplier;
		uniform sampler2D CZY_ChemtrailsTexture;
		uniform float CZY_ChemtrailsMoveSpeed;
		uniform float CZY_ChemtrailsMultiplier;
		uniform sampler2D CZY_PartlyCloudyTexture;
		uniform float CZY_CumulusCoverageMultiplier;
		uniform sampler2D CZY_MostlyCloudyTexture;
		uniform sampler2D CZY_OvercastTexture;
		uniform sampler2D CZY_LowNimbusTexture;
		uniform float CZY_NimbusMultiplier;
		uniform sampler2D CZY_MidNimbusTexture;
		uniform sampler2D CZY_HighNimbusTexture;
		uniform sampler2D CZY_LowBorderTexture;
		uniform float CZY_BorderHeight;
		uniform sampler2D CZY_HighBorderTexture;
		uniform float CZY_CloudThickness;
		uniform float _UnderwaterRenderingEnabled;
		uniform float _FullySubmerged;
		uniform sampler2D _UnderwaterMask;
		uniform float CZY_ClippingThreshold;
		uniform float _Cutoff = 0.5;


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


		float HLSL20_g80( bool enabled, bool submerged, float textureSample )
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
			float3 hsvTorgb2_g78 = RGBToHSV( CZY_CloudColor.rgb );
			float3 hsvTorgb3_g78 = HSVToRGB( float3(hsvTorgb2_g78.x,saturate( ( hsvTorgb2_g78.y + CZY_FilterSaturation ) ),( hsvTorgb2_g78.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g78 = ( float4( hsvTorgb3_g78 , 0.0 ) * CZY_FilterColor );
			float4 CloudColor13_g77 = ( temp_output_10_0_g78 * CZY_CloudFilterColor );
			float2 Pos6_g77 = i.uv_texcoord;
			float mulTime104_g77 = _Time.y * 0.01;
			float simplePerlin2D116_g77 = snoise( (Pos6_g77*1.0 + mulTime104_g77)*2.0 );
			float mulTime102_g77 = _Time.y * CZY_CirrostratusMoveSpeed;
			float cos110_g77 = cos( ( mulTime102_g77 * 0.01 ) );
			float sin110_g77 = sin( ( mulTime102_g77 * 0.01 ) );
			float2 rotator110_g77 = mul( Pos6_g77 - float2( 0.5,0.5 ) , float2x2( cos110_g77 , -sin110_g77 , sin110_g77 , cos110_g77 )) + float2( 0.5,0.5 );
			float cos112_g77 = cos( ( mulTime102_g77 * -0.02 ) );
			float sin112_g77 = sin( ( mulTime102_g77 * -0.02 ) );
			float2 rotator112_g77 = mul( Pos6_g77 - float2( 0.5,0.5 ) , float2x2( cos112_g77 , -sin112_g77 , sin112_g77 , cos112_g77 )) + float2( 0.5,0.5 );
			float mulTime118_g77 = _Time.y * 0.01;
			float simplePerlin2D115_g77 = snoise( (Pos6_g77*1.0 + mulTime118_g77) );
			simplePerlin2D115_g77 = simplePerlin2D115_g77*0.5 + 0.5;
			float4 CirrostratusPattern134_g77 = ( ( saturate( simplePerlin2D116_g77 ) * tex2D( CZY_CirrostratusTexture, (rotator110_g77*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrostratusTexture, (rotator112_g77*1.0 + 0.0) ) * saturate( simplePerlin2D115_g77 ) ) );
			float2 temp_output_123_0_g77 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult120_g77 = dot( temp_output_123_0_g77 , temp_output_123_0_g77 );
			float2 temp_output_4_0_g83 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g83 = dot( temp_output_4_0_g83 , temp_output_4_0_g83 );
			float temp_output_14_0_g83 = ( CZY_CirrostratusMultiplier * 0.5 );
			float3 appendResult131_g77 = (float3(( CirrostratusPattern134_g77 * saturate( (0.0 + (dotResult120_g77 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g83*48.2 + (-15.0 + (temp_output_14_0_g83 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g83 ) ) ).rgb));
			float temp_output_130_0_g77 = length( appendResult131_g77 );
			float CirrostratusColoring135_g77 = temp_output_130_0_g77;
			float CirrostratusAlpha136_g77 = temp_output_130_0_g77;
			float lerpResult260_g77 = lerp( 1.0 , CirrostratusColoring135_g77 , CirrostratusAlpha136_g77);
			float mulTime212_g77 = _Time.y * ( 0.001 * CZY_WindSpeed );
			float TIme213_g77 = mulTime212_g77;
			float2 cloudPosition211_g77 = (Pos6_g77*( 18.0 / CZY_MainCloudScale ) + ( TIme213_g77 * float2( 0.2,-0.4 ) ));
			float4 tex2DNode315_g77 = tex2D( CZY_AltocumulusTexture, cloudPosition211_g77 );
			float altocumulusColor216_g77 = tex2DNode315_g77.r;
			float2 temp_output_4_0_g89 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g89 = dot( temp_output_4_0_g89 , temp_output_4_0_g89 );
			float temp_output_14_0_g89 = CZY_AltocumulusMultiplier;
			float altocumulusAlpha217_g77 = tex2DNode315_g77.a;
			float temp_output_253_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g89*48.2 + (-15.0 + (temp_output_14_0_g89 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g89 ) ) * altocumulusAlpha217_g77 );
			float lerpResult252_g77 = lerp( 1.0 , altocumulusColor216_g77 , temp_output_253_0_g77);
			float finalAcColor288_g77 = lerpResult252_g77;
			float finalAcAlpha286_g77 = temp_output_253_0_g77;
			float lerpResult284_g77 = lerp( lerpResult260_g77 , finalAcColor288_g77 , finalAcAlpha286_g77);
			float mulTime64_g77 = _Time.y * 0.01;
			float simplePerlin2D76_g77 = snoise( (Pos6_g77*1.0 + mulTime64_g77)*2.0 );
			float mulTime62_g77 = _Time.y * CZY_CirrusMoveSpeed;
			float cos70_g77 = cos( ( mulTime62_g77 * 0.01 ) );
			float sin70_g77 = sin( ( mulTime62_g77 * 0.01 ) );
			float2 rotator70_g77 = mul( Pos6_g77 - float2( 0.5,0.5 ) , float2x2( cos70_g77 , -sin70_g77 , sin70_g77 , cos70_g77 )) + float2( 0.5,0.5 );
			float cos72_g77 = cos( ( mulTime62_g77 * -0.02 ) );
			float sin72_g77 = sin( ( mulTime62_g77 * -0.02 ) );
			float2 rotator72_g77 = mul( Pos6_g77 - float2( 0.5,0.5 ) , float2x2( cos72_g77 , -sin72_g77 , sin72_g77 , cos72_g77 )) + float2( 0.5,0.5 );
			float mulTime78_g77 = _Time.y * 0.01;
			float simplePerlin2D75_g77 = snoise( (Pos6_g77*1.0 + mulTime78_g77) );
			simplePerlin2D75_g77 = simplePerlin2D75_g77*0.5 + 0.5;
			float4 CirrusPattern79_g77 = ( ( saturate( simplePerlin2D76_g77 ) * tex2D( CZY_CirrusTexture, (rotator70_g77*1.5 + 0.75) ) ) + ( tex2D( CZY_CirrusTexture, (rotator72_g77*1.0 + 0.0) ) * saturate( simplePerlin2D75_g77 ) ) );
			float2 temp_output_84_0_g77 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult81_g77 = dot( temp_output_84_0_g77 , temp_output_84_0_g77 );
			float2 temp_output_4_0_g82 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g82 = dot( temp_output_4_0_g82 , temp_output_4_0_g82 );
			float temp_output_14_0_g82 = ( CZY_CirrusMultiplier * 0.5 );
			float3 appendResult95_g77 = (float3(( CirrusPattern79_g77 * saturate( (0.0 + (dotResult81_g77 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g82*48.2 + (-15.0 + (temp_output_14_0_g82 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g82 ) ) ).rgb));
			float temp_output_94_0_g77 = length( appendResult95_g77 );
			float CirrusColoring96_g77 = temp_output_94_0_g77;
			float CirrusAlpha97_g77 = temp_output_94_0_g77;
			float lerpResult283_g77 = lerp( lerpResult284_g77 , CirrusColoring96_g77 , CirrusAlpha97_g77);
			float mulTime33_g77 = _Time.y * 0.01;
			float simplePerlin2D40_g77 = snoise( (Pos6_g77*1.0 + mulTime33_g77)*2.0 );
			float mulTime31_g77 = _Time.y * CZY_ChemtrailsMoveSpeed;
			float cos32_g77 = cos( ( mulTime31_g77 * 0.01 ) );
			float sin32_g77 = sin( ( mulTime31_g77 * 0.01 ) );
			float2 rotator32_g77 = mul( Pos6_g77 - float2( 0.5,0.5 ) , float2x2( cos32_g77 , -sin32_g77 , sin32_g77 , cos32_g77 )) + float2( 0.5,0.5 );
			float cos39_g77 = cos( ( mulTime31_g77 * -0.02 ) );
			float sin39_g77 = sin( ( mulTime31_g77 * -0.02 ) );
			float2 rotator39_g77 = mul( Pos6_g77 - float2( 0.5,0.5 ) , float2x2( cos39_g77 , -sin39_g77 , sin39_g77 , cos39_g77 )) + float2( 0.5,0.5 );
			float mulTime29_g77 = _Time.y * 0.01;
			float simplePerlin2D41_g77 = snoise( (Pos6_g77*1.0 + mulTime29_g77)*4.0 );
			float4 ChemtrailsPattern46_g77 = ( ( saturate( simplePerlin2D40_g77 ) * tex2D( CZY_ChemtrailsTexture, (rotator32_g77*0.5 + 0.0) ) ) + ( tex2D( CZY_ChemtrailsTexture, rotator39_g77 ) * saturate( simplePerlin2D41_g77 ) ) );
			float2 temp_output_24_0_g77 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult26_g77 = dot( temp_output_24_0_g77 , temp_output_24_0_g77 );
			float2 temp_output_4_0_g81 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g81 = dot( temp_output_4_0_g81 , temp_output_4_0_g81 );
			float temp_output_14_0_g81 = ( CZY_ChemtrailsMultiplier * 0.5 );
			float3 appendResult58_g77 = (float3(( ChemtrailsPattern46_g77 * saturate( (0.4 + (dotResult26_g77 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) * saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g81*48.2 + (-15.0 + (temp_output_14_0_g81 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g81 ) ) ).rgb));
			float temp_output_59_0_g77 = length( appendResult58_g77 );
			float ChemtrailsColoring60_g77 = temp_output_59_0_g77;
			float ChemtrailsAlpha61_g77 = temp_output_59_0_g77;
			float lerpResult265_g77 = lerp( lerpResult283_g77 , ChemtrailsColoring60_g77 , ChemtrailsAlpha61_g77);
			float4 tex2DNode319_g77 = tex2D( CZY_PartlyCloudyTexture, cloudPosition211_g77 );
			float PartlyCloudyColor220_g77 = tex2DNode319_g77.r;
			float2 temp_output_4_0_g88 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g88 = dot( temp_output_4_0_g88 , temp_output_4_0_g88 );
			float temp_output_294_0_g77 = ( CZY_CumulusCoverageMultiplier * 1.0 );
			float temp_output_14_0_g88 = saturate( (0.0 + (min( temp_output_294_0_g77 , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
			float PartlyCloudyAlpha219_g77 = tex2DNode319_g77.a;
			float temp_output_249_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g88*48.2 + (-15.0 + (temp_output_14_0_g88 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g88 ) ) * PartlyCloudyAlpha219_g77 );
			float lerpResult242_g77 = lerp( 1.0 , PartlyCloudyColor220_g77 , temp_output_249_0_g77);
			float4 tex2DNode308_g77 = tex2D( CZY_MostlyCloudyTexture, cloudPosition211_g77 );
			float MostlyCloudyColor222_g77 = tex2DNode308_g77.r;
			float2 temp_output_4_0_g87 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g87 = dot( temp_output_4_0_g87 , temp_output_4_0_g87 );
			float temp_output_14_0_g87 = saturate( (0.0 + (min( ( temp_output_294_0_g77 - 0.3 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
			float MostlyCloudyAlpha221_g77 = tex2DNode308_g77.a;
			float temp_output_226_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g87*48.2 + (-15.0 + (temp_output_14_0_g87 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g87 ) ) * MostlyCloudyAlpha221_g77 );
			float lerpResult243_g77 = lerp( lerpResult242_g77 , MostlyCloudyColor222_g77 , temp_output_226_0_g77);
			float4 tex2DNode309_g77 = tex2D( CZY_OvercastTexture, cloudPosition211_g77 );
			float OvercastCloudyColoring223_g77 = tex2DNode309_g77.r;
			float2 temp_output_4_0_g91 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g91 = dot( temp_output_4_0_g91 , temp_output_4_0_g91 );
			float temp_output_14_0_g91 = saturate( (0.0 + (min( ( temp_output_294_0_g77 - 0.7 ) , 0.35 ) - 0.0) * (1.0 - 0.0) / (0.35 - 0.0)) );
			float OvercastCloudyAlpha224_g77 = tex2DNode309_g77.a;
			float temp_output_231_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g91*48.2 + (-15.0 + (temp_output_14_0_g91 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g91 ) ) * OvercastCloudyAlpha224_g77 );
			float lerpResult239_g77 = lerp( lerpResult243_g77 , OvercastCloudyColoring223_g77 , temp_output_231_0_g77);
			float cumulusCloudColor237_g77 = saturate( lerpResult239_g77 );
			float cumulusAlpha232_g77 = saturate( ( temp_output_249_0_g77 + temp_output_226_0_g77 + temp_output_231_0_g77 ) );
			float lerpResult269_g77 = lerp( lerpResult265_g77 , cumulusCloudColor237_g77 , cumulusAlpha232_g77);
			float mulTime202_g77 = _Time.y * 0.005;
			float cos201_g77 = cos( mulTime202_g77 );
			float sin201_g77 = sin( mulTime202_g77 );
			float2 rotator201_g77 = mul( Pos6_g77 - float2( 0.5,0.5 ) , float2x2( cos201_g77 , -sin201_g77 , sin201_g77 , cos201_g77 )) + float2( 0.5,0.5 );
			float4 tex2DNode312_g77 = tex2D( CZY_LowNimbusTexture, rotator201_g77 );
			float lowNimbusColor196_g77 = tex2DNode312_g77.r;
			float2 temp_output_4_0_g86 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g86 = dot( temp_output_4_0_g86 , temp_output_4_0_g86 );
			float temp_output_188_0_g77 = ( CZY_NimbusMultiplier * 0.5 );
			float temp_output_14_0_g86 = saturate( (0.0 + (min( temp_output_188_0_g77 , 2.0 ) - 0.0) * (1.0 - 0.0) / (2.0 - 0.0)) );
			float lowNimbusAlpha197_g77 = tex2DNode312_g77.a;
			float temp_output_166_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g86*48.2 + (-15.0 + (temp_output_14_0_g86 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g86 ) ) * lowNimbusAlpha197_g77 );
			float lerpResult173_g77 = lerp( 1.0 , lowNimbusColor196_g77 , temp_output_166_0_g77);
			float4 tex2DNode313_g77 = tex2D( CZY_MidNimbusTexture, rotator201_g77 );
			float mediumNimbusColor198_g77 = tex2DNode313_g77.r;
			float2 temp_output_4_0_g92 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g92 = dot( temp_output_4_0_g92 , temp_output_4_0_g92 );
			float temp_output_14_0_g92 = saturate( (0.0 + (min( ( temp_output_188_0_g77 - 0.2 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
			float mediumNimbusAlpha199_g77 = tex2DNode313_g77.a;
			float temp_output_164_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g92*48.2 + (-15.0 + (temp_output_14_0_g92 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g92 ) ) * mediumNimbusAlpha199_g77 );
			float lerpResult175_g77 = lerp( lerpResult173_g77 , mediumNimbusColor198_g77 , temp_output_164_0_g77);
			float4 tex2DNode314_g77 = tex2D( CZY_HighNimbusTexture, rotator201_g77 );
			float highNimbusColor204_g77 = tex2DNode314_g77.r;
			float2 temp_output_4_0_g85 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g85 = dot( temp_output_4_0_g85 , temp_output_4_0_g85 );
			float temp_output_14_0_g85 = saturate( (0.0 + (min( ( temp_output_188_0_g77 - 0.7 ) , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
			float HighNimbusAlpha205_g77 = tex2DNode314_g77.a;
			float temp_output_162_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g85*48.2 + (-15.0 + (temp_output_14_0_g85 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g85 ) ) * HighNimbusAlpha205_g77 );
			float lerpResult177_g77 = lerp( lerpResult175_g77 , highNimbusColor204_g77 , temp_output_162_0_g77);
			float nimbusColoring180_g77 = saturate( lerpResult177_g77 );
			float nimbusAlpha172_g77 = saturate( ( temp_output_166_0_g77 + temp_output_164_0_g77 + temp_output_162_0_g77 ) );
			float lerpResult274_g77 = lerp( lerpResult269_g77 , nimbusColoring180_g77 , nimbusAlpha172_g77);
			float mulTime195_g77 = _Time.y * 0.005;
			float cos194_g77 = cos( mulTime195_g77 );
			float sin194_g77 = sin( mulTime195_g77 );
			float2 rotator194_g77 = mul( Pos6_g77 - float2( 0.5,0.5 ) , float2x2( cos194_g77 , -sin194_g77 , sin194_g77 , cos194_g77 )) + float2( 0.5,0.5 );
			float4 tex2DNode311_g77 = tex2D( CZY_LowBorderTexture, rotator194_g77 );
			float MediumBorderColor189_g77 = tex2DNode311_g77.r;
			float2 temp_output_4_0_g84 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g84 = dot( temp_output_4_0_g84 , temp_output_4_0_g84 );
			float temp_output_14_0_g84 = saturate( (0.0 + (min( CZY_BorderHeight , 0.3 ) - 0.0) * (1.0 - 0.0) / (0.3 - 0.0)) );
			float MediumBorderAlpha190_g77 = tex2DNode311_g77.a;
			float temp_output_140_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g84*48.2 + (-15.0 + (temp_output_14_0_g84 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g84 ) ) * MediumBorderAlpha190_g77 );
			float lerpResult142_g77 = lerp( 1.0 , MediumBorderColor189_g77 , temp_output_140_0_g77);
			float4 tex2DNode310_g77 = tex2D( CZY_HighBorderTexture, rotator194_g77 );
			float HighBorderColoring191_g77 = tex2DNode310_g77.r;
			float2 temp_output_4_0_g90 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult3_g90 = dot( temp_output_4_0_g90 , temp_output_4_0_g90 );
			float temp_output_14_0_g90 = saturate( (0.0 + (min( ( CZY_BorderHeight - 0.5 ) , 0.2 ) - 0.0) * (1.0 - 0.0) / (0.2 - 0.0)) );
			float HighBorderAlpha192_g77 = tex2DNode310_g77.a;
			float temp_output_159_0_g77 = ( saturate( ( ( (-1.0 + (tex2D( CZY_LuxuryVariationTexture, (i.uv_texcoord*10.0 + 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) + (dotResult3_g90*48.2 + (-15.0 + (temp_output_14_0_g90 - 0.0) * (1.0 - -15.0) / (1.0 - 0.0))) ) + temp_output_14_0_g90 ) ) * HighBorderAlpha192_g77 );
			float lerpResult141_g77 = lerp( lerpResult142_g77 , HighBorderColoring191_g77 , temp_output_159_0_g77);
			float borderCloudsColor257_g77 = saturate( lerpResult141_g77 );
			float borderAlpha169_g77 = saturate( ( temp_output_140_0_g77 + temp_output_159_0_g77 ) );
			float lerpResult272_g77 = lerp( lerpResult274_g77 , borderCloudsColor257_g77 , borderAlpha169_g77);
			float cloudColoring270_g77 = lerpResult272_g77;
			float4 lerpResult12_g77 = lerp( float4( 0,0,0,0 ) , CloudColor13_g77 , cloudColoring270_g77);
			o.Emission = lerpResult12_g77.rgb;
			float cloudAlpha278_g77 = ( borderAlpha169_g77 + nimbusAlpha172_g77 + cumulusAlpha232_g77 + ChemtrailsAlpha61_g77 + CirrusAlpha97_g77 + finalAcAlpha286_g77 + CirrostratusAlpha136_g77 );
			bool enabled20_g80 =(bool)_UnderwaterRenderingEnabled;
			bool submerged20_g80 =(bool)_FullySubmerged;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float textureSample20_g80 = tex2Dlod( _UnderwaterMask, float4( ase_screenPosNorm.xy, 0, 0.0) ).r;
			float localHLSL20_g80 = HLSL20_g80( enabled20_g80 , submerged20_g80 , textureSample20_g80 );
			o.Alpha = ( saturate( ( cloudAlpha278_g77 + ( 0.0 * 2.0 * CZY_CloudThickness ) ) ) * ( 1.0 - localHLSL20_g80 ) );
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
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-695.2959,-681.1561;Float;False;True;-1;2;EmptyShaderGUI;0;0;Unlit;Distant Lands/Cozy/BiRP/Stylized Clouds (Luxury);False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;-50;True;TransparentCutout;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;True;221;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;39;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;1211;-1018.791,-572.5066;Inherit;False;Stylized Clouds (Luxury);0;;77;20d558f0b20b5f34db2c08faef3f114d;0;0;3;COLOR;0;FLOAT;334;FLOAT;333
WireConnection;0;2;1211;0
WireConnection;0;9;1211;334
WireConnection;0;10;1211;333
ASEEND*/
//CHKSM=AE16DD3EFE21F97527DEDE6F18933937F9E1ED03