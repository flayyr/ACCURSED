using UnityEngine;

[CreateAssetMenu(fileName = "DashAction", menuName = "Actions/DashAction")]
public class DashAction : ActionSO
{
    public override void Trigger(ref PlayerReference playerRef)
    {
        playerRef.playerManager.Dash();
    }
}
