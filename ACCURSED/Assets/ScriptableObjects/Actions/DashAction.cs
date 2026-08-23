using System.Security.Claims;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAction", menuName = "Actions/DashAction")]
public class DashAction : ActionSO
{
    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        playerRef.playerManager.Dash();
    }
}
