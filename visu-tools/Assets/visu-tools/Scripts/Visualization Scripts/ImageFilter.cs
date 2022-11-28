using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/* Script that contains functions for image filtering.
 * Radial: Removes high frequencies from image (around an origin).
 * Blur: Removes high frequencies from image.
 * High-Pass: Removes low frequencies from image.
 * Sharpen: Sharpens the image by adding the images high frequencies on top of it.
 * The 'strength' of each effect is determined by the kernel size. The bigger the kernel the stronger the effect.
 */
public class ImageFilter
{
    // --- GENERAL DATA ---

    #region General data
    private enum Pass // determines which passes in shader need to be used
    {
        Radial = 0,
        RadialDesat = 1,
        BlurH = 2,
        BlurV = 3,
        HighPass = 4,
        Sharpening = 5,
        MotionBlur = 6
    };

    public enum FilterMethod // list of all available basic filter methods
    {
        Radial = 0,
        RadialDesat = 1,
        Blur = 2,
        HighPass = 3,
        Sharpening = 4,
        MotionBlur = 5
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
    #endregion

    // --- DATA FOR RADIAL BLUR ---

    #region Radial blur data

    // ínformation about origin of blur effect for radial blur

    private const float DEFAULT_ORIGIN_X = 0.5f; // default is pixel in center of screen along x-axis
    private const float DEFAULT_ORIGIN_Y = 0.5f; // default is pixel in center of screen along y-axis

    private float originYLeftEye; // information for left eye; also used if only one eye is rendered 
    public float OriginYLeftEye
    {
        get { return originYLeftEye; }
        set
        {
            if (value >= 0 && value <= 1) // check if value is within bounds
            {
                originYLeftEye = value;
            }
            else
            {
                Debug.Log("The origin coordinates for y have to be between 0 and 1!");
            }
        }
    }

    private float originYRightEye;
    public float OriginYRightEye  // information for right eye
    {
        get { return originYRightEye; }
        set
        {
            if (value >= 0 && value <= 1) // check if value is within bounds
            {
                originYRightEye = value;
            }
            else
            {
                Debug.Log("The origin coordinates for y have to be between 0 and 1!");
            }
        }
    }

    private float originXLeftEye;
    public float OriginXLeftEye
    {
        get { return originXLeftEye; }
        set
        {
            if (value >= 0 && value <= 1) // check if value is within bounds
            {
                originXLeftEye = value;
            }
            else
            {
                Debug.Log("The origin coordinates for y have to be between 0 and 1!");
            }
        }
    }

    private float originXRightEye;
    public float OriginXRightEye
    {
        get { return originXRightEye; }
        set
        {
            if (value >= 0 && value <= 1) // check if value is within bounds
            {
                originXRightEye = value;
            }
            else
            {
                Debug.Log("The origin coordinates for y have to be between 0 and 1!");
            }
        }
    }

    private float scaleRadial;
    public float ScaleRadial
    {
        get { return scaleRadial; }
        set
        {
            if (value >= 0 && value <= 100) // check if value is within bounds
            {
                scaleRadial = value;
            }
            else
            {
                Debug.Log("The origin coordinates for y have to be between 0 and 1!");
            }
        }
    }
    private const float DEFAULT_SCALE = 5f;

    private float scaleMotionBlur;
    public float ScaleMotionBlur
    {
        get { return scaleMotionBlur; }
        set
        {
            if (value >= 0 && value <= 100) // check if value is within bounds
            {
                scaleMotionBlur = value;
            }
            else
            {
                Debug.Log("The origin coordinates for y have to be between 0 and 1!");
            }
        }
    }

    #endregion

    // --- DATA FOR SHARPENING ---
    private const float sharpeningFactor = 0.85f; // constant for image sharpening (could be made variable and set by user)

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- CONSTRUCTOR ---

    public ImageFilter()
    {
        kernelSize = DEFAULT_KERNEL_SIZE; // init kernelSize

        // setting up default origin and scale of radial blur and motion blur
        originXLeftEye = DEFAULT_ORIGIN_X;
        originXRightEye = DEFAULT_ORIGIN_X;
        originYLeftEye = DEFAULT_ORIGIN_Y;
        originYRightEye = DEFAULT_ORIGIN_Y;
        scaleRadial = DEFAULT_SCALE;
        scaleMotionBlur = DEFAULT_SCALE;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- FUNCTIONS FOR CONTROL SHADER SCRIPT TO USE ---

    /* Method that renders the image and modifies the final image the user will see.
     * Should be called from another script within the OnRenderImage() method. 
     * Material has to use Optical/BasicImageFiltering as a shader!
     */
    public void RenderShader(RenderTexture source, RenderTexture destination, Material material)
    {
        // crucial part for single pass instanced rendering; taken from: https://forum.unity.com/threads/creating-image-effects-for-singlepass-stereo-rendering-with-multiple-shader-passes.710609/
        RenderTextureDescriptor desc;
        if (XRSettings.enabled)
            desc = XRSettings.eyeTextureDesc;
        else
            desc = new RenderTextureDescriptor(Screen.width, Screen.height); // Not XR

        switch (currentFilterMethod)
        {
            case FilterMethod.Radial: // use radial blur
                material.SetFloat("_OriginXLeftEye", originXLeftEye);
                material.SetFloat("_OriginYLeftEye", originYLeftEye);
                material.SetFloat("_OriginXRightEye", originXRightEye);
                material.SetFloat("_OriginYRightEye", originYRightEye);
                material.SetFloat("_Scale", scaleRadial);
                Graphics.Blit(source, destination, material, (int)Pass.Radial);
                break;
            case FilterMethod.RadialDesat: // use radial blur and desaturate colors based on distance from blur origin
                material.SetFloat("_OriginXLeftEye", originXLeftEye);
                material.SetFloat("_OriginYLeftEye", originYLeftEye);
                material.SetFloat("_OriginXRightEye", originXRightEye);
                material.SetFloat("_OriginYRightEye", originYRightEye);
                material.SetFloat("_Scale", scaleRadial);
                Graphics.Blit(source, destination, material, (int)Pass.RadialDesat);
                break;
            case FilterMethod.Blur: // blur the image
                var tmp = RenderTexture.GetTemporary(desc); // temporary render texture for bluring

                Graphics.Blit(source, tmp, material, (int)Pass.BlurH); // horizontal blur
                Graphics.Blit(tmp, destination, material, (int)Pass.BlurV); // vertical blur

                RenderTexture.ReleaseTemporary(tmp); // cleaning up
                break;
            case FilterMethod.HighPass: // apply high pass to image
                material.SetTexture("_First", source); // setting current frame for shader to use

                var tmp2 = RenderTexture.GetTemporary(desc); // temporary render texture for horizontal blur pass
                var tmp3 = RenderTexture.GetTemporary(desc); // temporary render texture for vertical blur pass

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

                var tmp4 = RenderTexture.GetTemporary(desc); // temporary render texture for horizontal blur pass
                var tmp5 = RenderTexture.GetTemporary(desc); // temporary render texture for vertical blur pass

                Graphics.Blit(source, tmp4, material, (int)Pass.BlurH); // horizontal blur
                Graphics.Blit(tmp4, tmp5, material, (int)Pass.BlurV); // vertical blur
                Graphics.Blit(tmp5, destination, material, (int)Pass.Sharpening); // applying high pass filter effect to final (fullscreen) image

                // cleaning up
                RenderTexture.ReleaseTemporary(tmp4);
                RenderTexture.ReleaseTemporary(tmp5);
                break;
            case FilterMethod.MotionBlur: // using motion blur
                material.SetFloat("_Scale", scaleMotionBlur);
                Graphics.Blit(source, destination, material, (int)Pass.MotionBlur);
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

    #region Helper

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

    #endregion
}

