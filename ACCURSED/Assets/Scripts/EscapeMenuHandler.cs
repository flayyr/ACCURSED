using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EscapeMenuHandler : MonoBehaviour
{
    [Header("Scene Names")]
    public string startScreenName = "StartScene";

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
            SceneManager.LoadScene(startScreenName);
            return;
        }
    }

}
