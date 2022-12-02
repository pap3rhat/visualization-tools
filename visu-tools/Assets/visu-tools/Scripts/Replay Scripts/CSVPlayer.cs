using UnityEngine;

/* Script that contains methods to replay/pause/resume/stop replay information in active file scriptable object.
 * WARNING: In order to be frame rate independet the positions and rotations logged within the csv file (whos information is stored in sctive file scriptable object) hat to be recorded within fixedUpdate method!
 */
public class CSVPlayer : MonoBehaviour
{
    private int frameIdx = 0; // which frame are we on? 
    private bool start = false; // don't start replay unless it is specifically asked for
    private bool pause = false; // don't pause

    [Tooltip("Move scriptable object named 'Active File' in here.")] [SerializeField] private ActiveFile activeFile; // scriptableObject that contains position and rotation information for file that should currently be replayed

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- METHOD THAT NEEDS TO BE CALLED FROM ELSEWHERE TO CONTROL REPLAY ---

    /* Method that starts the replay */
    public void StartReplay()
    {
        start = true;
        frameIdx = 0;
    }

    /* Method that pauses the replay */
    public void PauseReplay()
    {
        pause = true;
    }

    /* Method that resumes replay after it was pasued. */
    public void ResumeReplay()
    {
        pause = false;
    }

    /* Method that stops the replay
     * In contrary to PauseReplay this method resets the replay to the beginning! 
     */
    public void StopReplay()
    {
        start = false;
        pause = false;
        frameIdx = 0;
    }

    /* Checks if the replay is still running.
     * Returns true iff the replay is still running
     * Returns fals iff the replay is done
     */
    public bool CheckRunning()
    {
        return start && frameIdx < activeFile.positions.Count - 1;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Method that is being called every fixed frame-rate frame */
    private void FixedUpdate()
    {
        if (start && frameIdx < activeFile.positions.Count - 1 && !pause)
        {
            SetPosition(frameIdx);
        }

    }

    /* Method that updates position and rotation of object on which this script is attached */
    private void SetPosition(int idx)
    {
        frameIdx = idx + 1;
        transform.position = activeFile.positions[idx];
        transform.rotation = activeFile.quaternions[idx];
    }
}
