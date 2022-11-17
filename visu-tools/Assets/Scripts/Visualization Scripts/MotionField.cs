using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script that contains function regarding the motion field.
 * Very small right now, might not me necessary to be its own script atm.
 */
public class MotionField
{
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- FUNCTIONS FOR CONTROL SHADER SCRIPT TO USE ---

    /* Method that renders the image and modifies the final image the user will see.
     * Should be called from another script within the OnRenderImage() method. 
     * Material has to use Optical/MotionField as a shader!
     */
    public void RenderShader(RenderTexture source, RenderTexture destination, Material material)
    {
        Graphics.Blit(source, destination, material);   // shader is being applied to final (fullscreen) image
    }
}
