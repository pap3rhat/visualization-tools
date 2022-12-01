using UnityEngine;
using System.IO;
using System;

/* Script that records positions and rotations of GameObject called player.
 * Can be used if no other way of generating log-files exists in programm.
 * WARNING: Your csv files have to be recorded within fixedUpdate!
 */
public class CSVRecorder : MonoBehaviour
{
    private string filename = "";
    private bool recording = false; // do not start with recording yet

    private GameObject player;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Method that is being called every fixed frame-rate frame */
    private void FixedUpdate()
    {
        if (recording)
        {
            RecordCurrent();
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- METHODS TO BE CALLED FROM OTHER PLACES (EG BUTTONS) ---

    /*  Method that sets up recording of csv file */
    public void StartRecording()
    {
        player = GameObject.Find("Player"); // finding player in scene

        if (player == null)
        {
            Debug.LogError("There needs to be a gameobject called 'Player' in the scene! Logging not on!", this);
            return;
        }

        filename = Application.dataPath + "/Resources/Log Files/logfile" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".csv";

        // maing sure flaot numbers are stored with a "." as a comma
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        TextWriter textWriter = new StreamWriter(filename, false); // if a file with current filename exists, override it; otherwise create a new file
        textWriter.WriteLine("frame,pos_x,pos_y,pos_z,quat_x,quat_y,quat_z,quat_w"); // csv header
        textWriter.Close();

        recording = true; // start recording
    }

    // adds current frame number as well as objects position and rotation (quaternion) into file
    private void RecordCurrent()
    {
        TextWriter textWriter = new StreamWriter(filename, true); // open file with filename and add to it
        textWriter.WriteLine($"{Time.frameCount}," +
            $"{player.transform.position.x}," +
            $"{player.transform.position.y}," +
            $"{player.transform.position.z}," +
            $"{player.transform.rotation.x}," +
            $"{player.transform.rotation.y}," +
            $"{player.transform.rotation.z}," +
            $"{player.transform.rotation.w}");
        textWriter.Close();
    }

    /* Method that stops recording */
    public void StopRecording()
    {
        recording = false;
    }

}
