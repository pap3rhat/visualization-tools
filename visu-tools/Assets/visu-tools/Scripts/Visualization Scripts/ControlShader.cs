using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/* Script that co-ordinates all available shader (combinations).
 * This is the script that needs to be attached to camera that is supposed to use the shaders. 
 */
[RequireComponent(typeof(Camera))]
public class ControlShader : MonoBehaviour
{
    #region Basic variables

    // setup for shader
    [Tooltip("Move camera to which this script is attached in here.")] [SerializeField] private Camera cam;
    [Tooltip("Move material named 'MotionField' in here.")] [SerializeField] private Material motionFieldMaterial;
    [Tooltip("Move material named 'ImageFilter' in here.")] [SerializeField] private Material imageFilterMaterial;
    [Tooltip("Move material named 'Depth' in here.")] [SerializeField] private Material depthMaterial;

    // necessary information for all shaders containing the motion field
    private MotionField motionScript;
    [Tooltip("How strong should the motion vectors be scaled.")] [Range(1f, 10)] [SerializeField] public float scaleMotionField;
    [Tooltip("How intense needs a pixel value to be after high-pass filtering in order to render motion field for it. If intensity is below threshold, pixel will show up black.")]
    [Range(0f, 0.25f)] [SerializeField] public float threshold;

    // necessary information for all shaders containing some kind of filtering (radial blur, guassian blur, high-pass filter, image sharpening)
    private ImageFilter imageFilterScript;
    [Tooltip("Size of the gaussian kernel used for image filtering. The bigger the stronger the effect.")] [Range(5, 127)] [SerializeField] public int kernelSize;
    [Tooltip("Offset of origin of radial blur effect.")] [Range(0f, 1f)] [SerializeField] public float radialBlurOriginX, radialBlurOriginY;
    [Tooltip("Offset of origin of radial blur effect for the left eye.")] [Range(0f, 1f)] [SerializeField] public float radialBlurOriginXLeftEye, radialBlurOriginYLeftEye;
    [Tooltip("Offset of origin of radial blur effect for the right eye.")] [Range(0f, 1f)] [SerializeField] public float radialBlurOriginXRightEye, radialBlurOriginYRightEye;
    [Tooltip("Strength of radial blur effect.")] [Range(0f, 10f)] [SerializeField] public float scaleRadial;
    [Tooltip("Strength of motion blur effect.")] [Range(0f, 10f)] [SerializeField] public float scaleMotionBlur;
    // necessary information for all shaders containing the depth
    private Depth depthScript;
    [Tooltip("Color of the objects close to the camera.")] [ColorUsage(true)] [SerializeField] private Color colorNear;
    [Tooltip("Color of the objects farthest from the camera.")] [ColorUsage(true)] [SerializeField] private Color colorFar;

    #endregion

    #region XR information
    // information if xr is used
    private bool xrActive;
    public bool XrActive
    {
        get { return xrActive; }
        set
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            xrActive = xrDisplaySubsystems.Count > 0;
        }
    }

    #endregion

    #region Active shader information
    public enum ShaderActive
    {
        None = -1,
        RadialBlur = 0,
        RadialBlurDesat = 1,
        GaussianBlur = 2,
        Sharpening = 3,
        HighPass = 4,
        MotionBlur = 5,
        MotionField = 6,
        HighPassOnMotionField = 7,
        MotionFieldOnHighPass = 8,
        Depth = 9,
        HighPassOnDepth = 10,
        MotionFieldOnHighPassOnDepth = 11
    }

    [Tooltip("Which image effect should be applied.")] public ShaderActive shaderActive;

    #endregion

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- MONO BEHAVIOUR METHODS ---

    #region Mono behaviour methods

    /* Method that is being called when the script instance is being loaded */
    private void Awake()
    {
        BasciSetUp();

        MotionFieldSetUp();
        ImageFilterSetUp();
        DepthSetUp();
    }

    /* Method that is being called every frame */
    private void Update()
    {
        // updating necessary information based on which shader is active 
        switch (shaderActive)
        {
            case ShaderActive.RadialBlur:
            case ShaderActive.RadialBlurDesat:
                UpdateKernelInformation();
                UpdateBlurOriginInformation();
                UpdateRadialBlurScaleInformation();
                break;
            case ShaderActive.GaussianBlur:
            case ShaderActive.Sharpening:
            case ShaderActive.HighPass:
                UpdateKernelInformation();
                break;
            case ShaderActive.MotionBlur:
                UpdateKernelInformation();
                UpdateMotionBlurScaleInformation();
                break;
            case ShaderActive.MotionField:
                UpdateMotionFieldScaleInformation();
                break;
            case ShaderActive.HighPassOnMotionField:
                UpdateKernelInformation();
                UpdateMotionFieldScaleInformation();
                break;
            case ShaderActive.MotionFieldOnHighPass:
                UpdateKernelInformation();
                UpdateMotionFieldScaleInformation();
                UpdateThresholdInformation();
                break;
            case ShaderActive.Depth:
                UpdateColorInformation();
                break;
            case ShaderActive.HighPassOnDepth:
                UpdateKernelInformation();
                UpdateColorInformation();
                break;
            case ShaderActive.MotionFieldOnHighPassOnDepth:
                UpdateKernelInformation();
                UpdateMotionFieldScaleInformation();
                UpdateThresholdInformation();
                UpdateColorInformation();
                break;
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
            case ShaderActive.MotionBlur: // display a motion blur
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.MotionBlur;
                imageFilterScript.RenderShader(source, destination, imageFilterMaterial);
                break;
            case ShaderActive.MotionField: // display motion field as colors
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.OnlyMotion;
                motionScript.RenderShader(source, destination, motionFieldMaterial);
                break;
            case ShaderActive.HighPassOnMotionField: // first display motion field as colors and then extract only the high frquencies from it
                var tmp = RenderTexture.GetTemporary(desc);
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.OnlyMotion;
                motionScript.RenderShader(source, tmp, motionFieldMaterial);
                imageFilterScript.CurrentFilterMethod = ImageFilter.FilterMethod.HighPass;
                imageFilterScript.RenderShader(tmp, destination, imageFilterMaterial);
                RenderTexture.ReleaseTemporary(tmp);
                break;
            case ShaderActive.MotionFieldOnHighPass: // first extract high frquencies from image and then apply motion field colors on it
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
            case ShaderActive.MotionFieldOnHighPassOnDepth: // motion field on high-pass filter on depth 
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

    #endregion

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- HELPER METHODS ---

    #region Set-up helper

    /* Sets up all infomration necessary for depth effects */
    private void DepthSetUp()
    {
        // set-up for all shaders regarding the depth
        depthScript = new Depth();
        colorNear = depthScript.ColorNear;
        colorFar = depthScript.ColorFar;
    }

    /* Sets up all infomration necessary for image filters effects*/
    private void ImageFilterSetUp()
    {
        // set-up for all shaders containing some kind of filtering
        imageFilterScript = new ImageFilter();
        imageFilterScript.KernelSetUp(imageFilterMaterial);
        kernelSize = imageFilterScript.KernelSize;
        radialBlurOriginX = imageFilterScript.OriginXLeftEye;
        radialBlurOriginY = imageFilterScript.OriginYLeftEye;
        radialBlurOriginXLeftEye = imageFilterScript.OriginXLeftEye;
        radialBlurOriginYLeftEye = imageFilterScript.OriginYLeftEye;
        radialBlurOriginXRightEye = imageFilterScript.OriginXRightEye;
        radialBlurOriginYRightEye = imageFilterScript.OriginYRightEye;
        scaleRadial = imageFilterScript.ScaleRadial;
        scaleMotionBlur = imageFilterScript.ScaleMotionBlur;
    }

    /* Sets up all infomration necessary for motion field effects */
    private void MotionFieldSetUp()
    {
        // set-up for all shaders regarding the motion field
        motionScript = new MotionField();
        scaleMotionField = motionScript.Scale;
        threshold = motionScript.Threshold;
    }

    /* Sets up all gernal information that is needed */
    private void BasciSetUp()
    {
        // setting active shader to "None" as a default
        shaderActive = ShaderActive.None;

        // setting up camera
        cam = GetComponent<Camera>(); // getting camera object to which this script is attached
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth | DepthTextureMode.MotionVectors; // activiating option to generate motion vectors 

        // checking if xr is being used (check happens above, the true here will be overwritten by either true or false)
        XrActive = true;
    }

    #endregion

    #region Update helper

    /* Updates color information for depth effects */
    private void UpdateColorInformation()
    {
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

    /* Updates threshold information for motion on high pass effects */
    private void UpdateThresholdInformation()
    {
        // checking if threshold for motion field on high-pass needs to be updated
        if (threshold != motionScript.Threshold && threshold >= 0 & threshold <= 1)
        {
            motionScript.Threshold = threshold;
        }
    }

    /* Updates scale motion field information for motion field effects */
    private void UpdateMotionFieldScaleInformation()
    {
        // checking if scale for motion vectors changed
        if (scaleMotionField != motionScript.Scale && scaleMotionField >= 1 && scaleMotionField <= 10)
        {
            motionScript.Scale = scaleMotionField;
        }
    }

    /* Updates scale motion blur information for motion blur effect */
    private void UpdateMotionBlurScaleInformation()
    {
        // checking if scale for motion blur changed
        if (scaleMotionBlur != imageFilterScript.ScaleMotionBlur && scaleMotionBlur >= 0 && scaleMotionBlur <= 100)
        {
            imageFilterScript.ScaleMotionBlur = scaleMotionBlur;

        }
    }

    /* Updates scale radial blur information for radial blur effects */
    private void UpdateRadialBlurScaleInformation()
    {
        // checking if scale for radial blurred changed
        if (scaleRadial != imageFilterScript.ScaleRadial && scaleRadial >= 0 && scaleRadial <= 100)
        {
            imageFilterScript.ScaleRadial = scaleRadial;
        }
    }

    /* Updates blur origin information for radial blur effects */
    private void UpdateBlurOriginInformation()
    {
        // checking if origin for radial blur changed
        if (!xrActive)
        {
            if (radialBlurOriginX != imageFilterScript.OriginXLeftEye && radialBlurOriginX >= 0 && radialBlurOriginX <= 1)
            {
                imageFilterScript.OriginXLeftEye = radialBlurOriginX;
            }
            if (radialBlurOriginY != imageFilterScript.OriginYLeftEye && radialBlurOriginY >= 0 && radialBlurOriginY <= 1)
            {
                imageFilterScript.OriginYLeftEye = radialBlurOriginY;
            }
        }
        else
        {
            if (radialBlurOriginXLeftEye != imageFilterScript.OriginXLeftEye && radialBlurOriginXLeftEye >= 0 && radialBlurOriginXLeftEye <= 1)
            {
                imageFilterScript.OriginXLeftEye = radialBlurOriginXLeftEye;
            }
            if (radialBlurOriginYLeftEye != imageFilterScript.OriginYLeftEye && radialBlurOriginYLeftEye >= 0 && radialBlurOriginYLeftEye <= 1)
            {
                imageFilterScript.OriginYLeftEye = radialBlurOriginYLeftEye;
            }
            if (radialBlurOriginXRightEye != imageFilterScript.OriginXRightEye && radialBlurOriginXRightEye >= 0 && radialBlurOriginXRightEye <= 1)
            {
                imageFilterScript.OriginXRightEye = radialBlurOriginXRightEye;
            }
            if (radialBlurOriginYRightEye != imageFilterScript.OriginYRightEye && radialBlurOriginYRightEye >= 0 && radialBlurOriginYRightEye <= 1)
            {
                imageFilterScript.OriginYRightEye = radialBlurOriginYRightEye;
            }
        }
    }

    /* Updates kernel information for image filter effects */
    private void UpdateKernelInformation()
    {
        // checking if kernel for basic image filtering needs to be updated
        if (kernelSize != imageFilterScript.KernelSize && kernelSize % 2 != 0 && kernelSize >= ImageFilter.MIN_KERNEL_SIZE)
        {
            imageFilterScript.KernelSize = kernelSize;
            imageFilterScript.KernelSetUp(imageFilterMaterial);
        }
    }
    #endregion

}
