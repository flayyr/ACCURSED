using UnityEngine;

[CreateAssetMenu(fileName = "ParryAction", menuName = "Actions/ParryAction")]
public class ParryAction : ActionSO
{
    [Header("Parry")]
    public float parryDuration = .3f;

    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        playerRef.hurtBox.Parry(parryDuration, playerRef.playerManager.GetDirection());
    }
}
