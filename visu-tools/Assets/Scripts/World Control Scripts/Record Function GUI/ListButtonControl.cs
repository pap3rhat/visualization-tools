using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* Controls how a button looks in the scrollable file view and sets currently active file information */
public class ListButtonControl : MonoBehaviour
{
    [SerializeField] private ActiveFile activeFile;

    [SerializeField] private TMP_Text text;
    private string fileName;
    private int fileNumber;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Changes the text of the button */
    private void ChangeText(string textString)
    {
        text.text = textString;
    }

    /* Set information which file this button represents.*/
    public void SetInformation(string name, int number)
    {
        fileName = name;
        fileNumber = number;

        ChangeText(name);
    }

    /* Sets the active file to the file that is represnted by this button */
    public void OnClick()
    {
        activeFile.fileName = fileName;
        activeFile.fileNumber = fileNumber;
    }
}
