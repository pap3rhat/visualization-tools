Shader "Optical/MotionField"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM

			// specifing methods for vertex and fragment shader
			#pragma vertex vert
			#pragma fragment frag

			//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

			// method that converts 2D vector into a color (in HSV color space)
			float3 motionToHSV(float2 motion)
			{
			   float v = 1; // no motion -> white
			   float lengthSquared = motion.x * motion.x + motion.y * motion.y;
			   float s = sqrt(lengthSquared / 2.0); // more motion-> higher saturation of color
			   float aCos = degrees(acos(motion.x / sqrt(lengthSquared)));
			   float h = motion.y > 0 ? aCos : 360 - aCos;

			   float3 hsv;
			   hsv.x = h;
			   hsv.y = s;
			   hsv.z = v;

			   return hsv;
			}

			// method that converts a color from HSV color space to RGB [0,1] color space
			// pseudo-code for convertion from page 304 in "Computer Graphics and Geometric Modeling - Implementation and Algorithms" by Max K. Agoston (2005)
		   float3 hsvToRgb(float3 hsv)
		   {
			   float h = hsv.x;
			   float s = hsv.y;
			   float v = hsv.z;
			   float3 rgb = 0;

			   if (s == 0) {
				   rgb = float3(v, v, v);
			   }
			   else {
				   h = h == 360 ? 0 : (h / 60);
				   float sextant = floor(h);
				   float f = h - sextant;
				   float p = v * (1 - s);
				   float q = v * (1 - s * f);
				   float t = v * (1 - s * (1 - f));

				   if (sextant == 1) {
					   rgb = float3(q, v, p);
				   }
				   else if (sextant == 2) {
					   rgb = float3(p, v, t);
				   }
				   else if (sextant == 3) {
					   rgb = float3(p, q, v);
				   }
				   else if (sextant == 4) {
					   rgb = float3(t, p, v);
				   }
				   else if (sextant == 5) {
					   rgb = float3(v, p, q);
				   }
				   else {
					   rgb = float3(v, t, p);
				   }
			   }
			   
			   return rgb;
		   }
		   //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  

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

		   // vertex shader; converts 3D world-coordinates of vertex into 2D camera-coordinates
		   v2f vert(a2v IN)
		   {
			   v2f OUT;
			   OUT.pos = UnityObjectToClipPos(IN.pos);
			   OUT.uv = IN.uv;
			   return OUT;
		   }
		   //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		   // by unity generated texture that contains information about the 2D movement of each point in clip space between the last frame and the current frame, based on its 3D-movement in the world relative to the camera
		   sampler2D_half _CameraMotionVectorsTexture; 

		   // fragment shader; converts motionVector for each pixel into a color that will be displayed
		   fixed4 frag(v2f IN) : SV_Target
		   {
				  // motion in x-direction is stored in the textures green channel, motion in y-direction is stored in the textures red channel
				  float r = (tex2D(_CameraMotionVectorsTexture, IN.uv).r * 1000); // motions are between -1 and 1, so we need to amplify them here otherwise they will not be visible
				  float g = (tex2D(_CameraMotionVectorsTexture, IN.uv).g * 1000);

				  float2 motion;
				  motion.x = g;
				  motion.y = r;

				  float3 motionHsv = motionToHSV(motion); // converting motion in x and y direction into a color (in HSV color space)
				  float3 motionRGB = hsvToRgb(motionHsv); // converting hsv color to rgb output color

				  return float4(motionRGB,1); 
		   }
		   ENDCG
	   }
	}
}