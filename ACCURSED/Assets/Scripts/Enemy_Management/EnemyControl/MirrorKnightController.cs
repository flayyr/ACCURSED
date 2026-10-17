using UnityEngine;
using SaintsField.Playa;
using SaintsField;
using System.Security.Claims;

public class MirrorKnightController : EnemyController
{
    public enum ShieldState { Up=0, Down, Raising, Lowering }

    [LayoutStart("Mirror Knight", ELayout.FoldoutBox)]
    [SerializeField] float shieldRange;
    [SerializeField][ReadOnly] ShieldState shieldState;
    [SerializeField] ActionSO shieldUpAction;
    [SerializeField] ActionSO shieldDownAction;

    ActionInstance currShieldAction;

    protected override void Start() {
        base.Start();
        SetShieldState(ShieldState.Down);
    }

    protected override void PursuingUpdate() {
        if (playerTransform == null) return;

        // Aim movementInput at the player so attacks step/face the right way,
        // even on frames where we hold position.
        Vector2 toPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position);
        float distToPlayer = toPlayer.magnitude;
        toPlayer = toPlayer.normalized;

        if (distToPlayer <= shieldRange) {
            //RAISE SHIELD
            if (shieldState is ShieldState.Down) {
                SetShieldState(ShieldState.Raising);
                currShieldAction = actionQueuer.QueueAction(shieldUpAction, RaiseShield);
            } else if (shieldState == ShieldState.Raising) {
                if (!currShieldAction.interrupted) {
                    //keep updating action instance while raising
                    currShieldAction.UpdateQueueTime();
                } else {
                    //shieldAction was interrupted, so drop the shield back down
                    SetShieldState(ShieldState.Down);
                }
            }
        } else {
            //LOWER SHIELD
            if (shieldState is ShieldState.Up) {
                SetShieldState(ShieldState.Lowering);
                currShieldAction = actionQueuer.QueueAction(shieldDownAction, LowerShield);
            } else if (shieldState == ShieldState.Lowering) {
                if (!currShieldAction.interrupted) {
                    currShieldAction.UpdateQueueTime();
                } else {
                    //Drop the shield here even though lowering is interrupted
                    //because I assume shield should be dropped when stunned
                    SetShieldState(ShieldState.Down);
                }
            }
        }

        combatManager.SetWalkInput(shieldState is not ShieldState.Down);

        if (distToPlayer <= attackRange) {
            bool wasIdle = false;
            // In range: face the player, hold position, and start a combo if idle.
            if (combatManager.GetCombatState() is ActionState.Idle) {
                currDir = toPlayer;
                combatManager.MoveInput(toPlayer);
                wasIdle = true;
            }

            enemyAttacker.CueAttack(canChangeDirMidCombo || !wasIdle);//does not continue combo if we were idle, unless we can changeDirMidCombo
        } else {
            // Out of range: only break off if we aren't mid-combo, then close the gap.
            if (combatManager.GetCombatState() is ActionState.Idle) {
                HandleMovement();
                SetTargetPosition(playerTransform.position);
            }
        }
    }

    void RaiseShield() {
        SetShieldState(ShieldState.Up); 
        currShieldAction = null; 
    }
    void LowerShield() {
        SetShieldState(ShieldState.Down);
        currShieldAction = null;
    }

    void SetShieldState(ShieldState newState) {
        if (shieldState == newState) return;

        shieldState = newState;
        switch (shieldState) {
            case ShieldState.Up:
                break;
            case ShieldState.Down:
                break;
            case ShieldState.Raising:
                break;
            case ShieldState.Lowering:
                break;
        }
    }
}
