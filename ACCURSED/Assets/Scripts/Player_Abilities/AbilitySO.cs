using UnityEngine;

public class AbilitySO : ActionSO
{
    [Header("Ability")]
    public Sprite abilityIcon;
    [SerializeField] float invincibleDuration;

    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        base.PlayerActionTrigger(ref playerRef);
        playerRef.hurtBox.InvincibleForSeconds(invincibleDuration);
    }
}
