using TMPro;
using UnityEngine;

public class RecordingControl : MonoBehaviour
{
    private bool record = true;
    private CSVRecorder recorder;

    private TMP_Text buttonText;

    public void ButtonClicked()
    {
        if (record) // while be executed with first button click
        {
            recorder = GetComponent<CSVRecorder>(); // getting acces to recorder script

            buttonText = GameObject.FindGameObjectWithTag("Record Button Text").GetComponent<TMP_Text>();

            buttonText.text = "STOP";
            record = false;
            recorder.StartRecording();
        }
        else
        {
            recorder.StopRecording();
            buttonText.text = "DONE";
        }

    }

}
