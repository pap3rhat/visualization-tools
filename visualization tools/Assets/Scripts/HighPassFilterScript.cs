using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[ExecuteInEditMode]
public class HighPassFilterScript : MonoBehaviour
{
    private const int DEFAULT_KERNEL_SIZE = 5;
    private const bool DEFAULT_HIGH_PASS = true;

    [SerializeField] private Material material;

    [SerializeField] private int kernelSize;
    private int prevKernelSize;

    [SerializeField] private List<float> kernel;
    [SerializeField] private List<int> offset;
    private ComputeBuffer kernelBuf, offsetBuf;

    [SerializeField] private bool highPass;

    private enum Pass
    {
        BlurH = 0,
        BlurV = 1,
        HighPass = 2
    };

    // method that is being called when the script instance is being loaded
    private void Awake()
    {
        // setting up shader
        kernelSize = DEFAULT_KERNEL_SIZE;
        highPass = DEFAULT_HIGH_PASS;

        SetUpShader();
    }

    // method that is being called after camera has finished rendering; here we can modify the final image the user will see
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (highPass)
        {
            material.SetTexture("_First", source); // setting current frame for shader to use

            var tmp = RenderTexture.GetTemporary(source.width, source.height); // temporary render texture for horizontal blur pass
            var tmp2 = RenderTexture.GetTemporary(source.width, source.height); // temporary render texture for vertical blur pass

            Graphics.Blit(source, tmp, material, (int)Pass.BlurH); // horizontal blur
            Graphics.Blit(tmp, tmp2, material, (int)Pass.BlurV); // vertical blur
            Graphics.Blit(tmp2, destination, material, (int)Pass.HighPass); // applying high pass filter effect to final (fullscreen) image

            // cleaning up
            RenderTexture.ReleaseTemporary(tmp);
            RenderTexture.ReleaseTemporary(tmp2);
        }
        else
        {
            var tmp = RenderTexture.GetTemporary(source.width, source.height); // temporary render texture for bluring

            Graphics.Blit(source, tmp, material, (int)Pass.BlurH); // horizontal blur
            Graphics.Blit(tmp, destination, material, (int)Pass.BlurV); // vertical blur

            RenderTexture.ReleaseTemporary(tmp); // cleaning up
        }
        
    }

    // method that is being called every frame
    private void Update()
    {
        // update kernel if size changed; only do so if kernel size is odd
        if (kernelSize != prevKernelSize && kernelSize % 2 == 1)
        {
            SetUpShader();
        }
    }

    //determines kernel infomration and sets them for shader to use
    private void SetUpShader()
    {
        // calculating kernel information
        CalculateKernel(); 
        CalculateOffset();

        // -- setting kernel information --
        if (kernelBuf != null) // cleaning up
        {
            kernelBuf.Dispose();
            offsetBuf.Dispose();
        }
        kernelBuf = new ComputeBuffer(kernelSize, sizeof(float) * kernelSize); // init buffer
        offsetBuf = new ComputeBuffer(kernelSize, sizeof(int) * kernelSize); // init buffer
        kernelBuf.SetData(kernel); // setting buffer
        offsetBuf.SetData(offset); // setting buffer

        // setting data for shader to use
        material.SetBuffer("_Kernel", kernelBuf);
        material.SetBuffer("_Offset", offsetBuf);
        material.SetInt("_KernelSize", kernelSize);

        // adjusting previous values
        prevKernelSize = kernelSize;
    }

    // calculates guassian kernel as described in "Bildverarbeitung" by FRanz Kummmert 
    private void CalculateKernel()
    {
        kernel = new List<float>();
        kernel.Add(1);
        for (int i = 0; i < (kernelSize - 1) / 2; i++)
        {
            kernel.Add(kernel[i] * (kernelSize - 1 - i) / (i + 1));
        }

        float mult = (float)(1 / System.Math.Pow(2, kernelSize - 1));

        for (int i = 0; i < (kernelSize - 1) / 2; i++)
        {

            kernel[i] *= mult;


        }

        for (int i = (kernelSize - 1) / 2 + 1; i <= kernelSize - 1; i++)
        {
            kernel.Add(kernel[kernelSize - 1 - i]);
        }

        int idx = (int)(System.Math.Floor((float)kernelSize / 2));

        kernel[idx] *= mult;
    }

    // calculates offset to neighboring pixel (not relative to screen size, that will be added inside shader)
    private void CalculateOffset()
    {
        offset = new List<int>();
        offset.AddRange(Enumerable.Range((int)(System.Math.Floor(-(float)kernelSize / 2) + 1), kernelSize));
    }

    // method that is being called when scene or game ends
    private void OnDestroy()
    {
        if (kernelBuf != null) // cleaning up
        {
            kernelBuf.Dispose();
            offsetBuf.Dispose();
        }
    }
}