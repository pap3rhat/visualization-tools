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

        // set-up for all shaders regarding the motion field
        motionScript = new MotionField();
        cam = GetComponent<Camera>(); // getting camera object to which this script is attached
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth | DepthTextureMode.MotionVectors; // activiating option to generate motion vectors 

        // set-up for all shaders containing some kind of "basic" filtering
        basicImageFilterScript = new BasicImageFilter();
        basicImageFilterScript.KernelSetUp(basicImageFilterMaterial);
        kernelSize = basicImageFilterScript.KernelSize;
    }

    /* Method that is being called every frame */
    private void Update()
    {
        if (kernelSize != basicImageFilterScript.KernelSize && kernelSize % 2 != 0 && kernelSize >= BasicImageFilter.MIN_KERNEL_SIZE)
        {
            basicImageFilterScript.KernelSize = kernelSize;
            basicImageFilterScript.KernelSetUp(basicImageFilterMaterial);
        }
    }

    /* Method that is being called after camera has finished rendering; here we can modify the final image the user will see */
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        switch (shaderActive)
        {
            case ShaderActive.None:
                Graphics.Blit(source, destination);
                break;
            case ShaderActive.Motion:
                motionScript.RenderShader(source, destination, motionFieldMaterial);
                break;
            case ShaderActive.Blur:
                basicImageFilterScript.CurrentFilterMethod = BasicImageFilter.FilterMethod.Blur;
                basicImageFilterScript.RenderShader(source, destination, basicImageFilterMaterial);
                break;
            case ShaderActive.Sharpening:
                basicImageFilterScript.CurrentFilterMethod = BasicImageFilter.FilterMethod.Sharpening;
                basicImageFilterScript.RenderShader(source, destination, basicImageFilterMaterial);
                break;
            case ShaderActive.HighPass:
                basicImageFilterScript.CurrentFilterMethod = BasicImageFilter.FilterMethod.HighPass;
                basicImageFilterScript.RenderShader(source, destination, basicImageFilterMaterial);
                break;
        }
    }

    /* Method that is being called when scene or game ends */
    private void OnDestroy()
    {
        basicImageFilterScript.CleanUp();
    }
}
