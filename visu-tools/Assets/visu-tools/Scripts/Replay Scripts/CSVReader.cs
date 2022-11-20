using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


/* Script that contains functions to read in csv files containing player movement data.
 * CSV file has to fullfill the following criteria:
 * - The first row needs to contain a header describing the attributes listed in the CSV file.
 * - The header need to contain at least the following attributes: 'pos_x', 'pos_y', 'pos_z', 'quat_x', 'quat_y', 'quat_z', 'quat_w'
 * - Every other row in the CSV file needs to represent a frame. The frames have to be in sequence (row 1 -> frame 1, row 2 -> frame 2, ..., row n -> frame n)
 * -> Every row (execept the top one) clearly indicates a position and rotation of an object (eg a player) for a given frame. Replaying them in sequence shows the correct recorded object movement.
 */
public class CSVReader
{
    private FileList fileList; // scriptable object containing a list of all available csv files
    private ActiveFile activeFile; // scriptable object containing information from a specific specific file

    private string subfolderPath; // path after Resources/ where CSV files are locted; if emtpy files are just in Resources

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- CONSTRUCTOR ---

    // construct without specific subfolder -> files are just in Resources folder
    // WARNING: There cannot be other textfiles in a Resources folder in that case. Be aware of that if you for example use TextMesh Pro!
    public CSVReader(FileList fileList, ActiveFile activeFile)
    {
        this.fileList = fileList;
        this.activeFile = activeFile;
    }

    // construct with specific subfolder -> files are in Resources/subfolderPath
    public CSVReader(FileList fileList, ActiveFile activeFile, string subfolderPath)
    {
        this.fileList = fileList;
        this.activeFile = activeFile;
        this.subfolderPath = subfolderPath;
    }


    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- METHODS TO BE CALLED FROM OTHER PLACES ---

    /* Method that loads all CSV files from Resources/subfolderPath
     * setFirst: controls if first CSV file in folder should be set as active file; default: true
     * readFirst: controls if information from first file in folder should be read (only makes sense if setFirst is true); default: true
     * For readFirst it makes sense to set it to false if for example user gets shown a list of all available files first before deciding for one -> less information has to be processed in that case
     */
    public void LoadAllFilesFromResources(bool setFirst = true, bool readFirst = true)
    {
        // loading all CSV files as textAssets
        if (subfolderPath != null)
        {
            fileList.files = Resources.LoadAll<TextAsset>(subfolderPath).ToList();
        }
        else
        {
            fileList.files = Resources.LoadAll("", typeof(TextAsset)).Cast<TextAsset>().ToList();
        }

        // setting up activeFile if wanted
        if (setFirst && fileList.files.Count > 0)
        {
            SetActiveFile(0, readFirst);
        }
    }

    /* Method that initializes fileName and fileNumber of activeFIle scriptable object.
     * index: which file form fileList scriptable object should be set active; WARNING: there has to ba a fileList scriptable object with correct information already!
     * read: controls if activeFile should be read in already
     */
    public void SetActiveFile(int index, bool read)
    {
        // init activeFile
        activeFile.fileName = fileList.files[index].name;
        activeFile.fileNumber = index;

        // read in file of wanted
        if (read)
        {
            ReadActiveFile();
        }
    }

    /* Method that reads in positions and rotations information froma activeFile
     * Should not be called if not necessary; quite expensive
     */
    public void ReadActiveFile()
    {
        // reading all rows from current file
        string file = fileList.files[activeFile.fileNumber].text;
        string[] rows = file.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries); // reading all rows and removing emtpy rows

        // reading header and checking in which columns 'key attributes' are located
        string[] header = rows[0].Split(new char[] { ',' });
        IDictionary<string, int> extractPositions = new Dictionary<string, int>();
        for (int i = 0; i < header.Length; i++)
        {
            header[i] = Regex.Replace(header[i], @"\s+", "");

            if (String.Equals(header[i], "pos_x") || header[i].Equals("pos_y") || header[i].Equals("pos_z")
                || header[i].Equals("quat_x") || header[i].Equals("quat_y") || header[i].Equals("quat_z") || header[i].Equals("quat_w"))
            {
                extractPositions.Add(header[i], i);
            }
        }

        // init ActiveFile lists
        activeFile.positions = new List<Vector3>();
        activeFile.quaternions = new List<Quaternion>();

        // going over all rows in csv file and saving postions and rotations in ActiveFIle scriptable object
        for (int i = 1; i < rows.Length; i++)
        {
            string[] currentRow = rows[i].Split(new char[] { ',' });
            activeFile.positions.Add(new Vector3(float.Parse(currentRow[extractPositions["pos_x"]], CultureInfo.InvariantCulture),
                                                 float.Parse(currentRow[extractPositions["pos_y"]], CultureInfo.InvariantCulture),
                                                 float.Parse(currentRow[extractPositions["pos_z"]], CultureInfo.InvariantCulture)));
            activeFile.quaternions.Add(new Quaternion(float.Parse(currentRow[extractPositions["quat_x"]], CultureInfo.InvariantCulture),
                                                      float.Parse(currentRow[extractPositions["quat_y"]], CultureInfo.InvariantCulture),
                                                      float.Parse(currentRow[extractPositions["quat_z"]], CultureInfo.InvariantCulture),
                                                      float.Parse(currentRow[extractPositions["quat_w"]], CultureInfo.InvariantCulture)));
        }

    }
}
