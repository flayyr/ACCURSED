using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Quit Button")]
public class SOExeQuit : EscMenuUIButtonSO
{
    public override void Execute()
    {
        EscMenuController.Instance.CloseMenu();

        Time.timeScale = 1f;

        if (RoomTransitionManager.Instance == null)
        {
            Debug.LogError("SOExeQuit: No RoomTransitionManager exists.");
            return;
        }

        RoomTransitionManager.Instance.BeginTransition("StartMenu");
    }

}
