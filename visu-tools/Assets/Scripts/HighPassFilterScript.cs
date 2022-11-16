using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class HighPassFilterScript : MonoBehaviour
{
    private const int DEFAULT_KERNEL_SIZE = 9;
    private const int MIN_KERNEL_SIZE = 5;
    private const bool DEFAULT_HIGH_PASS = true;

    [SerializeField] private Material material;

    [SerializeField] private int kernelSize;
    private int prevKernelSize;

    [SerializeField] private List<float> kernel;
    [SerializeField] private List<float> offset;
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
        // update kernel if size changed; only do so if kernel size is odd and bigger than or equal to 5
        if (kernelSize != prevKernelSize && kernelSize % 2 == 1 && kernelSize >= MIN_KERNEL_SIZE)
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
        LinearSampling();

        // -- setting kernel information --
        if (kernelBuf != null) // cleaning up
        {
            kernelBuf.Dispose();
            offsetBuf.Dispose();
        }

        int finalKernelSize = kernel.Count;

        kernelBuf = new ComputeBuffer(finalKernelSize, sizeof(float) * finalKernelSize); // init buffer
        offsetBuf = new ComputeBuffer(finalKernelSize, sizeof(float) * finalKernelSize); // init buffer
        kernelBuf.SetData(kernel); // setting buffer
        offsetBuf.SetData(offset); // setting buffer

        // setting data for shader to use
        material.SetBuffer("_Kernel", kernelBuf);
        material.SetBuffer("_Offset", offsetBuf);
        material.SetInt("_FinalKernelSize", finalKernelSize);

        // adjusting previous values
        prevKernelSize = kernelSize;
    }

    // calculates guassian kernel by using pascals triangle as described in "Bildverarbeitung" by Franz Kummmert and https://www.rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/
    private void CalculateKernel()
    {
        int discard = 2; // determines how many of the outer values should be discarded/ignored -> saves computational time within shader by blurring more with a smaller kernel
        int biggerSize = kernelSize + discard * 2; // new size that will be calculated before discarding

        // preaparing before calculating values
        kernel = new List<float>();
        kernel.Add(1);

        // calculating first half of pascals triangle row (second half is just mirrored at middle value)
        for (int i = 0; i < (biggerSize - 1) / 2; i++)
        {
            kernel.Add(kernel[i] * (biggerSize - 1 - i) / (i + 1));
        }

        float scale = (float)(1 / ((System.Math.Pow(2, biggerSize - 1) - discard * kernel[0] - discard * kernel[1]))); // determining scaling factor for each kernel value given that values will be discarded

        kernel.RemoveRange(0, discard); // removing outer values

        // scaling every value (except middle one)
        for (int i = 0; i <= (kernelSize - 1) / 2; i++)
        {
            kernel[i] *= scale;
        }

        kernel.Reverse(); // reversing list so middle element is at index 0
    }

    // calculates offset to neighboring pixel (not relative to screen size, that will be added by shader)
    private void CalculateOffset()
    {
        offset = new List<float>();
        int halfSize = (int)Mathf.Ceil(kernelSize / 2f); // half the size of the kernel (+1 middle value); is enough because kernel is "mirrored" at middle value
        for (int i = 0; i < halfSize; i++)
        {
            offset.Add(i);
        }
    }

    // modifies kernel and offset by using linear sampling as described here:  https://www.rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/
    private void LinearSampling()
    {
        List<float> newOffset = new List<float>();
        List<float> newKernel = new List<float>();

        // middle element stays the same
        newOffset.Add(offset[0]);
        newKernel.Add(kernel[0]);

        // getting information of two pixel by sampling between them
        for (int i = 1; i < kernel.Count - 1; i += 2)
        {
            float weightSum = kernel[i] + kernel[i + 1];
            newKernel.Add(weightSum);
            newOffset.Add((offset[i] * kernel[i] + offset[i + 1] * kernel[i + 1]) / weightSum);
        }

        // kernel has an unfortunate size for linear sampling
        if (kernel.Count % 2 == 0)
        {
            newOffset.Add(offset[kernel.Count - 1]);
            newKernel.Add(kernel[kernel.Count - 1]);
        }

        offset = newOffset;
        kernel = newKernel;
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