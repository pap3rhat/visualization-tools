using UnityEngine;
using UnityEngine.XR;

/* Script that co-ordinates all available shader (combinations).
 * This is the script that needs to be attached to camera that is supposed to use the shaders. 
 */
[RequireComponent(typeof(Camera))]
public class ControlShader : MonoBehaviour
{
    // setup for shader
    [Tooltip("Move camera to which this script is attached in here.")] [SerializeField] private Camera cam;
    [Tooltip("Move material named 'MotionField' in here.")] [SerializeField] private Material motionFieldMaterial;
    [Tooltip("Move material named 'ImageFilter' in here.")] [SerializeField] private Material imageFilterMaterial;
    [Tooltip("Move material named 'Depth' in here.")] [SerializeField] private Material depthMaterial;

    // necessary information for all shaders containing the motion field
    private MotionField motionScript;
    [Tooltip("How intense needs a pixel value to be after high-pass filtering in order to render motion field for it. If intensity is below threshold, pixel will show up black.")]
    [Range(0f, 0.25f)] [SerializeField] private float threshold;

    // necessary information for all shaders containing some kind of filtering (radial blur, guassian blur, high-pass filter, image sharpening)
    private ImageFilter imageFilterScript;
    [Tooltip("Size of the gaussian kernel used for image filtering. The bigger the stronger the effect.")] [Range(5, 127)] [SerializeField] private int kernelSize;
    [Tooltip("Offset of origin of radial blur effect.")] [Range(0f, 1f)] [SerializeField] private float radialBlurOriginX, radialBlurOriginY;
    [Tooltip("Strength of radial blur effect.")] [Range(0f, 10f)] [SerializeField] private float scale;

    // necessary information for all shaders containing the depth
    private Depth depthScript;
    [Tooltip("Color of the objects close to the camera.")] [ColorUsage(true)] [SerializeField] private Color colorNear;
    [Tooltip("Color of the objects farthest from the camera.")] [ColorUsage(true)] [SerializeField] private Color colorFar;

    public enum ShaderActive
    {
        None = -1,
        RadialBlur = 0,
        RadialBlurDesat = 1,
        GaussianBlur = 2,
        Sharpening = 3,
        HighPass = 4,
        Motion = 5,
        HighPassOnMotion = 6,
        MotionOnHighPass = 7,
        Depth = 8,
        HighPassOnDepth = 9,
        MotionOnHighPassOnDepth = 10
    }

    [Tooltip("Which image effect should be applied.")] public ShaderActive shaderActive;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Method that is being called when the script instance is being loaded */
    private void Awake()
    {
        // setting active shader to "None" as a default
        shaderActive = ShaderActive.None;

        // setting up camera
        cam = GetComponent<Camera>(); // getting camera object to which this script is attached
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth | DepthTextureMode.MotionVectors; // activiating option to generate motion vectors 

        // set-up for all shaders regarding the motion field
        motionScript = new MotionField();
        threshold = motionScript.Threshold;

        // set-up for all shaders containing some kind of "basic" filtering
        imageFilterScript = new ImageFilter();
        imageFilterScript.KernelSetUp(imageFilterMaterial);
        kernelSize = imageFilterScript.KernelSize;
        radialBlurOriginX = imageFilterScript.OriginX;
        radialBlurOriginY = imageFilterScript.OriginY;
        scale = imageFilterScript.Scale;

        // set-up for all shaders regarding the depth
        depthScript = new Depth();
        colorNear = depthScript.ColorNear;
        colorFar = depthScript.ColorFar;
    }

    /* Method that is being called every frame */
    private void Update()
    {
        // checking if kernel for basic image filtering needs to be updated
        if (kernelSize != imageFilterScript.KernelSize && kernelSize % 2 != 0 && kernelSize >= ImageFilter.MIN_KERNEL_SIZE)
        {
            imageFilterScript.KernelSize = kernelSize;
            imageFilterScript.KernelSetUp(imageFilterMaterial);
        }

        // checking if origin for radial blur changed
        if (radialBlurOriginX != imageFilterScript.OriginX && radialBlurOriginX >= 0 && radialBlurOriginX <= 1)
        {
            imageFilterScript.OriginX = radialBlurOriginX;
        }
        if (radialBlurOriginY != imageFilterScript.OriginY && radialBlurOriginY >= 0 && radialBlurOriginY <= 1)
        {
            imageFilterScript.OriginY = radialBlurOriginY;
        }

        // cehcking if scale for radial blurred changed
        if (scale != imageFilterScript.Scale && scale >= 0 && scale <= 100)
        {
            imageFilterScript.Scale = scale;
        }

        // checking if threshold for motion field on high-pass needs to be updated
        if (threshold != motionScript.Threshold && threshold >= 0 & threshold <= 1)
        {
            motionScript.Threshold = threshold;
        }

        // checking if color changed
        if (colorNear != depthScript.ColorNear)
        {
            depthScript.ColorNear = colorNear;
        }
        if (colorFar != depthScript.ColorFar)
        {
            depthScript.ColorFar = colorFar;
        }
    }

    /* Method that is being called after camera has finished rendering; here we can modify the final image the user will see */
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // crucial part for single pass instanced rendering; taken from: https://forum.unity.com/threads/creating-image-effects-for-singlepass-stereo-rendering-with-multiple-shader-passes.710609/
        RenderTextureDescriptor desc;
        if (XRSettings.enabled)
            desc = XRSettings.eyeTextureDesc;
        else
            desc = new RenderTextureDescriptor(Screen.width, Screen.height); // Not XR

        switch (shaderActive)
        {
            case ShaderActive.None: // nothing is applied, image stays the same
                Graphics.Blit(source, destination);
                break;
            case ShaderActive.RadialBlur: // display a radial blur
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.Radial;
                imageFilterScript.RenderShader(source, destination, imageFilterMaterial);
                break;
            case ShaderActive.RadialBlurDesat: // display a radial blur with desaturation
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.RadialDesat;
                imageFilterScript.RenderShader(source, destination, imageFilterMaterial);
                break;
            case ShaderActive.GaussianBlur: // blur (low-pass filter) the image (remove high frequencies)
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.Blur;
                imageFilterScript.RenderShader(source, destination, imageFilterMaterial);
                break;
            case ShaderActive.Sharpening: // sharpen the image (add high image frequencies on top of image)
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.Sharpening;
                imageFilterScript.RenderShader(source, destination, imageFilterMaterial);
                break;
            case ShaderActive.HighPass: // high-pass filter the image (remove low frequencies)
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.HighPass;
                imageFilterScript.RenderShader(source, destination, imageFilterMaterial);
                break;
            case ShaderActive.Motion: // display motion field as colors
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.OnlyMotion;
                motionScript.RenderShader(source, destination, motionFieldMaterial);
                break;
            case ShaderActive.HighPassOnMotion: // first display motion field as colors and then extract only the high frquencies from it
                var tmp = RenderTexture.GetTemporary(desc);
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.OnlyMotion;
                motionScript.RenderShader(source, tmp, motionFieldMaterial);
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.HighPass;
                imageFilterScript.RenderShader(tmp, destination, imageFilterMaterial);
                RenderTexture.ReleaseTemporary(tmp);
                break;
            case ShaderActive.MotionOnHighPass: // first extract high frquencies from image and then apply motion field colors on it
                var tmp2 = RenderTexture.GetTemporary(desc);
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.HighPass;
                imageFilterScript.RenderShader(source, tmp2, imageFilterMaterial);
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.MotionOnHighPass;
                motionScript.RenderShader(tmp2, destination, motionFieldMaterial);
                RenderTexture.ReleaseTemporary(tmp2);
                break;
            case ShaderActive.Depth: // visualizing depth 
                depthScript.RenderShader(source, destination, depthMaterial);
                break;
            case ShaderActive.HighPassOnDepth: // high-pass filter on depth 
                var tmp3 = RenderTexture.GetTemporary(desc);
                depthScript.RenderShader(source, tmp3, depthMaterial);
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.HighPass;
                imageFilterScript.RenderShader(tmp3, destination, imageFilterMaterial);
                RenderTexture.ReleaseTemporary(tmp3);
                break;
            case ShaderActive.MotionOnHighPassOnDepth: // motion field on high-pass filter on depth 
                var tmp4 = RenderTexture.GetTemporary(desc);
                var tmp5 = RenderTexture.GetTemporary(desc);
                depthScript.RenderShader(source, tmp4, depthMaterial);
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.HighPass;
                imageFilterScript.RenderShader(tmp4, tmp5, imageFilterMaterial);
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.MotionOnHighPass;
                motionScript.RenderShader(tmp5, destination, motionFieldMaterial);
                RenderTexture.ReleaseTemporary(tmp4);
                RenderTexture.ReleaseTemporary(tmp5);
                break;
        }
    }

    /* Method that is being called when scene or game ends */
    private void OnDestroy()
    {
        imageFilterScript.CleanUp();
    }
}
