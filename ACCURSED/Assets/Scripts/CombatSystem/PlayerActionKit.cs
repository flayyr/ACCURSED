using UnityEngine;

public class PlayerActionKit : MonoBehaviour  //replaces PlayerAbilities
{
    [SerializeField] ActionSO baseAttackSO;
    [SerializeField] ActionSO dashActionSO;
    [SerializeField] HealingActionSO healingActionSO;
    [SerializeField] VestigeSO vestigeSO;
    [SerializeField] RemembranceSO remembranceSO;

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

    public void CueDash() {
        actionQueuer.QueueAction(dashActionSO);
    }
}
