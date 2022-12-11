Shader "Optical/MotionField"
{
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Assets/visu-tools/Shader/Visualization Shader/ColorMethods.cginc"

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

	float _Scale; // used to scale motionVectors, because they can be very tiny

	// used when motion field is applied after high-pass filter
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_Filtered); // texture after high-pass filter got applied
	float _Threshold; // determines how high intensity has to be in order to show up on the final image

	// by unity generated texture that contains information about the 2D movement of each point in clip space between the last frame and the current frame, based on its 3D-movement in the world relative to the camera
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture);

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	
	// vertex shader; converts 3D world-coordinates of vertex into 2D camera-coordinates
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

	// fragment shader; converts motionVector for each pixel into a color that will be displayed
	fixed4 frag(v2f IN) : SV_Target
	{
		// motion in x-direction is stored in the textures red channel, motion in y-direction is stored in the textures green channel
		float2 motion;
		motion.x = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture, IN.uv).r;
		motion.y = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture, IN.uv).g;

		motion = motion * unity_DeltaTime.y * _Scale; // scaling vector so it's visible; making it frame-rate independent

		float3 motionHsv = motionToHSV(motion); // converting motion in x and y direction into a color (in HSV color space)
		float3 motionRGB = hsvToRgb(motionHsv); // converting hsv color to rgb output color

		return float4(motionRGB,1);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// fragment shader; converts motionVector for each pixel into a color; will be displayed where high-pass filter found edges(high image frequencies)
	fixed4 fragAHP(v2f IN) : SV_Target
	{
		// motion in x-direction is stored in the textures red channel, motion in y-direction is stored in the textures green channel
		float2 motion;
		motion.x = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture, IN.uv).r;
		motion.y = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture, IN.uv).g;

		motion = motion * unity_DeltaTime.y * _Scale; // scaling vector so it's visible; making it frame-rate independent

		float3 motionHsv = motionToHSV(motion); // converting motion in x and y direction into a color (in HSV color space)
		float3 motionRGB = hsvToRgb(motionHsv); // converting hsv color to rgb output color

		// convert current pixel into intensity value (grayscale value)
		float intent = intensity(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_Filtered, IN.uv));

		// if intensity is high enough (high frequency in image got detected) show motion field colors; otherwise just show black
		if (intent >= _Threshold) 
		{
			return float4(motionRGB, 1);
		}
		else
		{
			return float4(0, 0, 0, 1);
		}
	}

	ENDCG

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	SubShader
	{
		// 0: just displays motion field with colors
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			ENDCG
		}

			// 1: display motion field with colors where high pass filter found edges
			Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragAHP
			#pragma multi_compile_instancing
			ENDCG
		}
	}
}
