using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    protected ActionQueuer actionQueuer;
    protected CharacterData cData;
    protected CharacterMovementNew cMove;
    protected CharacterAnimatorNew cAnimator;


    float windTimer;

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
    }

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
        if (winding) {
            windTimer = cData.currentAction.actionSO.windDuration;
        } else {
            EndWind();
        }
    }

    protected virtual void EndWind() {
        cAnimator.SetWind(false);
        cData.currentAction.actionSO.GeneralActionTrigger(ref cData.characterRef);
    }

}
