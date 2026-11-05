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
    [SerializeField] Transform playerOffset;
    [SerializeField] PlayerReference playerRef;
    [SerializeField] DashAction dashAction;
    [SerializeField] float parryGriefGain;

    PlayerDeath playerDeath;

    Vector2 toMouseDirection;

    float dashTimer;

    protected override void OnEnable()
    {
        base.OnEnable();
        playerDeath = GetComponent<PlayerDeath>();

        playerDeath.OnDeath += OnDeath;
        playerDeath.OnReviveAnimStarted += PlayReviveAnim;
        playerDeath.OnReviveAnimFinished += OnReviveFinish;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        playerDeath.OnDeath -= OnDeath;
        playerDeath.OnReviveAnimStarted -= PlayReviveAnim;
        playerDeath.OnReviveAnimFinished -= OnReviveFinish;
    }

    protected override void Update()
    {
        base.Update();

        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
        }
    }

    protected override void EndWind()
    {
        if (currAction.actionSO.GetType() == typeof(ChargeableActionSO) )
        {
            float chargeTime = Time.time - currAction.queueTime;
            ChargeableActionSO chargeableAction = (ChargeableActionSO)currAction.actionSO;
            if (chargeTime >= chargeableAction.chargeTime)
            {
                PlayActionInstantly(chargeableAction.chargedAction);
                return;
            }
        }
        
        base.EndWind();
        currAction.actionSO.PlayerActionTrigger(ref playerRef);
    }

    public override void ParrySuccess(Vector2 parryPoint, GameObject originObject)
    {
        base.ParrySuccess(parryPoint, originObject);
        playerRef.playerStats.UpdateVitality(parryGriefGain);
    }

    public void Dash()
    {
        UpdateDirection();
        cMove.Dash(currDir);
        cAnim.SetStunned(false);
        cAnim.SetMoveState(0);
        cAnim.SetDashing();
    }

    //called by player controller, skips the action queue because it needs to also be called during StunnedCancellable state
    public bool CueDash()
    {
        if ((combatState is ActionState.Idle or ActionState.StunnedCancellable) && dashTimer<=0)
        {
            dashTimer = dashAction.dashCooldown;
            PlayActionInstantly(dashAction);
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

    private void PlayReviveAnim()
    {
        currDir = Vector2.down;
        UpdateDirection();
        cAnim.SetDead(false);
        cAnim.SetStunned(false);
    }

    private void OnReviveFinish() {
        combatState = ActionState.Idle;
    }

    public void UpdateMouseDirection(Vector2 mousePosition) {
        toMouseDirection = (mousePosition - (Vector2)transform.position).normalized;
    }

    public Vector2 GetMouseDirection() {  return toMouseDirection; }
}
