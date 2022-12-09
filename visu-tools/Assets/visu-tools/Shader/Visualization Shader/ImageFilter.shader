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

		UNITY_VERTEX_INPUT_INSTANCE_ID // single pass instancing support
	};

	// vertex shader to fragment shader
	struct v2f
	{
		float4 pos : SV_POSITION; // position of vertex in camera coordinates (CLIP SPACE POISION)
		float2 uv : TEXCOORD0; // uv coordinate

		UNITY_VERTEX_OUTPUT_STEREO // single pass instancing support
	};

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// --- GENERAL DATA ---
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex); // main diffuse texture
	// by unity generated texture that contains information about the 2D movement of each point in clip space between the last frame and the current frame, based on its 3D-movement in the world relative to the camera
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture);

	float4 _MainTex_TexelSize; // contains texture size information for _MainTex

	int _FinalKernelSize; // size of ther kernel after linear sampling in consideration that first and second half are the same
	StructuredBuffer<float> _Kernel; // kernel values
	StructuredBuffer<float> _Offset; // offset values

	// --- DATA FOR RADIAL BLUR ---
	float _OriginX; // determins x-coordinate of blur origin
	float _OriginY; // determins y-coordiante of blur origin
	float _OriginXLeftEye; // determins x-coordinate of blur origin for left eye
	float _OriginYLeftEye; // determins y-coordiante of blur origin for left eye
	float _OriginXRightEye; // determins x-coordinate of blur origin for right eye
	float _OriginYRightEye; // determins y-coordiante of blur origin for right eye
	float _Scale; // scales the strength of the blur effect

	// --- DATA FOR HIGH-PASS AND SHARPENING ---
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_First); // "original" frame
	float _SharpeningFactor; // determines 'strength' of sharpening

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Vertex shader; converts 3D world-coordinates of vertex into 2D camera coordinates
	v2f vert(a2v IN)
	{
		v2f OUT;

		// single pass instancing support
		UNITY_SETUP_INSTANCE_ID(IN);
		UNITY_INITIALIZE_OUTPUT(v2f, OUT);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

		OUT.pos = UnityObjectToClipPos(IN.pos);
		OUT.uv = IN.uv;
		return OUT;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; determines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on line given by relative position between origin (eye dependent if xr active; else only left eye is used) and pixel
	fixed4 fragR(v2f IN) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
		float eye = unity_StereoEyeIndex; // which eye is curretnly rendered? 0:left, 1:right

		float2 diffVec = (IN.uv - float2(_OriginXLeftEye, _OriginYLeftEye)) * (1 - eye) + (IN.uv - float2(_OriginXRightEye, _OriginYRightEye)) * eye; // vector from origin of blur to current pixel

		float4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.uv) * _Kernel[0]; // init final color

		for (int i = 1; i < _FinalKernelSize; i++)
		{
			float2 offset = _Offset[i] * _MainTex_TexelSize.xy * diffVec * _Scale; // offset of sample point; the farther the point is away from the origin the more it is blurred
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv + offset)) * _Kernel[i];
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv - offset)) * _Kernel[i];
		}

		return col;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	// Fragment shader; determines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on line given by relative position between origin (eye dependent if xr active; else only left eye is used) and pixel; the farther away the more desaturated are the colors
	fixed4 fragRD(v2f IN) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
		float eye = unity_StereoEyeIndex; // which eye is curretnly rendered? 0:left, 1:right

		float2 diffVec = (IN.uv - float2(_OriginXLeftEye, _OriginYLeftEye)) * (1 - eye) + (IN.uv - float2(_OriginXRightEye, _OriginYRightEye)) * eye; // vector from origin of blur to current pixel

		float4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.uv) * _Kernel[0]; // init final color

		for (int i = 1; i < _FinalKernelSize; i++)
		{
			float2 offset = _Offset[i] * _MainTex_TexelSize.xy * diffVec * _Scale; // offset of sample point; the farther the point is away from the origin the more it is blurred
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv + offset)) * _Kernel[i];
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv - offset)) * _Kernel[i];
		}

		// getting maximal distance from blur origin to a pixel
		float2 leftRightOrigin = (float2(_OriginXLeftEye, _OriginYLeftEye) * (1 - eye)) + (float2(_OriginXRightEye, _OriginYRightEye) * eye);
		float maxDist = max(length(float2(0, 0) - leftRightOrigin), length(float2(0, 1) - leftRightOrigin));
		maxDist = max(maxDist, length(float2(1, 0) - leftRightOrigin));
		maxDist = max(maxDist, length(float2(1, 1) - leftRightOrigin));


		// linear interpolation between 'normal' color and grayscale color
		float scale = (maxDist - (maxDist - length(diffVec))) / maxDist; // the farther away from origin a pixel is the more desaturated (gray) it will be
		float3 lerped = lerp(col.xyz, intensity(col).xxx, scale);

		return float4(lerped, 1);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; determines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on horizontal line through pixel
	fixed4 fragH(v2f IN) : SV_Target
	{
		float4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.uv) * _Kernel[0];
		for (int i = 1; i < _FinalKernelSize; i++)
		{
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv + float2(_Offset[i] * _MainTex_TexelSize.x,0))) * _Kernel[i];
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv - float2(_Offset[i] * _MainTex_TexelSize.x, 0))) * _Kernel[i];
		}
		return col;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; detemines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on vertical line through pixel
	fixed4 fragV(v2f IN) : SV_Target
	{
		float4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.uv) * _Kernel[0];
		for (int i = 1; i < _FinalKernelSize; i++)
		{
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv + float2(0, _Offset[i] * _MainTex_TexelSize.y))) * _Kernel[i];
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv - float2(0, _Offset[i] * _MainTex_TexelSize.y))) * _Kernel[i];
		}
		return col;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; determines final color for each pixel by subtracting blurred frame from "original" frame (realizes gaussian high-spass filter)
	fixed4 fragHPF(v2f IN) : SV_Target
	{
		return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_First, IN.uv) - UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.uv);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; determines final color for each pixel by subtracting blurred frame from "original" frame, weighting it and then adding it back to the original image (realizes image sharpening)
	fixed4 fragSH(v2f IN) : SV_Target
	{
		return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_First, IN.uv) + _SharpeningFactor * (UNITY_SAMPLE_SCREENSPACE_TEXTURE(_First, IN.uv) - UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.uv));
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Fragment shader; determines color for each pixel by looking at its own color as well as the neighboring pixels color and weighting them according to the kernel values
	// here: neighbors on line given by motion vector of pixel
	fixed4 fragM(v2f IN) : SV_Target
	{
		float2 diffVec = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture, IN.uv).rg; // getting motion vector
		diffVec *= unity_DeltaTime.y; // scaling vector so it's visible; making it frame-rate independent

		float4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.uv) * _Kernel[0]; // init final color

		for (int i = 1; i < _FinalKernelSize; i++)
		{
			float2 offset = _Offset[i] * _MainTex_TexelSize.xy * diffVec * _Scale; // offset of sample point; the farther the point is away from the origin the more it is blurred
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv + offset)) * _Kernel[i];
			col += UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, saturate(IN.uv - offset)) * _Kernel[i];
		}

		return col;
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
			#pragma multi_compile_instancing

			ENDCG
		}

		// 1: radial gaussian blur with desaturation
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragRD
			#pragma multi_compile_instancing

			ENDCG
		}

		// 2: horizontal seperable gaussian blur
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragH
			#pragma multi_compile_instancing

			ENDCG
		}

		// 3: vertical seperable guassian blur
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma multi_compile_instancing

			ENDCG
		}

		// 4: high-pass filter
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragHPF
			#pragma multi_compile_instancing

			ENDCG
		}

		// 5: image sharpening
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragSH
			#pragma multi_compile_instancing

			ENDCG
		}

		// 6: motion blur
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragM
			#pragma multi_compile_instancing

			ENDCG
		}
	}
}