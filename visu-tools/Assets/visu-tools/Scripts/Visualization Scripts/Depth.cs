using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depth
{
    // --- COLOR DATA ---

    #region Color data

    private Color colorNear;
    public Color ColorNear
    {
        get { return colorNear; }
        set { colorNear = value; }
    }
    private Color DEFAULT_COLOR_NEAR = new Color(1f, 1f, 1f, 1f);

    private Color colorFar;
    public Color ColorFar
    {
        get { return colorFar; }
        set { colorFar = value; }
    }
    private Color DEFAULT_COLOR_FAR = new Color(0f, 0f, 0f, 1f);

    #endregion

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- CONSTRUCTOR ---
    public Depth()
    {
        // setting up default colors
        colorNear = DEFAULT_COLOR_NEAR;
        colorFar = DEFAULT_COLOR_FAR;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- FUNCTIONS FOR CONTROL SHADER SCRIPT TO USE ---

    #region To be called method

    /* Method that renders the image and modifies the final image the user will see.
     * Should be called from another script within the OnRenderImage() method. 
     * Material has to use Optical/BasicImageFiltering as a shader!
     */
    public void RenderShader(RenderTexture source, RenderTexture destination, Material material)
    {
        material.SetColor("_ColorNear", colorNear);
        material.SetColor("_ColorFar", colorFar);
        Graphics.Blit(source, destination, material);
    }

    #endregion
}
