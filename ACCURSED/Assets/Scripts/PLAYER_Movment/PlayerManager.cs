using System;
using UnityEngine;

[Serializable]
public struct PlayerReference
{
    public ParticleSystem particleSystem;
    public SpriteRenderer spriteRenderer;
    public HurtBox hurtBox;
    public PlayerStatistics playerStats;
    public PlayerManager playerManager;
}

public class PlayerManager : CharacterManager
{
    [SerializeField] PlayerReference playerRef;
    [SerializeField] DashAction dashAction;

    protected override void EndWind()
    {
        base.EndWind();

        currAction.actionSO.PlayerActionTrigger(ref playerRef);
    }

    //gets triggered by dash actionSO
    public void Dash()
    {
        //dashing causes troubles with the queue system, so I clear it just in case of bugs
        //actionQueuer.ClearActions();

        combatState = ActionState.Idle;
        UpdateDirection();
        cMove.Dash(moveInput);
        cAnim.SetStunned(false);
    }

    //called by player controller, queues a dash action. Ideally manager doesnt queue actions, but I'll allow dashing
    public bool CueDash()
    {
        if (combatState is ActionState.Idle or ActionState.StunnedCancellable && moveInput != Vector2.zero)
        {
            actionQueuer.QueueAction(dashAction);
            return true;
        }
        return false;
    }
}
