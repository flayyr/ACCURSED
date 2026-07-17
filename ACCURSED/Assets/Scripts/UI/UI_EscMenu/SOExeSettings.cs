using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Settings Button")]
public class SOExeSettings : EscMenuUIButtonSO
{
    public override void Execute()
    {
        EscMenuController.Instance.CloseMenu();

        // open settings
        
    }
}
