Shader "Optical/Depth"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

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

	// --- GENERAL DATA ---
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex); // main diffuse texture

	// by unity generated texture that contains information about 
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraDepthTexture);

	float4 _ColorNear; // determines color of near objects
	float4 _ColorFar; // determines color of far objects

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

	// Fragment shader; displays depth value as color
	// WARNING: For this to function properly the clipping planes of the camera must make sense for set-up!
	fixed4 frag(v2f IN) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

		float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, IN.uv));
		float3 lerped = lerp(_ColorNear, _ColorFar, depth); // determining color of pixel based on depth

		return float4(lerped,1);
	}

	ENDCG

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Tags {"RenderType" = "Opaque"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}

	}
}
