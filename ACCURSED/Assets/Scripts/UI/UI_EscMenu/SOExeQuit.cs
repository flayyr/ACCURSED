using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Quit Button")]
public class SOExeQuit : EscMenuUIButtonSO
{
    public override void Execute()
    {
        EscMenuController.Instance.CloseMenu();

        // quit to main menu
        RoomTransitionWithoutPlayer.Instance.BeginTransition("StartMenu");
    }
}
