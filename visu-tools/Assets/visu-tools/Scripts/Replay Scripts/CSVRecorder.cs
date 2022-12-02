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

    [Tooltip("Move gameobject whos movement is to be recorded in here.")] [SerializeField] private GameObject recorde;
    [Tooltip("Where should the files be saved? If empty 'Application.persistentDataPath' is used")] [SerializeField] private string filePath;

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
        if (filePath != null && !filePath.Equals(""))
        {
            filename = filePath + Path.DirectorySeparatorChar + "logfile" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".csv";
        }
        else
        {
            filename = Application.persistentDataPath + Path.DirectorySeparatorChar + "logfile" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".csv";
        }


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
            $"{recorde.transform.position.x}," +
            $"{recorde.transform.position.y}," +
            $"{recorde.transform.position.z}," +
            $"{recorde.transform.rotation.x}," +
            $"{recorde.transform.rotation.y}," +
            $"{recorde.transform.rotation.z}," +
            $"{recorde.transform.rotation.w}");
        textWriter.Close();
    }

    /* Method that stops recording */
    public void StopRecording()
    {
        recording = false;
    }

}
