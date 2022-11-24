Shader "Optical/ImageFilter"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Assets/visu-tools/Shader/ColorMethods.cginc"

	// appdata to vertex shader
	struct a2v
	{
		float4 pos : POSITION; // position of vertex in world coordinates (WORLD POSITION)
		float2 uv : TEXCOORD0; // uv coordinate
	};

	// vertex shader to fragment shader
	struct v2f
	{
		float4 pos : SV_POSITION; // position of vertex in camera coordinates (CLIP SPACE POISION)
		float2 uv : TEXCOORD0; // uv coordinate
	};

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// --- GENERAL DATA ---
	sampler2D _MainTex; // main diffuse texture

	float4 _MainTex_TexelSize; // contains texture size information for _MainTex

	int _FinalKernelSize; // size of ther kernel after linear sampling in consideration that first and second half are the same
	StructuredBuffer<float> _Kernel; // kernel values
	StructuredBuffer<float> _Offset; // offset values

	// --- DATA FOR RADIAL BLUR ---
	float _OriginX; // determins x-coordinate of blur origin
	float _OriginY; // determins y-coordiante of blur origin
	float _Scale; // scales the strength of the blur effect

	// --- DATA FOR HIGH-PASS AND SHARPENING ---
	sampler2D _First; // "original" frame
	float _SharpeningFactor; // determines 'strength' of sharpening

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Vertex shader; converts 3D world-coordinates of vertex into 2D camera coordinates
	v2f vert(a2v IN)
	{
		v2f OUT;
		OUT.pos = UnityObjectToClipPos(IN.pos);
		OUT.uv = IN.uv;
		return OUT;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; detemines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on line given by relative position between origin and pixel
	fixed4 fragR(v2f IN) : SV_Target
	{
		float2 diffVec = IN.uv - float2(_OriginX, _OriginY); // vector from origin of blur to current pixel

		float4 col = tex2D(_MainTex, IN.uv) * _Kernel[0]; // init final color

		for (int i = 1; i < _FinalKernelSize; i++)
		{
			float2 offset = _Offset[i] * _MainTex_TexelSize.xy * diffVec * _Scale; // offset of sample point; the farther the point is away from the origin the more it is blurred
			col += tex2D(_MainTex, saturate(IN.uv + offset)) * _Kernel[i];
			col += tex2D(_MainTex, saturate(IN.uv - offset)) * _Kernel[i];
		}
		
		return col;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	// Fragment shader; detemines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on line given by relative position between origin and pixel; the farther away the more desaturated are the colors
	fixed4 fragRD(v2f IN) : SV_Target
	{
		float2 diffVec = IN.uv - float2(_OriginX, _OriginY); // vector from origin of blur to current pixel

		float4 col = tex2D(_MainTex, IN.uv) * _Kernel[0]; // init final color

		for (int i = 1; i < _FinalKernelSize; i++)
		{
			float2 offset = _Offset[i] * _MainTex_TexelSize.xy * diffVec * _Scale; // offset of sample point; the farther the point is away from the origin the more it is blurred
			col += tex2D(_MainTex, saturate(IN.uv + offset)) * _Kernel[i];
			col += tex2D(_MainTex, saturate(IN.uv - offset)) * _Kernel[i];
		}

		// getting maximal distance from blur origin to a pixel
		float maxDist = max(length(float2(0, 0) - float2(_OriginX, _OriginY)), length(float2(0, 1) - float2(_OriginX, _OriginY)));
		maxDist = max(maxDist, length(float2(1, 0) - float2(_OriginX, _OriginY)));
		maxDist = max(maxDist, length(float2(1, 1) - float2(_OriginX, _OriginY)));

	
		// linear interpolation between 'normal' color and grayscale color
		float scale = (maxDist - (maxDist - length(diffVec)))/maxDist; // the farther away from origin a pixel is the more desaturated (gray) it will be
		float3 lerped = lerp(col.xyz, intensity(col).xxx, scale);
		
		return float4(lerped, 1);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; detemines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on horizontal line through pixel
	fixed4 fragH(v2f IN) : SV_Target
	{
		float4 col = tex2D(_MainTex, IN.uv) * _Kernel[0];
		for (int i = 1; i < _FinalKernelSize; i++)
		{
			col += tex2D(_MainTex, saturate(IN.uv + float2(_Offset[i] * _MainTex_TexelSize.x,0))) * _Kernel[i];
			col += tex2D(_MainTex, saturate(IN.uv - float2(_Offset[i] * _MainTex_TexelSize.x, 0))) * _Kernel[i];
		}
		return col;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; detemines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on vertical line through pixel
	fixed4 fragV(v2f IN) : SV_Target
	{
		float4 col = tex2D(_MainTex, IN.uv) * _Kernel[0];
		for (int i = 1; i < _FinalKernelSize; i++)
		{
			col += tex2D(_MainTex, saturate(IN.uv + float2(0, _Offset[i] * _MainTex_TexelSize.y))) * _Kernel[i];
			col += tex2D(_MainTex, saturate(IN.uv - float2(0, _Offset[i] * _MainTex_TexelSize.y))) * _Kernel[i];
		}
		return col;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; detemines final color for each pixel by subtracting blurred frame from "original" frame (realizes gaussian high-spass filter)
	fixed4 fragHPF(v2f IN) : SV_Target
	{
		return tex2D(_First, IN.uv) - tex2D(_MainTex, IN.uv);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; detemines final color for each pixel by subtracting blurred frame from "original" frame, weighting it and then adding it back to the original image (realizes image sharpening)
	fixed4 fragSH(v2f IN) : SV_Target
	{
		return tex2D(_First, IN.uv) + _SharpeningFactor * (tex2D(_First, IN.uv) - tex2D(_MainTex, IN.uv));
	}

	ENDCG

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		// 0: radial gaussian blur
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragR

			ENDCG
		}

		// 1: radial gaussian blur with desaturation
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragRD

			ENDCG
		}

		// 2: horizontal seperable gaussian blur
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragH

			ENDCG
		}

		// 3: vertical seperable guassian blur
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV

			ENDCG
		}

		// 4: high-pass filter
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragHPF

			ENDCG
		}

		// 5: image sharpening
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragSH

			ENDCG
		}
	}
}
