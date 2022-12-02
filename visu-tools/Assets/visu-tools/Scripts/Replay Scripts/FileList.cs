using System.Collections.Generic;
using UnityEngine;


/* This scriptable object holds all information about available files to choose from and which file is chosen. */
//[CreateAssetMenu(fileName = "File List", menuName = "Replay/FileList")]
public class FileList : ScriptableObject
{
    public List<string> paths;
    public int activeFileNumber;
    public string activeFileName;
}
