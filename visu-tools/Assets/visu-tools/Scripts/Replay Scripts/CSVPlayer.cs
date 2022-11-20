using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVPlayer : MonoBehaviour
{
    private int frameIdx = 0; // which frame are we on? 

    [SerializeField] private ActiveFile activeFile; // scriptableObject that contains position and rotation information for file that should currently be replayed

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Method that is being called every frame */
    void Update()
    {
        if (frameIdx < activeFile.positions.Count - 1)
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
