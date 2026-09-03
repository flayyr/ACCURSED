using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory Button")]
public class SOExeInventory : EscMenuUIButtonSO
{
    public override void Execute()
    {

        // open inventory
        EscMenuController.Instance.OpenInventory();
        EscMenuController.Instance.CloseMenu();
    }
}
