using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneTransitionHandler : MonoBehaviour
{
    [Header("Scene Name")]
    public string startScreenName = "StartMenu";
    
    public void ChangeScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != startScreenName)
        {
            SceneManager.LoadScene(startScreenName);
            return;
        }
    }
}
