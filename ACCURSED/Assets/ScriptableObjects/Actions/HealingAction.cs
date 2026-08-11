using UnityEngine;

[CreateAssetMenu(fileName = "HealingAction", menuName = "Actions/HealingAction")]
public class HealingAction : ActionSO
{
    [Header("Healing")]
    public float healingInvincibleDuration = 1f;

    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        if (playerRef.playerStats.UseHealCharge())
        {
            playerRef.hurtBox.InvincibleForSeconds(healingInvincibleDuration);
        }
    }
}
