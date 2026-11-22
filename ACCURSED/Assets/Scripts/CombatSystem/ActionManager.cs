using UnityEngine;

public class ActionManager : MonoBehaviour
{
    ActionQueuer actionQueuer;
    CharacterData cData;
    CharacterMovementNew cMove;
    CharacterAnimatorNew cAnimator;

    ActionInstance currentAction;

    private void Awake() {
        actionQueuer = GetComponent<ActionQueuer>();
        cData = GetComponent<CharacterData>();
        cMove = GetComponent<CharacterMovementNew>();
        cAnimator = GetComponent<CharacterAnimatorNew>();
    }

    private void GetNextAction() {
        currentAction = actionQueuer.GetNextAction();
    }

    private void PerformAction() {
        if (currentAction == null) {
            return;
        }


    }

}
