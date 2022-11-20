using System.Collections.Generic;
using UnityEngine;


/* This scriptable object holds all the information which is necessary to replay playermovement from specific csv file. */

// [CreateAssetMenu(fileName = "Active File", menuName = "OnlyOneOfEach/ActiveFile")]
public class ActiveFile : ScriptableObject
{
    public string fileName;
    public int fileNumber;
    public List<Vector3> positions;
    public List<Quaternion> quaternions;
}
