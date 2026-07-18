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

        Debug.Log(PlayerPrefs.GetString("LastScene"));
    }

    private void HandleEscapeInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != startScreenName)
        {
            RoomTransitionWithoutPlayer.Instance.BeginTransition(startScreenName);
            return;
        }
    }
}
