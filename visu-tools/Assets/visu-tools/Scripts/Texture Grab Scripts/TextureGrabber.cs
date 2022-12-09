using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

/* Grabs the motion vectors texture and depth texture if wanted and saves them as png files
 * If no xr is used it is just one png for every time OnRenderImage is called
 * If xr is used in multipass mode there is one image for the left eye and one image for the right eye for every time OnRenderImage is called
 * If xr is used in single pass instanced mode ther is only the image for the left eye for every time OnRenderImage is called
 */
public class TextureGrabber : MonoBehaviour
{
    // set up for shader
    [Tooltip("Move camera to which this script is attached in here.")] [SerializeField] private Camera cam;
    [Tooltip("Move material named 'TextureGrabber' in here.")] [SerializeField] private Material grabberMaterial;
    [Tooltip("Where should the files be saved? If empty 'Application.persistentDataPath' is used")] [SerializeField] private string filePath;

    // set up for deciding what to save
    [Tooltip("Determines wether the motion vector texture should be saved to an image file.")] [SerializeField] private bool motionSave;
    [Tooltip("Determines wether the depth vector texture should be saved to an image file.")] [SerializeField] private bool depthSave;
    private const bool DEFAULT_MOTION_SAVE = false;
    private const bool DEFAULT_DEPTH_SAVE = false;
    private const string SAVE_SUBFOLDER_MOTION = "MotionGrab";
    private const string SAVE_SUBFOLDER_DEPTH = "DepthGrab";
    private const string FILE_BEGINNGING_MOTION = "motion";
    private const string FILE_BEGINNING_DEPTH = "depth";

    // which pass need to be used
    private enum SavePass
    {
        None = -1,
        Motion = 0,
        Depth = 1
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- MONO BEHAVIOUR METHODS ---

    #region Mono behaviour methods

    /* Method that is being called when the script instance is being loaded */
    private void Awake()
    {
        BasicSetUp();
        DirSetUp();
    }

    /* Method that is being called after camera has finished rendering */
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (motionSave)
        {
            // crucial part for single pass instanced rendering; taken from: https://forum.unity.com/threads/creating-image-effects-for-singlepass-stereo-rendering-with-multiple-shader-passes.710609/
            RenderTextureDescriptor desc;
            if (XRSettings.enabled)
                desc = XRSettings.eyeTextureDesc;
            else
                desc = new RenderTextureDescriptor(Screen.width, Screen.height); // Not XR

            // rendering into temporary rendertexture
            var tmp = RenderTexture.GetTemporary(desc);
            Graphics.Blit(source, tmp, grabberMaterial, (int)SavePass.Motion);

            SaveTextureAsPng(tmp, SAVE_SUBFOLDER_MOTION, FILE_BEGINNGING_MOTION + "_" + cam.stereoActiveEye.ToString().ToLower()); // saving motion vector texture as png

            RenderTexture.ReleaseTemporary(tmp); // cleaning up
        }

        if (depthSave)
        {
            // crucial part for single pass instanced rendering; taken from: https://forum.unity.com/threads/creating-image-effects-for-singlepass-stereo-rendering-with-multiple-shader-passes.710609/
            RenderTextureDescriptor desc;
            if (XRSettings.enabled)
                desc = XRSettings.eyeTextureDesc;
            else
                desc = new RenderTextureDescriptor(Screen.width, Screen.height); // Not XR

            // rendering into temporary rendertexture
            var tmp = RenderTexture.GetTemporary(desc);
            Graphics.Blit(source, tmp, grabberMaterial, (int)SavePass.Depth);
            SaveTextureAsPng(tmp, SAVE_SUBFOLDER_DEPTH, FILE_BEGINNING_DEPTH + "_" + cam.stereoActiveEye.ToString().ToLower()); // saving depth texture as png

            RenderTexture.ReleaseTemporary(tmp); // cleaning up
        }

        Graphics.Blit(source, destination); // just returning image wihtout any effects



    }

    #endregion

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- HELPER METHODS ---

    #region Set up

    /* Sets everything up */
    private void BasicSetUp()
    {
        // setting up camera
        cam = GetComponent<Camera>(); // getting camera object to which this script is attached
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth | DepthTextureMode.MotionVectors; // activiating option to generate motion vectors 

        // setting up save bools
        motionSave = DEFAULT_MOTION_SAVE;
        depthSave = DEFAULT_DEPTH_SAVE;

        // maing sure float numbers are stored with a "." as a comma
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
    }

    /* Setting up sub-folders to save pngs in */
    private void DirSetUp()
    {
        if (filePath != null && !filePath.Equals(""))
        {
            if (!Directory.Exists(Path.Combine(filePath, SAVE_SUBFOLDER_DEPTH)))
            {
                Directory.CreateDirectory(Path.Combine(filePath, SAVE_SUBFOLDER_DEPTH));
            }

            if (!Directory.Exists(Path.Combine(filePath, SAVE_SUBFOLDER_MOTION)))
            {
                Directory.CreateDirectory(Path.Combine(filePath, SAVE_SUBFOLDER_MOTION));
            }
        }
        else
        {
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, SAVE_SUBFOLDER_DEPTH)))
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, SAVE_SUBFOLDER_DEPTH));
            }

            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, SAVE_SUBFOLDER_MOTION)))
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, SAVE_SUBFOLDER_MOTION));
            }
        }
    }

    #endregion

    /* Method that converts RenderTexture into texture and saves texture into png; mainly from: https://gist.github.com/AlexanderDzhoganov/d795b897005389071e2a */
    private void SaveTextureAsPng(RenderTexture rendTex, string subFolder, string fileBeginning)
    {
        var curRendTex = RenderTexture.active;

        var tex = new Texture2D(rendTex.width, rendTex.height);
        RenderTexture.active = rendTex;
        tex.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0); // reading reder texture into texture
        tex.Apply();

        // saving texture as png 
        if (filePath != null && !filePath.Equals(""))
        {
            File.WriteAllBytes(filePath + Path.DirectorySeparatorChar + subFolder + Path.DirectorySeparatorChar + fileBeginning + "_" + Time.frameCount + "_" + Time.deltaTime + ".png", tex.EncodeToPNG());
        }
        else
        {
            File.WriteAllBytes(Application.persistentDataPath + Path.DirectorySeparatorChar + subFolder + Path.DirectorySeparatorChar + fileBeginning + "_" + Time.frameCount + "_" + Time.deltaTime + ".png", tex.EncodeToPNG());
        }

        RenderTexture.active = curRendTex;
    }

}
