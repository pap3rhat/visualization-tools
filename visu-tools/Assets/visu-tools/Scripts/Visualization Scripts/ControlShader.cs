using UnityEngine;

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

    // necessary information for all shaders containing the motion field
    private MotionField motionScript;
    [Tooltip("How intense needs a pixel value to be after high-pass filtering in order to render motion field for it. If intensity is below threshold, pixel will show up black.")]
    [Range(0f, 0.25f)] [SerializeField] private float threshold;

    // necessary information for all shaders containing some kind of filtering (radial blur, guassian blur, high-pass filter, image sharpening)
    private ImageFilter imageFilterScript;
    [Tooltip("Size of the gaussian kernel used for image filtering. The bigger the stronger the effect.")] [Range(5, 127)] [SerializeField] private int kernelSize;
    [Tooltip("Offset of origin of radial blur effect.")] [Range(0f, 1f)] [SerializeField] private float radialBlurOriginX, radialBlurOriginY;
    [Tooltip("Strength of radial blur effect.")] [Range(0f, 15f)] [SerializeField] private float scale;

    public enum ShaderActive
    {
        None = -1,
        RadialBlur = 0,
        GaussianBlur = 1,
        Sharpening = 2,
        HighPass = 3,
        Motion = 4,
        HighPassOnMotion = 5,
        MotionOnHighPass = 6
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
    }

    /* Method that is being called after camera has finished rendering; here we can modify the final image the user will see */
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        switch (shaderActive)
        {
            case ShaderActive.None: // nothing is applied, image stays the same
                Graphics.Blit(source, destination);
                break;
            case ShaderActive.RadialBlur: // display a radial blur
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.Radial;
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
                var tmp = RenderTexture.GetTemporary(source.width, source.height);
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.OnlyMotion;
                motionScript.RenderShader(source, tmp, motionFieldMaterial);
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.HighPass;
                imageFilterScript.RenderShader(tmp, destination, imageFilterMaterial);
                RenderTexture.ReleaseTemporary(tmp);
                break;
            case ShaderActive.MotionOnHighPass: // first extract high frquencies from image and then apply motion field colors on it
                var tmp2 = RenderTexture.GetTemporary(source.width, source.height);
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.HighPass;
                imageFilterScript.RenderShader(source, tmp2, imageFilterMaterial);
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.MotionOnHighPass;
                motionScript.RenderShader(tmp2, destination, motionFieldMaterial);
                RenderTexture.ReleaseTemporary(tmp2);
                break;
        }
    }

    /* Method that is being called when scene or game ends */
    private void OnDestroy()
    {
        imageFilterScript.CleanUp();
    }
}
