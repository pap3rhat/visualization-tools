using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script that contains function regarding the motion field. */
public class MotionField
{
    private enum Pass // determines which passes in shader need to be used
    {
        OnlyMotion = 0,
        MotionOnHighPass = 1
    };

    public enum FilterMethod // list of all available effects concerning the motion field
    {
        OnlyMotion = 0,
        MotionOnHighPass = 1
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

    private const float DEFAULT_SCALE = 1f;
    private float scale;
    public float Scale
    {
        get { return scale; }
        set
        {
            if (value >= 1 && value <= 10)
            {
                scale = value;
            }
            else
            {
                Debug.LogError("Your input for the scale has to be in [1,10]!");
            }

        }
    }

    private const float DEFAULT_THRESHOLD = 0.0075f;
    private float threshold;
    public float Threshold
    {
        get { return threshold; }
        set
        {
            if (value >= 0 && value <= 1)
            {
                threshold = value;
            }
            else
            {
                Debug.LogError("Your input for the threshold has to be in [0,1]!");
            }

        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- CONSTRUCTOR ---

    public MotionField()
    {
        scale = DEFAULT_SCALE; // init scale
        threshold = DEFAULT_THRESHOLD; // init threshold
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- FUNCTIONS FOR CONTROL SHADER SCRIPT TO USE ---

    /* Method that renders the image and modifies the final image the user will see.
     * Should be called from another script within the OnRenderImage() method. 
     * Material has to use Optical/MotionField as a shader!
     */
    public void RenderShader(RenderTexture source, RenderTexture destination, Material material)
    {
        switch (currentFilterMethod)
        {
            case FilterMethod.OnlyMotion:
                material.SetFloat("_Scale", scale); // setting scale to scale motion vectors with, because they can be quite tiny
                Graphics.Blit(source, destination, material, (int)Pass.OnlyMotion);   // shader is being applied to final (fullscreen) image
                break;
            case FilterMethod.MotionOnHighPass:
                material.SetFloat("_Scale", scale); // setting scale to scale motion vectors with, because they can be quite tiny
                material.SetTexture("_Filtered", source); // setting current texture for shader to use (_Filtered and _MainTex should be the same within filter at this moment, but somehow they are not. This works though.)
                material.SetFloat("_Threshold", threshold); // setting threshold value that determines when intensity of a pixel is high enough in order to be 'shown' (is not black) in the final image 
                Graphics.Blit(source, destination, material, (int)Pass.MotionOnHighPass);   // shader is being applied to final (fullscreen) image
                break;
        }
    }
}
