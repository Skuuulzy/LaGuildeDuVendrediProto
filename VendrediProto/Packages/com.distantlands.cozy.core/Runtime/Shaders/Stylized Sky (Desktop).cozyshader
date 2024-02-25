// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Distant Lands/Cozy/BiRP/Stylized Sky (Desktop)"
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
		uniform float CZY_SunHaloFalloff;
		uniform float4 CZY_SunHaloColor;
		uniform float4 CZY_SunFilterColor;
		uniform float4 CZY_SunColor;
		uniform float CZY_SunSize;
		uniform float3 CZY_EclipseDirection;
		uniform float3 CZY_MoonDirection;
		uniform float CZY_MoonFlareFalloff;
		uniform float4 CZY_MoonFlareColor;
		uniform sampler2D CZY_GalaxyVariationMap;
		uniform sampler2D CZY_StarMap;
		uniform sampler2D CZY_GalaxyMap;
		uniform sampler2D CZY_GalaxyStarMap;
		uniform float4 CZY_StarColor;
		uniform float4 CZY_GalaxyColor1;
		uniform float4 CZY_GalaxyColor2;
		uniform float4 CZY_GalaxyColor3;
		uniform float CZY_GalaxyMultiplier;
		uniform float CZY_RainbowSize;
		uniform float CZY_RainbowWidth;
		uniform float CZY_RainbowIntensity;
		uniform sampler2D CZY_LightScatteringMap;
		uniform float4 CZY_LightColumnColor;


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


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1));
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1));
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
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
			float4 HorizonColor140_g5 = temp_output_10_0_g6;
			float3 hsvTorgb2_g7 = RGBToHSV( CZY_ZenithColor.rgb );
			float3 hsvTorgb3_g7 = HSVToRGB( float3(hsvTorgb2_g7.x,saturate( ( hsvTorgb2_g7.y + CZY_FilterSaturation ) ),( hsvTorgb2_g7.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g7 = ( float4( hsvTorgb3_g7 , 0.0 ) * CZY_FilterColor );
			float4 ZenithColor139_g5 = temp_output_10_0_g7;
			float2 temp_output_136_0_g5 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult138_g5 = dot( temp_output_136_0_g5 , temp_output_136_0_g5 );
			float SimpleGradient137_g5 = dotResult138_g5;
			float GradientPos132_g5 = ( 1.0 - saturate( pow( saturate( (0.0 + (SimpleGradient137_g5 - 0.0) * (2.0 - 0.0) / (1.0 - 0.0)) ) , CZY_Power ) ) );
			float4 lerpResult110_g5 = lerp( HorizonColor140_g5 , ZenithColor139_g5 , GradientPos132_g5);
			float4 SimpleSkyGradient127_g5 = lerpResult110_g5;
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult62_g5 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult66_g5 = dot( normalizeResult62_g5 , CZY_SunDirection );
			float SunDot72_g5 = dotResult66_g5;
			float3 hsvTorgb2_g9 = RGBToHSV( CZY_SunHaloColor.rgb );
			float3 hsvTorgb3_g9 = HSVToRGB( float3(hsvTorgb2_g9.x,saturate( ( hsvTorgb2_g9.y + CZY_FilterSaturation ) ),( hsvTorgb2_g9.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g9 = ( float4( hsvTorgb3_g9 , 0.0 ) * CZY_FilterColor );
			half4 SunFlare143_g5 = abs( ( saturate( pow( saturate( (SunDot72_g5*0.5 + 0.4) ) , ( ( CZY_SunHaloFalloff * 40.0 ) + 5.0 ) ) ) * ( temp_output_10_0_g9 * CZY_SunFilterColor ) ) );
			float3 normalizeResult233_g5 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult234_g5 = dot( normalizeResult233_g5 , CZY_EclipseDirection );
			float EclipseDot237_g5 = dotResult234_g5;
			float eclipse208_g5 = ( ( 1.0 - EclipseDot237_g5 ) > ( pow( CZY_SunSize , 3.0 ) * 0.0006 ) ? 0.0 : 1.0 );
			float4 SunRender219_g5 = ( CZY_SunColor * saturate( ( ( ( 1.0 - SunDot72_g5 ) > ( pow( CZY_SunSize , 3.0 ) * 0.0007 ) ? 0.0 : 1.0 ) - eclipse208_g5 ) ) );
			float3 normalizeResult227_g5 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult228_g5 = dot( normalizeResult227_g5 , CZY_MoonDirection );
			float MoonDot240_g5 = dotResult228_g5;
			float3 hsvTorgb2_g8 = RGBToHSV( CZY_MoonFlareColor.rgb );
			float3 hsvTorgb3_g8 = HSVToRGB( float3(hsvTorgb2_g8.x,saturate( ( hsvTorgb2_g8.y + CZY_FilterSaturation ) ),( hsvTorgb2_g8.z + CZY_FilterValue )) );
			float4 temp_output_10_0_g8 = ( float4( hsvTorgb3_g8 , 0.0 ) * CZY_FilterColor );
			half4 MoonFlare53_g5 = abs( ( saturate( pow( saturate( (MoonDot240_g5*0.5 + 0.4) ) , ( ( CZY_MoonFlareFalloff * 20.0 ) + 5.0 ) ) ) * temp_output_10_0_g8 ) );
			float2 Pos17_g5 = i.uv_texcoord;
			float mulTime41_g5 = _Time.y * 0.005;
			float cos18_g5 = cos( mulTime41_g5 );
			float sin18_g5 = sin( mulTime41_g5 );
			float2 rotator18_g5 = mul( Pos17_g5 - float2( 0.5,0.5 ) , float2x2( cos18_g5 , -sin18_g5 , sin18_g5 , cos18_g5 )) + float2( 0.5,0.5 );
			float mulTime36_g5 = _Time.y * -0.02;
			float simplePerlin2D19_g5 = snoise( (Pos17_g5*5.0 + mulTime36_g5) );
			simplePerlin2D19_g5 = simplePerlin2D19_g5*0.5 + 0.5;
			float StarPlacementPattern171_g5 = saturate( ( min( tex2D( CZY_GalaxyVariationMap, (Pos17_g5*5.0 + mulTime41_g5) ).r , tex2D( CZY_GalaxyVariationMap, (rotator18_g5*2.0 + 0.0) ).r ) * simplePerlin2D19_g5 * (0.2 + (SimpleGradient137_g5 - 0.0) * (0.0 - 0.2) / (1.0 - 0.0)) ) );
			float2 panner45_g5 = ( 1.0 * _Time.y * float2( 0.0007,0 ) + Pos17_g5);
			float mulTime75_g5 = _Time.y * 0.001;
			float cos57_g5 = cos( mulTime75_g5 );
			float sin57_g5 = sin( mulTime75_g5 );
			float2 rotator57_g5 = mul( Pos17_g5 - float2( 0.5,0.5 ) , float2x2( cos57_g5 , -sin57_g5 , sin57_g5 , cos57_g5 )) + float2( 0.5,0.5 );
			float temp_output_60_0_g5 = min( tex2D( CZY_StarMap, (panner45_g5*4.0 + mulTime75_g5) ).r , tex2D( CZY_GalaxyVariationMap, (rotator57_g5*0.1 + 0.0) ).r );
			float2 panner9_g5 = ( 1.0 * _Time.y * float2( 0.0007,0 ) + Pos17_g5);
			float mulTime13_g5 = _Time.y * 0.005;
			float2 panner14_g5 = ( 1.0 * _Time.y * float2( 0.001,0 ) + Pos17_g5);
			float mulTime11_g5 = _Time.y * 0.005;
			float cos10_g5 = cos( mulTime11_g5 );
			float sin10_g5 = sin( mulTime11_g5 );
			float2 rotator10_g5 = mul( Pos17_g5 - float2( 0.5,0.5 ) , float2x2( cos10_g5 , -sin10_g5 , sin10_g5 , cos10_g5 )) + float2( 0.5,0.5 );
			float2 panner16_g5 = ( mulTime11_g5 * float2( 0.004,0 ) + rotator10_g5);
			float2 GalaxyPos63_g5 = panner16_g5;
			float GalaxyPattern192_g5 = saturate( ( min( (0.3 + (tex2D( CZY_GalaxyVariationMap, (panner9_g5*4.0 + mulTime13_g5) ).r - 0.0) * (1.0 - 0.3) / (0.8 - 0.0)) , (0.3 + (( 1.0 - tex2D( CZY_GalaxyVariationMap, (panner14_g5*3.0 + mulTime13_g5) ).r ) - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) ) * (0.3 + (SimpleGradient137_g5 - 0.0) * (-0.2 - 0.3) / (0.2 - 0.0)) * tex2D( CZY_GalaxyMap, GalaxyPos63_g5 ).r ) );
			float4 break80_g5 = MoonFlare53_g5;
			float StarPattern90_g5 = ( ( ( StarPlacementPattern171_g5 * temp_output_60_0_g5 ) + ( temp_output_60_0_g5 * GalaxyPattern192_g5 ) + ( tex2D( CZY_GalaxyStarMap, GalaxyPos63_g5 ).r * 0.2 ) ) * ( 1.0 - ( break80_g5.r + break80_g5.g + break80_g5.b + break80_g5.a ) ) );
			float cos200_g5 = cos( 0.002 * _Time.y );
			float sin200_g5 = sin( 0.002 * _Time.y );
			float2 rotator200_g5 = mul( Pos17_g5 - float2( 0.5,0.5 ) , float2x2( cos200_g5 , -sin200_g5 , sin200_g5 , cos200_g5 )) + float2( 0.5,0.5 );
			float cos199_g5 = cos( 0.004 * _Time.y );
			float sin199_g5 = sin( 0.004 * _Time.y );
			float2 rotator199_g5 = mul( Pos17_g5 - float2( 0.5,0.5 ) , float2x2( cos199_g5 , -sin199_g5 , sin199_g5 , cos199_g5 )) + float2( 0.5,0.5 );
			float cos201_g5 = cos( 0.001 * _Time.y );
			float sin201_g5 = sin( 0.001 * _Time.y );
			float2 rotator201_g5 = mul( Pos17_g5 - float2( 0.5,0.5 ) , float2x2( cos201_g5 , -sin201_g5 , sin201_g5 , cos201_g5 )) + float2( 0.5,0.5 );
			float4 appendResult43_g5 = (float4(tex2D( CZY_GalaxyVariationMap, (rotator200_g5*10.0 + 0.0) ).r , tex2D( CZY_GalaxyVariationMap, (rotator199_g5*8.0 + 2.04) ).r , tex2D( CZY_GalaxyVariationMap, (rotator201_g5*6.0 + 2.04) ).r , 1.0));
			float4 GalaxyColoring69_g5 = appendResult43_g5;
			float4 break124_g5 = GalaxyColoring69_g5;
			float4 FinalGalaxyColoring189_g5 = ( ( CZY_GalaxyColor1 * break124_g5.r ) + ( CZY_GalaxyColor2 * break124_g5.g ) + ( CZY_GalaxyColor3 * break124_g5.b ) );
			float4 GalaxyFullColor120_g5 = ( saturate( ( StarPattern90_g5 * CZY_StarColor ) ) + ( GalaxyPattern192_g5 * FinalGalaxyColoring189_g5 * CZY_GalaxyMultiplier ) );
			Gradient gradient95_g5 = NewGradient( 0, 8, 4, float4( 1, 0, 0, 0.1205921 ), float4( 1, 0.3135593, 0, 0.2441138 ), float4( 1, 0.8774895, 0.2216981, 0.3529412 ), float4( 0.3030533, 1, 0.2877358, 0.4529488 ), float4( 0.3726415, 1, 0.9559749, 0.5529412 ), float4( 0.4669811, 0.7253776, 1, 0.6470588 ), float4( 0.1561944, 0.3586135, 0.735849, 0.802945 ), float4( 0.2576377, 0.08721964, 0.5283019, 0.9264668 ), float2( 0, 0 ), float2( 0, 0.08235294 ), float2( 0.6039216, 0.8264744 ), float2( 0, 1 ), 0, 0, 0, 0 );
			float temp_output_86_0_g5 = ( 1.0 - SunDot72_g5 );
			float temp_output_91_0_g5 = ( CZY_RainbowSize * 0.01 );
			float temp_output_96_0_g5 = ( temp_output_91_0_g5 + ( CZY_RainbowWidth * 0.01 ) );
			float4 RainbowClipping112_g5 = ( SampleGradient( gradient95_g5, (0.0 + (temp_output_86_0_g5 - temp_output_91_0_g5) * (1.0 - 0.0) / (temp_output_96_0_g5 - temp_output_91_0_g5)) ) * ( ( temp_output_86_0_g5 < temp_output_91_0_g5 ? 0.0 : 1.0 ) * ( temp_output_86_0_g5 > temp_output_96_0_g5 ? 0.0 : 1.0 ) ) * SampleGradient( gradient95_g5, (0.0 + (temp_output_86_0_g5 - temp_output_91_0_g5) * (1.0 - 0.0) / (temp_output_96_0_g5 - temp_output_91_0_g5)) ).a * CZY_RainbowIntensity );
			float cos70_g5 = cos( -0.005 * _Time.y );
			float sin70_g5 = sin( -0.005 * _Time.y );
			float2 rotator70_g5 = mul( Pos17_g5 - float2( 0.5,0.5 ) , float2x2( cos70_g5 , -sin70_g5 , sin70_g5 , cos70_g5 )) + float2( 0.5,0.5 );
			float cos67_g5 = cos( 0.01 * _Time.y );
			float sin67_g5 = sin( 0.01 * _Time.y );
			float2 rotator67_g5 = mul( Pos17_g5 - float2( 0.5,0.5 ) , float2x2( cos67_g5 , -sin67_g5 , sin67_g5 , cos67_g5 )) + float2( 0.5,0.5 );
			float4 transform185_g5 = mul(unity_WorldToObject,float4( ase_worldPos , 0.0 ));
			float saferPower187_g5 = abs( ( ( abs( transform185_g5.y ) * 0.03 ) + -0.3 ) );
			float LightColumnsPattern93_g5 = saturate( ( min( tex2D( CZY_LightScatteringMap, rotator70_g5 ).r , tex2D( CZY_LightScatteringMap, rotator67_g5 ).r ) * (1.0 + (saturate( pow( saferPower187_g5 , 1.0 ) ) - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) );
			float4 LightColumnsColor114_g5 = ( LightColumnsPattern93_g5 * CZY_LightColumnColor );
			o.Emission = ( SimpleSkyGradient127_g5 + SunFlare143_g5 + SunRender219_g5 + MoonFlare53_g5 + GalaxyFullColor120_g5 + RainbowClipping112_g5 + LightColumnsColor114_g5 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "EmptyShaderGUI"
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;104.1543,85.88161;Float;False;True;-1;2;EmptyShaderGUI;0;0;Unlit;Distant Lands/Cozy/BiRP/Stylized Sky (Desktop);False;False;False;False;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;False;False;Front;0;False;;7;False;;False;0;False;;0;False;;True;0;Translucent;0.5;True;False;-100;False;Opaque;;Transparent;All;12;all;True;True;True;True;0;False;;True;220;False;;255;False;;255;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;598;-192,112;Inherit;False;Stylized Sky (Desktop);0;;5;6fc9715951ffc7d4dae1a16a0961dc28;0;0;2;COLOR;0;FLOAT;245
WireConnection;0;2;598;0
ASEEND*/
//CHKSM=5E4624E25381B8F201931A5C3BE562F5E1F7CB5F