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

    protected override void EndWind()
    {
        base.EndWind();

        currAction.actionSO.PlayerActionTrigger(ref playerRef);
    }

    //called by player controller, queues a dash action. Ideally manager doesnt queue actions, but I'll allow dashing
    public bool CueDash()
    {
        if ((combatState is ActionState.Idle or ActionState.StunnedCancellable) && moveInput != Vector2.zero)
        {
            combatState = ActionState.Idle;
            UpdateDirection();
            cMove.Dash(moveInput);
            cAnim.SetStunned(false);
            cAnim.SetMoveState(0);
            cAnim.SetDashing();
            return true;
        }
        return false;
    }
}
