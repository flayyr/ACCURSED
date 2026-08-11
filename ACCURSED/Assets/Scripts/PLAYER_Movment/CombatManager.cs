using System.ComponentModel;
using UnityEngine;

public enum CombatState { Idle, Winding, Attacking, Stunned, StunnedCancellable }
public enum BaseMoveState { None = 0, Walk=1, Run=2, Sprint=3}
public class CombatManager : MonoBehaviour
{
    AttackQueuer attackQueuer;
    CharacterAnimator cAnim;
    CharacterMovement cMove;

    public AttackInstance currAttack = null;

    [SerializeField] CombatState combatState = CombatState.Idle;

    float windTimer = 0;
    float stunTimer = 0;
    float stunCancelTimer = 0;

    private void Awake()
    {
        attackQueuer = GetComponent<AttackQueuer>();
        cAnim = GetComponent<CharacterAnimator>();
        cMove = GetComponent<CharacterMovement>();
    }

    private void OnEnable()
    {
        combatState = CombatState.Idle;
        currAttack = null;
        windTimer = 0;

        attackQueuer.OnAttackQueued += PlayNextAttack;
        cAnim.OnAttackFinished += OnAttackFinish;
        //cMove.OnFinishDash += DashFinished;
    }

    private void OnDisable()
    {
        attackQueuer.OnAttackQueued -= PlayNextAttack;
        cAnim.OnAttackFinished -= OnAttackFinish;
        //cMove.OnFinishDash -= DashFinished;
    }

    private void Update()
    {
        if(combatState is CombatState.Stunned or CombatState.StunnedCancellable)
            UpdateStunTimer();
        if(combatState is CombatState.Winding)
            UpdateWindTimer();

        UpdateMovement();
    }

    void UpdateWindTimer()
    {
        if (windTimer > 0)
        {
            windTimer -= Time.deltaTime;
            if (windTimer <= 0)
            {
                windTimer = 0;
                cAnim.SetWind(false);
                combatState = CombatState.Attacking;
            }
        }
    }

    void UpdateStunTimer()
    {
        if (stunCancelTimer > 0)
        {
            stunCancelTimer -= Time.deltaTime;
            if(stunCancelTimer <= 0)
            {
                stunCancelTimer = 0;
                combatState = CombatState.StunnedCancellable;
            }
        }
        else if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                stunTimer = 0;
                cAnim.SetStunned(false);
                combatState = CombatState.Idle;
            }
        }
    }

    void OnAttackFinish()
    {
        combatState = CombatState.Idle;
        //currAttack = null;
        PlayNextAttack();
    }

    private void PlayNextAttack()
    {
        if(combatState is CombatState.Idle)
        {
            currAttack = attackQueuer.NextAttack();

            if (currAttack != null)
            {
                cAnim.SetWind(true);

                combatState = CombatState.Winding;
                windTimer = currAttack.attackSO.windDuration;

                cAnim.SwitchAnimationState(currAttack.attackSO.windAnimationState);

                if (currAttack.skipWindWhenQueued)
                {
                    SkipWind(currAttack);
                }

                return;
            }
        }
    }

    public void SkipWind(AttackInstance attackInstance)
    {
        if (attackInstance != currAttack || combatState is not CombatState.Winding) return; //make sure currAttack is the same instance to be skipped

        windTimer = 0;
        cAnim.SetWind(false);
        combatState = CombatState.Attacking;
    }

    public void Stun(float stunDuration, float timeBeforeCancellable)
    {
        combatState = CombatState.Stunned;
        cAnim.SetStunned(true);

        currAttack = null;

        stunTimer = stunDuration;
        stunCancelTimer = timeBeforeCancellable;
    }


    //Movement

    [SerializeField] BaseMoveState moveState = BaseMoveState.None;
    //bool dashing = false;

    Vector2 moveInput;
    bool walkInput;
    bool sprintInput;

    public void MoveInput(Vector2 input)
    {
        moveInput = input;
        if(combatState is CombatState.Idle or CombatState.Winding)
            cAnim.SetFacingDirection(moveInput);
    }

    void UpdateMovement()
    {
        if(combatState is CombatState.Idle /*&& !dashing*/)
        {
            FigureOutMovementState();

            cMove.SetMoveSpeed(moveState);
            cAnim.SetMoveState((int)moveState);
            cMove.BaseMove(moveInput);
        }
        else
        {
            moveState = BaseMoveState.None;
            cAnim.SetMoveState(0);
        }
    }


    public void SetWalkInput(bool walk)
    {
        walkInput = !walkInput;
    }
    public void SetSprintInput(bool sprint)
    {
        sprintInput = sprint;
    }

    void FigureOutMovementState()
    { 
        if(moveInput == Vector2.zero)
        {
            moveState = BaseMoveState.None;
            return;
        }

        if (sprintInput)
        {
            moveState = BaseMoveState.Sprint;
        }
        else
        {
            if (walkInput)
            {
                moveState = BaseMoveState.Walk;
            }
            else
            {
                moveState = BaseMoveState.Run;
            }
        }
    }

    public void Dash()
    {
        if (combatState is CombatState.Idle or CombatState.StunnedCancellable && moveInput != Vector2.zero)
        {
            combatState = CombatState.Idle;
            cAnim.SetFacingDirection(moveInput);
            cMove.Dash(moveInput);
            cAnim.SetStunned(false);
        }
    }

    public CombatState GetCombatState() => combatState;
    public BaseMoveState GetMoveState() => moveState;
}
