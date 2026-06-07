using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EscapeMenuHandler : MonoBehaviour
{
    [Header("Scene Name")]
    public string startScreenName = "StartMenu";

    public void Update()
    {
        HandleEscapeInput();
    }

    private void HandleEscapeInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != startScreenName)
        {
            if (PlayerPrefs.HasKey("LastScene"))
            {
                string previousScene = PlayerPrefs.GetString("LastScene");
                SceneManager.LoadScene(previousScene);
                return;
            }
            else
            {
                SceneManager.LoadScene(startScreenName);
                return;
            }
        }
    }
}
