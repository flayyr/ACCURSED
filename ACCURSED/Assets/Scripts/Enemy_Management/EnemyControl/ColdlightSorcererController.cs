using UnityEngine;
using static MirrorKnightController;

public class ColdlightSorcererController : EnemyController
{
    [Header("Sorcerer")]
    [SerializeField] protected float stopRange;
    protected override void PursuingUpdate() {
        if (playerTransform == null) return;

        // Aim movementInput at the player so attacks step/face the right way,
        // even on frames where we hold position.
        Vector2 toPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position);
        float distToPlayer = toPlayer.magnitude;
        toPlayer = toPlayer.normalized;

        if (distToPlayer <= attackRange) {
            bool wasIdle = false;
            // In range: face the player, hold position, and start a combo if idle.
            if (combatManager.GetCombatState() is ActionState.Idle) {
                combatManager.MoveInput(toPlayer);
                wasIdle = true;
            }

            if (attackCooldownTimer > attackCooldown) {
                enemyAttacker.CueAttack(canChangeDirMidCombo || !wasIdle);//does not continue combo if we were idle, unless we can changeDirMidCombo
                attackCooldownTimer -= attackCooldown;
            } else {
                attackCooldownTimer += Time.deltaTime;
            }
        } else {
            // Out of range: only break off if we aren't mid-combo, then close the gap.
            if (combatManager.GetCombatState() is ActionState.Idle) {
                HandleMovement();
                SetTargetPosition(playerTransform.position);
            }
        }
    }
}
