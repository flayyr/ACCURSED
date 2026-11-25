using UnityEngine;

public class PlayerActionKit : MonoBehaviour  //replaces PlayerAbilities
{
    [SerializeField] ActionSO baseAttackSO;

    ActionQueuer actionQueuer;
    PlayerData pData;

    private void Awake() {
        actionQueuer = GetComponent<ActionQueuer>();
        pData = GetComponent<PlayerData>();
    }

    public void CueAttack(bool isPressed) {
        if (isPressed) {
            actionQueuer.QueueAction(baseAttackSO);
        } else {

        }
    }
}
