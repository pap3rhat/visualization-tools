using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script that contains functions for image filtering.
 * Radial: Removes high frequencies from image (around an origin).
 * Blur: Removes high frequencies from image.
 * High-Pass: Removes low frequencies from image.
 * Sharpen: Sharpens the image by adding the images high frequencies on top of it.
 * The 'strength' of each effect is determined by the kernel size. The bigger the kernel the stronger the effect.
 */
public class BasicImageFilter
{
    // --- GENERAL DATA ---
    private enum Pass // determines which passes in shader need to be used
    {
        Radial = 0,
        BlurH = 1,
        BlurV = 2,
        HighPass = 3,
        Sharpening = 4
    };

    public enum FilterMethod // list of all available basic filter methods
    {
        Radial = 0,
        Blur = 1,
        HighPass = 2,
        Sharpening = 3
    };

    private FilterMethod currentFilterMethod;
    public FilterMethod CurrentFilterMethod  // determines which filter method should be applied 
    {
        get { return currentFilterMethod; }
        set
        {
            if (Enum.IsDefined(typeof(FilterMethod), value))
            {
                currentFilterMethod = value;
            }
            else
            {
                Debug.LogError("Your input was not a valid basic filter method!");
            }
        }
    }

    // basic information for filter kernel
    private const int DEFAULT_KERNEL_SIZE = 9;
    public const int MIN_KERNEL_SIZE = 5;

    private int kernelSize;
    public int KernelSize
    {
        get { return kernelSize; }
        set
        {
            if (value % 2 != 0 && value >= MIN_KERNEL_SIZE) // check for correct size (ideally one is never able to input a wrong size)
            {
                kernelSize = value;
            }
            else
            {
                Debug.Log("Kernel size has to be an odd number and bigger than " + (MIN_KERNEL_SIZE - 1) + "!");
            }
        }
    }

    private List<float> kernel;
    private List<float> offset;
    private ComputeBuffer kernelBuf, offsetBuf;

    // --- DATA FOR RADIAL BLUR ---

    // ínformation about origin of blur effect for radial blur
    private float originX;
    public float OriginX
    {
        get { return originX; }
        set
        {
            if (value >= 0 && value <= 1) // check if value is within bounds
            {
                originX = value;
            }
            else
            {
                Debug.Log("The origin coordinates for x have to be between 0 and 1!");
            }
        }
    }
    private const float DEFAULT_ORIGIN_X = 0.5f; // default is pixel in center of screen along x-axis

    private float originY;
    public float OriginY
    {
        get { return originY; }
        set
        {
            if (value >= 0 && value <= 1) // check if value is within bounds
            {
                originY = value;
            }
            else
            {
                Debug.Log("The origin coordinates for y have to be between 0 and 1!");
            }
        }
    }
    private const float DEFAULT_ORIGIN_Y = 0.5f; // default is pixel in center of screen along y-axis

    private float scale;
    public float Scale
    {
        get { return scale; }
        set
        {
            if (value >= 0 && value <= 100) // check if value is within bounds
            {
                scale = value;
            }
            else
            {
                Debug.Log("The origin coordinates for y have to be between 0 and 1!");
            }
        }
    }
    private const float DEFAULT_SCALE = 5f;

    // --- DATA FOR SHARPENING ---
    private const float sharpeningFactor = 0.85f; // constant for image sharpening (could be made variable and set by user)

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- CONSTRUCTOR ---

    public BasicImageFilter()
    {
        kernelSize = DEFAULT_KERNEL_SIZE; // init kernelSize

        // setting up default origin and scale of radial blur
        originX = DEFAULT_ORIGIN_X;
        originY = DEFAULT_ORIGIN_Y;
        scale = DEFAULT_SCALE;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- FUNCTIONS FOR CONTROL SHADER SCRIPT TO USE ---

    /* Method that renders the image and modifies the final image the user will see.
     * Should be called from another script within the OnRenderImage() method. 
     * Material has to use Optical/BasicImageFiltering as a shader!
     */
    public void RenderShader(RenderTexture source, RenderTexture destination, Material material)
    {
        switch (currentFilterMethod)
        {
            case FilterMethod.Radial: // use radial blur
                material.SetFloat("_OriginX", originX);
                material.SetFloat("_OriginY", originY);
                material.SetFloat("_Scale", scale);
                Graphics.Blit(source, destination, material, (int)Pass.Radial);
                break;
            case FilterMethod.Blur: // blur the image
                var tmp = RenderTexture.GetTemporary(source.width, source.height); // temporary render texture for bluring

                Graphics.Blit(source, tmp, material, (int)Pass.BlurH); // horizontal blur
                Graphics.Blit(tmp, destination, material, (int)Pass.BlurV); // vertical blur

                RenderTexture.ReleaseTemporary(tmp); // cleaning up
                break;
            case FilterMethod.HighPass: // apply high pass to image
                material.SetTexture("_First", source); // setting current frame for shader to use

                var tmp2 = RenderTexture.GetTemporary(source.width, source.height); // temporary render texture for horizontal blur pass
                var tmp3 = RenderTexture.GetTemporary(source.width, source.height); // temporary render texture for vertical blur pass

                Graphics.Blit(source, tmp2, material, (int)Pass.BlurH); // horizontal blur
                Graphics.Blit(tmp2, tmp3, material, (int)Pass.BlurV); // vertical blur
                Graphics.Blit(tmp3, destination, material, (int)Pass.HighPass); // applying high pass filter effect to final (fullscreen) image

                // cleaning up
                RenderTexture.ReleaseTemporary(tmp2);
                RenderTexture.ReleaseTemporary(tmp3);
                break;
            case FilterMethod.Sharpening: // sharpen the image
                material.SetTexture("_First", source); // setting current frame for shader to use
                material.SetFloat("_SharpeningFactor", sharpeningFactor); // setting sharpening factor for shader to use

                var tmp4 = RenderTexture.GetTemporary(source.width, source.height); // temporary render texture for horizontal blur pass
                var tmp5 = RenderTexture.GetTemporary(source.width, source.height); // temporary render texture for vertical blur pass

                Graphics.Blit(source, tmp4, material, (int)Pass.BlurH); // horizontal blur
                Graphics.Blit(tmp4, tmp5, material, (int)Pass.BlurV); // vertical blur
                Graphics.Blit(tmp5, destination, material, (int)Pass.Sharpening); // applying high pass filter effect to final (fullscreen) image

                // cleaning up
                RenderTexture.ReleaseTemporary(tmp4);
                RenderTexture.ReleaseTemporary(tmp5);
                break;
        }
    }

    /* Determines kernel information and sets them for shader (given by material) to use */
    public void KernelSetUp(Material material)
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
    }

    /* Disposes compute buffer */
    public void CleanUp()
    {
        if (kernelBuf != null) // cleaning up
        {
            kernelBuf.Dispose();
            offsetBuf.Dispose();
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
    // --- HELPER FUNCTIONS ---

    /* Calculates guassian kernel by using pascals triangle as described in "Bildverarbeitung" by Franz Kummmert and https://www.rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/ */
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

    /* Calculates offset to neighboring pixel (not relative to screen size, that will be added by shader) */
    private void CalculateOffset()
    {
        offset = new List<float>();
        int halfSize = (int)Mathf.Ceil(kernelSize / 2f); // half the size of the kernel (+1 middle value); is enough because kernel is "mirrored" at middle value
        for (int i = 0; i < halfSize; i++)
        {
            offset.Add(i);
        }
    }

    /* Modifies kernel and offset by using linear sampling as described here: https://www.rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/ */
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
}

