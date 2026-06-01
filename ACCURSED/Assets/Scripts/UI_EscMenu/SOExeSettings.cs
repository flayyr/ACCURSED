using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Settings Button")]
public class SOExeSettings : EscMenuUIButtonSO
{
    public override void Execute()
    {
        // open settings
        EscMenuController.Instance.CloseMenu();
    }
}
