// method that converts 2D vector into a color (in HSV color space)
float3 motionToHSV(float2 motion)
{
	float v = 1; // no motion -> white
	float lengthSquared = motion.x * motion.x + motion.y * motion.y;
	float s = sqrt(lengthSquared / 2.0); // more motion-> higher saturation of color
	float aCos = degrees(acos(clamp(motion.x / sqrt(lengthSquared), -1, 1)));
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

// method that converts a color into a grayscale value
// formula is the same as OpenCV uses: https://docs.opencv.org/2.4/modules/imgproc/doc/miscellaneous_transformations.html#void%20cvtColor%28InputArray%20src,%20OutputArray%20dst,%20int%20code,%20int%20dstCn%29
float intensity(float4 col)
{
	return 0.299 * col.r + 0.587 * col.g + 0.114 * col.b;
}