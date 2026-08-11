using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Windows;

public enum ActionState { Idle, Winding, Attacking, Stunned, StunnedCancellable }
public enum BaseMoveState { None = 0, Walk=1, Run=2, Sprint=3}

public class CharacterManager : MonoBehaviour
{
    [SerializeField] DashAction dashAction;
    [SerializeField] HitBox hitBox;

    ActionQueuer actionQueuer;
    CharacterAnimator cAnim;
    CharacterMovement cMove;

    public ActionInstance currAction = null;

    [SerializeField] ActionState combatState = ActionState.Idle;

    float windTimer = 0;
    float stunTimer = 0;
    float stunCancelTimer = 0;

    private void Awake()
    {
        actionQueuer = GetComponent<ActionQueuer>();
        cAnim = GetComponent<CharacterAnimator>();
        cMove = GetComponent<CharacterMovement>();
    }

    private void OnEnable()
    {
        combatState = ActionState.Idle;
        currAction = null;
        windTimer = 0;

        actionQueuer.OnActionQueued += PlayNextAction;
        cAnim.OnAttackFinished += OnAttackFinish;
        //cMove.OnFinishDash += DashFinished;
    }

    private void OnDisable()
    {
        actionQueuer.OnActionQueued -= PlayNextAction;
        cAnim.OnAttackFinished -= OnAttackFinish;
        //cMove.OnFinishDash -= DashFinished;
    }

    private void Update()
    {
        if(combatState is ActionState.Stunned or ActionState.StunnedCancellable)
            UpdateStunTimer();
        if(combatState is ActionState.Winding)
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
                EndWind();
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
                combatState = ActionState.StunnedCancellable;
            }
        }
        else if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                stunTimer = 0;
                cAnim.SetStunned(false);
                combatState = ActionState.Idle;
            }
        }
    }

    void OnAttackFinish()
    {
        combatState = ActionState.Idle;
        UpdateDirection();
        PlayNextAction();
    }

    private void PlayNextAction()
    {
        if(combatState is ActionState.Idle)
        {
            currAction = actionQueuer.NextAction();

            if (currAction != null)
            {
                hitBox.SetAttackSO(currAction.actionSO);

                cAnim.SetWind(true);

                combatState = ActionState.Winding;
                windTimer = currAction.actionSO.windDuration;

                cAnim.SwitchAnimationState(currAction.actionSO.windAnimationState);

                if (currAction.skipWindWhenQueued || windTimer<=0)
                {
                    EndWind();
                }

                return;
            }
        }
    }

    public void SkipWind(ActionInstance attackInstance)
    {
        if (attackInstance != currAction || combatState is not ActionState.Winding) return; //make sure currAttack is the same instance to be skipped

        EndWind();
    }

    protected virtual void EndWind()
    {
        windTimer = 0;
        cAnim.SetWind(false);
        combatState = ActionState.Attacking;

        cMove.AttackForwardStep(currDir, currAction.actionSO.stepAmount);
    }

    public void Stun(float stunDuration, float timeBeforeCancellable)
    {
        combatState = ActionState.Stunned;
        cAnim.SetStunned(true);
        actionQueuer.ClearActions();

        currAction = null;

        stunTimer = stunDuration;
        stunCancelTimer = timeBeforeCancellable;
    }


    //Movement

    [SerializeField] BaseMoveState moveState = BaseMoveState.None;
    //bool dashing = false;

    Vector2 moveInput;
    Vector2 currDir = Vector2.down;

    bool walkInput;
    bool sprintInput;

    public void MoveInput(Vector2 input)
    {
        moveInput = input;
        if(combatState is ActionState.Idle or ActionState.Winding)
            UpdateDirection();
    }

    private void UpdateDirection()
    {
        if (moveInput == Vector2.zero) return;

        currDir = moveInput;
        cAnim.SetFacingDirection(currDir);
    }

    void UpdateMovement()
    {
        if(combatState is ActionState.Idle /*&& !dashing*/)
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
        combatState = ActionState.Idle;
        UpdateDirection();
        cMove.Dash(moveInput);
        cAnim.SetStunned(false);
    }

    public bool CueDash()
    {
        if (combatState is ActionState.Idle or ActionState.StunnedCancellable && moveInput != Vector2.zero)
        {
            actionQueuer.QueueAction(dashAction);
            return true;
        }
        return false;
    }

    public ActionState GetCombatState() => combatState;
    public BaseMoveState GetMoveState() => moveState;
}
