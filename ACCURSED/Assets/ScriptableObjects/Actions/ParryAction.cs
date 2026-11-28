using UnityEngine;

[CreateAssetMenu(fileName = "ParryAction", menuName = "Actions/ParryAction")]
public class ParryAction : ActionSO
{
    [Header("Parry")]
    public float parryDuration = .3f;

    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        Vector2 direction = Vector2.zero;
        if(playerRef.playerManager !=null) {
            direction = playerRef.playerManager.GetFacingDirection();
        } 
        if(playerRef.playerData != null) {
            direction = playerRef.playerData.facingDirection;
        }
        playerRef.hurtBox.Parry(parryDuration, direction);
    }
}
