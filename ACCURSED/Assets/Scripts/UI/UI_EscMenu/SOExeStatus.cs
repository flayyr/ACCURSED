using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Status Button")]
public class SOExeStatus : EscMenuUIButtonSO
{
    public override void Execute()
    {
        // show status

        EscMenuController.Instance.OpenStatus();
        EscMenuController.Instance.CloseMenu();
    }
}
