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
            string currentSceneName = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("LastScene", currentSceneName);
            PlayerPrefs.Save();

            SceneManager.LoadScene(startScreenName);
            return;
        }
    }
}
