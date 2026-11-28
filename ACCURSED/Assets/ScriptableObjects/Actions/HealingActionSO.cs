using UnityEngine;

[CreateAssetMenu(fileName = "HealingAction", menuName = "Actions/HealingAction")]
public class HealingActionSO : ActionSO
{
    [Header("Healing")]
    public float healingInvincibleDuration = 1f;

    public override void PlayerActionTrigger(ref PlayerReference playerRef) {
        base.PlayerActionTrigger(ref playerRef);

        if (playerRef.playerStats != null) { 
            if (playerRef.playerStats.UseHealCharge()) {
                playerRef.hurtBox.InvincibleForSeconds(healingInvincibleDuration);
            }
        }

        if (playerRef.playerData != null) {
            if (playerRef.playerData.UseHealCharge()) {
                playerRef.hurtBox.InvincibleForSeconds(healingInvincibleDuration);
            }
        }
    }
}
