using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script that co-ordinates all available shader (combinations).
 * This is the script that needs to be attached to camera that is supposed to use the shaders. 
 */
[RequireComponent(typeof(Camera))]
public class ControlShader : MonoBehaviour
{
    // necessary information for all shaders containing the motion field
    private MotionField motionScript;
    [SerializeField] private Material motionFieldMaterial;
    [SerializeField] private Camera cam;
    [SerializeField] private float threshold;

    // necessary information for all shaders containing some kind of "basic" filtering (basic image blurr, high-pass filter, basic image sharpening)
    private BasicImageFilter basicImageFilterScript;
    [SerializeField] private Material basicImageFilterMaterial;
    [SerializeField] private int kernelSize;

    private enum ShaderActive
    {
        None = -1,
        Motion = 0,
        Blur = 1,
        Sharpening = 2,
        HighPass = 3,
        HighPassOnMotion = 4,
        MotionOnHighPass = 5
    }

    [SerializeField] private ShaderActive shaderActive;

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
        basicImageFilterScript = new BasicImageFilter();
        basicImageFilterScript.KernelSetUp(basicImageFilterMaterial);
        kernelSize = basicImageFilterScript.KernelSize;
    }

    /* Method that is being called every frame */
    private void Update()
    {
        // checking if kernel for basic image filtering needs to be updated
        if (kernelSize != basicImageFilterScript.KernelSize && kernelSize % 2 != 0 && kernelSize >= BasicImageFilter.MIN_KERNEL_SIZE)
        {
            basicImageFilterScript.KernelSize = kernelSize;
            basicImageFilterScript.KernelSetUp(basicImageFilterMaterial);
        }

        // checking if tehrshold for motion field on high-pass needs to be updated
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
            case ShaderActive.None: // nothing is apllied, image stays the same
                Graphics.Blit(source, destination);
                break;
            case ShaderActive.Motion: // display motion field as colors
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.OnlyMotion;
                motionScript.RenderShader(source, destination, motionFieldMaterial);
                break;
            case ShaderActive.Blur: // blur (low-pass filter) the image (remove high frequencies)
                basicImageFilterScript.CurrentFilterMethod = BasicImageFilter.FilterMethod.Blur;
                basicImageFilterScript.RenderShader(source, destination, basicImageFilterMaterial);
                break;
            case ShaderActive.Sharpening: // sharpen the image (add high image frequencies on top of image)
                basicImageFilterScript.CurrentFilterMethod = BasicImageFilter.FilterMethod.Sharpening;
                basicImageFilterScript.RenderShader(source, destination, basicImageFilterMaterial);
                break;
            case ShaderActive.HighPass: // high-pass filter the image (remove low frequencies)
                basicImageFilterScript.CurrentFilterMethod = BasicImageFilter.FilterMethod.HighPass;
                basicImageFilterScript.RenderShader(source, destination, basicImageFilterMaterial);
                break;
            case ShaderActive.HighPassOnMotion: // first display motion field as colors and then extract only the high frquencies from it
                var tmp = RenderTexture.GetTemporary(source.width, source.height);
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.OnlyMotion;
                motionScript.RenderShader(source, tmp, motionFieldMaterial);
                basicImageFilterScript.CurrentFilterMethod = BasicImageFilter.FilterMethod.HighPass;
                basicImageFilterScript.RenderShader(tmp, destination, basicImageFilterMaterial);
                RenderTexture.ReleaseTemporary(tmp);
                break;
            case ShaderActive.MotionOnHighPass: // first extract high frquencies from image and then apply motion field colors on it
                var tmp2 = RenderTexture.GetTemporary(source.width, source.height);
                basicImageFilterScript.CurrentFilterMethod = BasicImageFilter.FilterMethod.HighPass;
                basicImageFilterScript.RenderShader(source, tmp2, basicImageFilterMaterial);
                motionScript.CurrentFilterMethod = MotionField.FilterMethod.MotionOnHighPass;
                motionScript.RenderShader(tmp2, destination, motionFieldMaterial);
                RenderTexture.ReleaseTemporary(tmp2);
                break;
        }
    }

    /* Method that is being called when scene or game ends */
    private void OnDestroy()
    {
        basicImageFilterScript.CleanUp();
    }
}
