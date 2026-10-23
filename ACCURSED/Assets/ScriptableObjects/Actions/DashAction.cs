using System.Security.Claims;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAction", menuName = "Actions/DashAction")]
public class DashAction : ActionSO
{
    [Header("Dash Action")]
    [SerializeField] public float dashCooldown;
    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        playerRef.playerManager.Dash();
    }
}
