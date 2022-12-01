using UnityEngine;

public class ReadCSVFiles : MonoBehaviour
{

    [SerializeField] private FileList fileList;
    [SerializeField] private ActiveFile activeFile;

    private CSVReader csvReader;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        csvReader = new CSVReader(fileList, activeFile, "Log Files");
        csvReader.LoadAllFilesFromResources(); // loading all files; for now: setting first one as active and reading it in
    }
}
