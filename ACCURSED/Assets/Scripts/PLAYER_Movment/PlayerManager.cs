using System;
using MoreMountains.Feedbacks;
using UnityEngine;

[Serializable]
public struct PlayerReference
{
    public ParticleSystem particleSystem;
    public SpriteRenderer spriteRenderer;
    public HurtBox hurtBox;
    public HitBox hitBox;
    public PlayerStatistics playerStats;
    public PlayerManager playerManager;
    public MMF_Player hitFeedback;
}

public class PlayerManager : CharacterManager
{
    [SerializeField] PlayerReference playerRef;
    [SerializeField] ActionSO dashAction;

    protected override void EndWind()
    {
        base.EndWind();

        currAction.actionSO.PlayerActionTrigger(ref playerRef);
    }

    public void Dash()
    {
        //combatState = ActionState.Idle;
        UpdateDirection();
        cMove.Dash(moveInput);
        cAnim.SetStunned(false);
        cAnim.SetMoveState(0);
        cAnim.SetDashing();
    }

    //called by player controller, skips the action queue because it needs to also be called during StunnedCancellable state
    public bool CueDash()
    {
        if ((combatState is ActionState.Idle or ActionState.StunnedCancellable) && moveInput != Vector2.zero)
        {
            currAction = new ActionInstance(dashAction, Time.time);
            PlayCurrentAction();
            return true;
        }
        return false;
    }
}
