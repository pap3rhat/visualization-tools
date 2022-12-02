using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Loads all avilable lof files and initiates replay*/
public class ReadCSVFiles : MonoBehaviour
{

    [SerializeField] private FileList fileList;
    [SerializeField] private ActiveFile activeFile;

    [SerializeField] private GameObject replayButton;
    [SerializeField] private TMP_Text replayText;

    private CSVReader csvReader;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        csvReader = new CSVReader(fileList, activeFile);
        csvReader.LoadAllFiles(true, false); // loading all files; for now: setting first one as active but not reading it

        if (fileList.paths.Count() == 0)
        {
            replayButton.GetComponent<Button>().interactable = false; // disabling button if there are no files
            GetComponent<PopulateReplayList>().SetNoFilesButton(); // setting "error" message
            // making button text look disabled
            replayText.GetComponent<TMP_Text>().color = new Color(0.05098039f, 0.07450981f, 0.09019608f);
            replayText.GetComponent<TMP_Text>().enableVertexGradient = false;
        }
    }


    /* Initiates the replay.*/
    public void InitReplay()
    {
        csvReader.ReadActiveFile(); // reading in information of file that should be replayed
        SceneManager.LoadScene("Replay World"); // changing scene to replay world
    }
}
