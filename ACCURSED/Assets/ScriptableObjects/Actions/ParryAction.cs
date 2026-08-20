using UnityEngine;

[CreateAssetMenu(fileName = "ParryAction", menuName = "Actions/ParryAction")]
public class ParryAction : ActionSO
{
    [Header("Parry")]
    public float parryTotalDuration = 1f;
    public float perfectParryWindow = .3f;

    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        playerRef.hurtBox.Parry(parryTotalDuration, perfectParryWindow);
    }
}
