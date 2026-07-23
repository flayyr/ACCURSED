using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Quit Button")]
public class SOExeQuit : EscMenuUIButtonSO
{
    private Transform player;
    private string playerTag = "Player";

    private void Awake()
    {
        if (player == null)
            FindPlayerTarget();
    }

    private void FindPlayerTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
            player = playerObject.transform;
    }

    public override void Execute()
    {
        EscMenuController.Instance.CloseMenu();

        // quit to main menu
        RoomTransitionManager.Instance.BeginTransition("StartMenu", "back", player);
    }
}
