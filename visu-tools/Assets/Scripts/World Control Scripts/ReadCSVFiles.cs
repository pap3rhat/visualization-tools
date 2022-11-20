using UnityEngine;

public class ReadCSVFiles : MonoBehaviour
{

    [SerializeField] private FileList fileList;
    [SerializeField] private ActiveFile activeFile;
    [SerializeField] private string subfolderPath;

    private CSVReader csvReader;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        csvReader = new CSVReader(fileList, activeFile, subfolderPath);
        csvReader.LoadAllFilesFromResources(); // loading all files; for now: setting first one as active and reading it in
    }
}
