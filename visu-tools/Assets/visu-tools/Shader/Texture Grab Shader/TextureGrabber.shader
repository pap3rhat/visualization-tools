Shader "Optical/TextureGrabber"
{
	CGINCLUDE
	#include "UnityCG.cginc"

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

	// by unity generated texture that contains information about the 2D movement of each point in clip space between the last frame and the current frame, based on its 3D-movement in the world relative to the camera
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture);

	// by unity generated texture that contains information about 
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraDepthTexture);

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

	// fragment shader; just returns motion vector texture scaled by time.DeltaTime^-1
	fixed4 fragM(v2f IN) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

		return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture, IN.uv) * unity_DeltaTime.y;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// fragment shader; just returns depth texture made linear
	fixed4 fragD(v2f IN) : SV_Target
	{	 
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

		return Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, IN.uv));

	}
	
	ENDCG

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Tags{ "RenderType" = "Opaque" }

		// 0: returns motion vectors
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragM
			#pragma multi_compile_instancing
			ENDCG
		}

		// 1: returns depth information
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragD
			#pragma multi_compile_instancing
			ENDCG
		}
	}
}
