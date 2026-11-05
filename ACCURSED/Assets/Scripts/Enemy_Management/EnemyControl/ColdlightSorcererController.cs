using UnityEngine;
using static MirrorKnightController;

public class ColdlightSorcererController : EnemyController
{
    [Header("Sorcerer")]
    [SerializeField] protected float teleportRange;
    [SerializeField] protected float teleportAfterDuration;
    [SerializeField] protected float teleportAfterDamage;
    [SerializeField] private ActionSO teleportAction;

    float teleportTimer;
    float hpSinceLastTeleport;

    CharacterStatistics cStats;

    protected override void Start() {
        base.Start();
        cStats = GetComponent<CharacterStatistics>();
    }
    protected override void PursuingUpdate() {
        if (playerTransform == null) return;

        // Aim movementInput at the player so attacks step/face the right way,
        // even on frames where we hold position.
        Vector2 toPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position);
        float distToPlayer = toPlayer.magnitude;
        toPlayer = toPlayer.normalized;

        if(distToPlayer < teleportRange) {
            teleportTimer += Time.deltaTime;
            if(teleportTimer > teleportAfterDuration)
                Teleport();
        } else {
            teleportTimer = 0;
        }

        if (hpSinceLastTeleport - cStats.GetHP() >= teleportAfterDamage) {
            Teleport();
        }
        

        if (distToPlayer <= attackRange) {
            cManager.SetWalkInput(true);//set to walking if within attack range, stops enemy from walking when on cooldown
            bool wasIdle = false;
            // In range: face the player, hold position, and start a combo if idle.
            if (cManager.GetCombatState() is ActionState.Idle) {
                cManager.MoveInput(toPlayer);
                wasIdle = true;
            }

            if (attackCooldownTimer > attackCooldown) {
                enemyAttacker.CueAttack(canChangeDirMidCombo || !wasIdle);//does not continue combo if we were idle, unless we can changeDirMidCombo
                attackCooldownTimer -= attackCooldown;
            } else {
                attackCooldownTimer += Time.deltaTime;
            }
        } else {
            cManager.SetWalkInput(false);
            // Out of range: only break off if we aren't mid-combo, then close the gap.
            if (cManager.GetCombatState() is ActionState.Idle) {
                HandleMovement();
                SetTargetPosition(playerTransform.position);
            }
        }
    }

    private void Teleport() {
        hpSinceLastTeleport = cStats.GetHP();
        teleportTimer = 0;
        actionQueuer.QueueAction(teleportAction);
    }
}
