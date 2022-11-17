using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // loads scene given its name
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
