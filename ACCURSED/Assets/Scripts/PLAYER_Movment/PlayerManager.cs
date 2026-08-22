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

    PlayerDeath playerDeath;

    protected override void OnEnable()
    {
        base.OnEnable();
        playerDeath = GetComponent<PlayerDeath>();

        playerDeath.OnDeath += OnDeath;
        playerDeath.OnRevive += OnRevive;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        playerDeath.OnDeath -= OnDeath;
        playerDeath.OnRevive -= OnRevive;
    }

    protected override void EndWind()
    {
        base.EndWind();

        currAction.actionSO.PlayerActionTrigger(ref playerRef);
    }

    public void Dash()
    {
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

    public void OnStartRest()
    {
        currDir = Vector2.down;
        UpdateDirection();

        combatState = ActionState.Stunned;

        cAnim.SetResting(true);
    }

    public void OnStopRest()
    {
        combatState = ActionState.Idle;

        cAnim.SetResting(false);
    }

    private void OnDeath()
    {
        combatState = ActionState.Stunned;
        cAnim.SetDead(true);
    }

    private void OnRevive()
    {
        combatState = ActionState.Idle;
        cAnim.SetDead(false);
    }
}
