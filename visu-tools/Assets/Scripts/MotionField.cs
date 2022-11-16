using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MotionField : MonoBehaviour
{
    [SerializeField] private Material material; 

    [SerializeField] private Camera cam;

    // method that is being called when the script instance is being loaded
    private void Awake()
    {
        cam = GetComponent<Camera>(); // getting camera object to which this script is attached
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth | DepthTextureMode.MotionVectors; // activiating option to generate motion vectors 
    }

    // method that is being called after camera has finished rendering; here we can modify the final image the user will see
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);   // shader is being applied to final (fullscreen) image
    }
}
