using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    //[SerializeField] ActionSO attack;
    [SerializeField] ActionSO[] attackList;

    ActionQueuer actionQueuer;
    PlayerManager playerManager;

    private ActionInstance currAttackInstance;
    int currentAtkIndex = 0;

    private void Start()
    {
        actionQueuer = GetComponent<ActionQueuer>();
        playerManager = GetComponent<PlayerManager>();
    }

    //called by player controller, simply queues attacks
    public void CueAttack(bool pressed)
    {
        if (pressed)
        {
            if (currAttackInstance != null && playerManager.GetCurrAction() == currAttackInstance) {//player is currently attacking with this basic attack
                ActionSO playingSO = currAttackInstance.actionSO;
                QueueNextAttackInSequence(playingSO);
            } else {
                QueueAttack(0);
            }
        }
        else if(currAttackInstance != null)
        {
            //skip wind when let go
            currAttackInstance.skipWindWhenQueued = true;
            playerManager.SkipWind(currAttackInstance);
        }
    }

    private bool QueueNextAttackInSequence(ActionSO currSO) {

        for (int i = 0; i < attackList.Length; i++) {
            if(currSO == attackList[i]) {
                QueueAttack((i + 1) % attackList.Length);
                return true;
            }
        }
        return false;
    }

    private void QueueAttack(int index) {
        currAttackInstance = actionQueuer.QueueAction(attackList[index]);
    }
}
