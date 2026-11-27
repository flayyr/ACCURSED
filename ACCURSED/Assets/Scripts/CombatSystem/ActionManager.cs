using UnityEngine;

public class ActionManager : MonoBehaviour
{
    ActionQueuer actionQueuer;
    CharacterData cData;
    CharacterMovementNew cMove;
    CharacterAnimatorNew cAnimator;

    ActionInstance currentAction;


    float windTimer;

    private void Awake() {
        actionQueuer = GetComponent<ActionQueuer>();
        cData = GetComponent<CharacterData>();
        cMove = GetComponent<CharacterMovementNew>();
        cAnimator = GetComponent<CharacterAnimatorNew>();

        actionQueuer.OnActionQueued += GetNextAction;
        cAnimator.OnActionFinished += EndAction;
    }

    private void Update() {
        if (windTimer > 0) {
            windTimer -= Time.deltaTime;
            if (windTimer < 0 || currentAction.skipWindWhenQueued) {
                windTimer = 0;
                EndWind();
            }
        }
    }

    private void GetNextAction() {
        if (!cData.isIdle)
            return;

        currentAction = actionQueuer.GetNextAction();
        if (currentAction != null) {
            PerformAction();
        }
    }

    private void GetNextAction(ActionSO previousAction) {
        if (!cData.isIdle)
            return;

        currentAction = actionQueuer.GetNextAction();
        if (currentAction != null) {
            currentAction.actionSO = currentAction.actionSO.GetChainedAction(previousAction);
            PerformAction();
        }
    }

    private void PerformAction() {
        currentAction.loaded = true;
        currentAction.loadTime = Time.time;

        SetUpWinding();

        cAnimator.SetAnimationState(currentAction.actionSO.windAnimationState);
        cData.actionState = ActionState.Acting;
    }

    private void EndAction() {
        currentAction.finishTime = Time.time;
        currentAction.onFinish?.Invoke();

        cData.actionState = ActionState.Idle;

        GetNextAction(currentAction.actionSO);
    }

    private void SetUpWinding() {
        bool winding = currentAction.actionSO.useWind && !currentAction.skipWindWhenQueued;
        cAnimator.SetWind(winding);
        if (winding) {
            windTimer = currentAction.actionSO.windDuration;
        }
    }

    private void EndWind() {
        cAnimator.SetWind(false);
    }

}
