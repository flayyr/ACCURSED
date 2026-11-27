using UnityEngine;

public class PlayerActionKit : MonoBehaviour  //replaces PlayerAbilities
{
    [SerializeField] ActionSO baseAttackSO;

    ActionQueuer actionQueuer;
    PlayerData pData;

    ActionInstance currAttackAction;

    private void Awake() {
        actionQueuer = GetComponent<ActionQueuer>();
        pData = GetComponent<PlayerData>();
    }

    public void CueAttack(bool isPressed) {
        if (isPressed) {
            currAttackAction = actionQueuer.QueueAction(baseAttackSO);
        } else {
            if (currAttackAction != null) {
                currAttackAction.skipWindWhenQueued = true;
            }
        }
    }
}
