using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/* This script populates the scrollable list of available log files that can be replayed.*/
public class PopulateReplayList : MonoBehaviour
{

    [SerializeField] private FileList fileList;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform scrollViewContent;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Method that is being called when the script instance is being loaded */
    public void Awake()
    {
        List<string> filePaths = fileList.paths; // get list of all files that exist
        int active = fileList.activeFileNumber; // get which file is default active

        for (int i = 0; i < filePaths.Count; i++) // for each file that exits make a button that displays the name of the file
        {
            GameObject newButton = Instantiate(buttonPrefab, scrollViewContent);
            newButton.GetComponent<ListButtonControl>().SetInformation(Path.GetFileName(filePaths[i]), i);
            if (i == active)
            {
                newButton.GetComponent<Button>().Select(); // make button appear selected if it represents the curretnly active file
            }
        }
    }

    /* Generates one default button that just says that there are no files. */
    public void SetNoFilesButton()
    {
        GameObject newButton = Instantiate(buttonPrefab, scrollViewContent);
        newButton.GetComponent<ListButtonControl>().SetInformation("No files found.", -1);
    }


}
