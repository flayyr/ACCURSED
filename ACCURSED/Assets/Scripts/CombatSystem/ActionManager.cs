using System.Security.Claims;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class ActionManager : MonoBehaviour
{
    protected ActionQueuer actionQueuer;
    protected CharacterData cData;
    protected CharacterMovementNew cMove;
    protected CharacterAnimatorNew cAnimator;

    protected float windTimer;
    protected float stunTimer = 0;
    protected float stunCancelTimer = 0;
    protected float staggerTimer = 0;
    [Header("Stagger")]
    [SerializeField] protected float staggerDuration;

    protected virtual void Awake() {
        actionQueuer = GetComponent<ActionQueuer>();
        cData = GetComponent<CharacterData>();
        cMove = GetComponent<CharacterMovementNew>();
        cAnimator = GetComponent<CharacterAnimatorNew>();

        actionQueuer.OnActionQueued += GetNextAction;
        cAnimator.OnActionFinished += EndAction;
    }

    private void Update() {
        UpdateWind();
        UpdateStunTimer();
    }

    #region action management
    private void UpdateWind() {
        if (windTimer > 0) {
            windTimer -= Time.deltaTime;
            if (windTimer < 0 || cData.currentAction.skipWindWhenQueued) {
                windTimer = 0;
                EndWind();
            }
        }
    }

    private void GetNextAction() {
        if (!cData.isIdle)
            return;

        cData.currentAction = actionQueuer.GetNextAction();
        if (cData.currentAction != null) {
            PerformAction();
        }
    }
    private void GetNextAction(ActionSO previousAction) {
        if (!cData.isIdle)
            return;

        cData.currentAction = actionQueuer.GetNextAction();
        if (cData.currentAction != null) {
            cData.currentAction.actionSO = cData.currentAction.actionSO.GetChainedAction(previousAction);
            PerformAction();
        }
    }

    public void PlayActionInstantly(ActionSO actionSO, ActionInstance.FunctionCall func = null) {
        InterruptCurrentAction();
        cAnimator.SetStunned(false);
        cData.actionState = ActionState.Idle;

        cData.currentAction = new ActionInstance(actionSO, Time.time, func);
        PerformAction();
    }

    private void PerformAction() {
        if (cData.currentAction.actionSO.allowDirectionUpdate) {
            cData.UpdateCurrDirection();
        }

        cData.currentAction.loaded = true;
        cData.currentAction.loadTime = Time.time;

        SetUpWinding();

        cAnimator.SetAnimationState(cData.currentAction.actionSO.windAnimationState);
        cData.actionState = ActionState.Acting;
    }

    private void EndAction() {
        cData.currentAction.finishTime = Time.time;
        cData.currentAction.onFinish?.Invoke();

        cData.actionState = ActionState.Idle;

        GetNextAction(cData.currentAction.actionSO);
    }

    private void SetUpWinding() {
        bool winding = cData.currentAction.actionSO.useWind && !cData.currentAction.skipWindWhenQueued;
        cAnimator.SetWind(winding);
        if (winding && cData.currentAction.actionSO.windDuration>0) {
            windTimer = cData.currentAction.actionSO.windDuration;
        } else {
            EndWind();
        }
    }

    protected virtual void EndWind() {
        cAnimator.SetWind(false);
        cData.currentAction.actionSO.GeneralActionTrigger(ref cData.characterRef);
    }
    #endregion

    #region Stunning
    //gets stunned when hit, called by hurtbox
    public void Stun(float stunDuration, float timeBeforeCancellable) {
        if (stunDuration <= 0) return;


        cData.actionState = ActionState.Stunned;
        cAnimator.SetStunned(true);//for stun animations

        InterruptCurrentAction();

        if (stunTimer < stunDuration)
            stunTimer = stunDuration;
        stunCancelTimer = timeBeforeCancellable;

        if (stunCancelTimer <= 0) {
            cData.actionState = ActionState.StunnedCancellable;
        }
    }

    public void Stagger(float staggerValue) {
        if (staggerValue <= 0 || staggerDuration <= 0) return;

        cData.actionState = ActionState.Stunned;
        cAnimator.SetStagger(true);//for stagger animations

        InterruptCurrentAction();

        staggerTimer = staggerDuration;
    }

    private void InterruptCurrentAction() {
        actionQueuer.ClearActions();
        cData.characterRef.hitBox.gameObject.SetActive(false);
        if (cData.currentAction != null) {
            cData.currentAction.interrupted = true;
            cData.currentAction = null;
        }
    }

    protected void UpdateStunTimer() {
        if (stunCancelTimer > 0) {
            stunCancelTimer -= Time.deltaTime;
            if (stunCancelTimer <= 0) {
                stunCancelTimer = 0;
                cData.actionState = ActionState.StunnedCancellable;
            }
        }

        if (stunTimer > 0) {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0) {
                stunTimer = 0;
                cAnimator.SetStunned(false);
                cData.actionState = ActionState.Idle;
            }
        }

        if (staggerTimer > 0) {
            staggerTimer -= Time.deltaTime;
            if (staggerTimer <= 0) {
                staggerTimer = 0;
                cAnimator.SetStagger(false);
                cData.actionState = ActionState.Idle;
            }
        }
    }
    #endregion

    //need to rethink how knockback should work
    public void Launch(Vector2 moveInput, float launchForce, bool resetVel) {
        //cMove.Knockback(moveInput, launchForce, resetVel);
    }

}
