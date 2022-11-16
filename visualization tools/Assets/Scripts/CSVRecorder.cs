using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVRecorder : MonoBehaviour
{
    private string filename = "";
    private bool recording = false;

    private GameObject player;

    // method that is being called after Update functions have been called
    private void LateUpdate()
    {
        if (recording)
        {
            RecordCurrent();
        }

        if (Time.frameCount == 3000) // for the time being stops after 300 frames
        {
            StopRecording();
        }
    }

    // sets up recording of csv file
    public void StartRecording()
    {
        player = GameObject.Find("Player"); // finding player in scene

        if(player == null)
        {
            Debug.LogError("There needs to be a gameobject called 'Player' in the scene! Logging not on!", this); 
            return;
        }

        filename = Application.dataPath + "/Resources/logfile.csv"; // TODO: figure out how to name properly

        // maing sure flaot numbers are stored with a "." as a comma
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        TextWriter textWriter = new StreamWriter(filename, false); // if a file with current filename exists, override it; otherwise create a new file
        textWriter.WriteLine("frame, xPos, yPos, zPos, xRot, yRot, zRot, wRot"); // csv header
        textWriter.Close();

        recording = true; // start recording
    }

    // adds current frame number as well as objects position and rotation (quaternion) into file
    private void RecordCurrent()
    {
        TextWriter textWriter = new StreamWriter(filename, true); // open file with filename and add to it
        textWriter.WriteLine($"{Time.frameCount}," +
            $"{transform.position.x}," +
            $"{transform.position.y}," +
            $"{transform.position.z}," +
            $"{transform.rotation.x}," +
            $"{transform.rotation.y}," +
            $"{transform.rotation.z}," +
            $"{transform.rotation.w}");
        textWriter.Close();
    }

    // stops recording
    public void StopRecording()
    {
        recording = false;
    }

}
