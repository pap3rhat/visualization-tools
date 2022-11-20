using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplayControl : MonoBehaviour
{
    private bool playing = false; // do not start playing yet
    private bool pause = false; // do not pause in beginning

    private CSVPlayer csvPlayer;
    private TMP_Text playPauseButtonText;
    private Button stopButton;

    private void Awake()
    {
        csvPlayer = GameObject.Find("Player").GetComponent<CSVPlayer>(); // getting csvScript reference of of player gameObject
        playPauseButtonText = GameObject.Find("Play/Pause Button Text").GetComponent<TMP_Text>(); // getting reference to text on play/pause button
        stopButton = GameObject.Find("Stop Button").GetComponent<Button>();
        stopButton.gameObject.SetActive(false);
    }

    public void PlayPauseButtonClicked()
    {
        if (!playing) // will be executed if replay was not started yet (or after it was started again after stopping it)
        {
            playing = true; // starting replay
            csvPlayer.StartReplay();

            playPauseButtonText.text = "PAUSE";
            stopButton.gameObject.SetActive(true);
        }
        else if (!pause) // will be executed if user wants to pause replay
        {
            pause = true;
            csvPlayer.PauseReplay();

            playPauseButtonText.text = "RESUME";
        }
        else // will be executed if user wants to resume replay
        {
            pause = false;
            csvPlayer.ResumeReplay();

            playPauseButtonText.text = "PAUSE";
        }
    }

    public void StopButtonCLicked()
    {
        playing = false;
        csvPlayer.StopReplay();
        stopButton.gameObject.SetActive(false);
        playPauseButtonText.text = "START";
    }
}
