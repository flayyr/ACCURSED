using System;
using System.ComponentModel;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Windows;

public enum ActionState { Idle, Winding, Attacking, Stunned, StunnedCancellable }
public enum BaseMoveState { None = 0, Walk=1, Run=2, Sprint=3}

public class CharacterManager : MonoBehaviour
{
    //This script processes actions from actionQueuer. It handles both movement and combat

    [SerializeField] protected HitBox hitBox;

    protected ActionQueuer actionQueuer;
    protected CharacterAnimator cAnim;
    protected CharacterMovement cMove;

    public ActionInstance currAction = null;

    protected ActionState combatState = ActionState.Idle;

    protected float windTimer = 0;
    protected float stunTimer = 0;
    protected float stunCancelTimer = 0;

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
        cAnim.OnActionFinished += OnActionFinish;
    }

    private void OnDisable()
    {
        actionQueuer.OnActionQueued -= PlayNextAction;
        cAnim.OnActionFinished -= OnActionFinish;
    }

    private void Update()
    {
        if(combatState is ActionState.Stunned or ActionState.StunnedCancellable)
            UpdateStunTimer();
        if(combatState is ActionState.Winding)
            UpdateWindTimer();

        UpdateMovement();
    }

    #region Combat

    //all winds have a max wind time and will end once timer runs out
    protected void UpdateWindTimer()
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

    protected void UpdateStunTimer()
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
        
        if (stunTimer > 0)
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

    //event call from animator, runs when lastest action animation ends
    protected void OnActionFinish()
    {
        combatState = ActionState.Idle;
        UpdateDirection();
        PlayNextAction();
    }

    protected void PlayNextAction()
    {
        if(combatState is ActionState.Idle)//only perform actions when idle
        {
            currAction = actionQueuer.GetNextAction();//gets next action from queuer, null if queue empty

            if (currAction != null)
            {
                hitBox.SetAttackData(currAction.actionSO.attackData);//sets attack data into hitbox

                //start winding
                cAnim.SetWind(true);
                combatState = ActionState.Winding;
                windTimer = currAction.actionSO.windDuration;
                cAnim.SwitchAnimationState(currAction.actionSO.windAnimationState);

                //end wind immediately if released key early (set thru PlayerAttacker), or the action has no wind
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

        Launch(currDir, currAction.actionSO.attackData.stepAmount, true);
    }

    //gets stunned when hit, called by hurtbox
    public void Stun(float stunDuration, float timeBeforeCancellable)
    {
        if (stunDuration <= 0) return;


        combatState = ActionState.Stunned;
        cAnim.SetStunned(true);//for stun animations
        actionQueuer.ClearActions();

        currAction = null;

        if(stunTimer<stunDuration)
            stunTimer = stunDuration;
        stunCancelTimer = timeBeforeCancellable;

        if (stunCancelTimer <= 0)
        {
            combatState = ActionState.StunnedCancellable;
        }
    }
    #endregion

    #region Movement
    //moveState is only None or Run for NPCs.
    protected BaseMoveState moveState = BaseMoveState.None;

    protected Vector2 moveInput;
    protected Vector2 currDir = Vector2.down;

    protected bool walkInput;
    protected bool sprintInput;

    //called by controller scripts
    public void MoveInput(Vector2 input)
    {
        moveInput = input;

        if(moveInput != Vector2.zero)
            currDir = moveInput;

        if (combatState is ActionState.Idle or ActionState.Winding)
            UpdateDirection();
    }

    //updates animator direction
    protected void UpdateDirection()
    {
        cAnim.SetFacingDirection(currDir);
    }

    protected void UpdateMovement()
    {
        if(combatState is ActionState.Idle)
        {
            FigureOutMovementState();

            //sets move speed and animator parameter depending on movestate, mostly for player
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

    protected void FigureOutMovementState()
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

    public void Launch(Vector2 moveInput, float launchForce, bool resetVel)
    {
        cMove.Knockback(moveInput, launchForce, resetVel);
    }

    //leaving this here as reference since I haven't implemented this yet
    void IdleUpdate()
    {
        if (combatState is ActionState.Idle)
        {
            if (moveInput == Vector2.zero) // not moving
            {
                //idleTime -= Time.deltaTime; // timer for when the play the special idle animation

                //cAnimator.SetMoveState(0);

                //if (idleTime < 0) // time to play the special idle animation
                //{
                //    idleTime = timeTillSpecialIdle; // reset timer
                //    cAnimator.Play(idles[UnityEngine.Random.Range(1, idles.Count)]); // play special animation
                //    doingIdleSpecial = true;
                //}
                //else if (!cAnimator.IsCurrentState(idles[0])) // if the animation if currently not the normal idle animation
                //{
                //    if (doingIdleSpecial)
                //    {
                //        if (cAnimator.GetCurrentNormalizedTime() >= 1f)
                //        {
                //            cAnimator.Play(idles[0]);
                //            doingIdleSpecial = false;
                //        }
                //    }
                //    else
                //        cAnimator.Play(idles[0]);
                //}
            }
            else
            {
                //doingIdleSpecial = false;

                //idleTime = timeTillSpecialIdle;

                //string targetAnim = sprint ? movements[2] : walk ? movements[0] : movements[1];

                //if (!cAnimator.IsCurrentState(targetAnim))
                //    cAnimator.Play(targetAnim);

                //cAnimator.SetMoveState(sprint ? 3 : walk ? 1 : 2); //1=walk, 2=run, 3=sprint, 0=not moving
            }
        }
        else
        {
            //doingIdleSpecial = false;
        }
    }

    #endregion

    public ActionState GetCombatState() => combatState;
    public BaseMoveState GetMoveState() => moveState;
    public Vector2 GetDirection() => currDir;
}
