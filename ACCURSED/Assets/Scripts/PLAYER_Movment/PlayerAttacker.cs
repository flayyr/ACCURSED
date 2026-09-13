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
            ActionInstance lastPlayed = actionQueuer.GetLastPlayedInstance();
            QueueNextAttackInSequence(lastPlayed != null ? lastPlayed.actionSO : null);
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
        QueueAttack(0);
        return false;
    }

    private void QueueAttack(int index) {
        ActionInstance lastQueued = actionQueuer.GetLastQueuedInstance();
        if (lastQueued != null && attackList[index] == lastQueued.actionSO) {//dont queue new attack if target attack is already the latest queued attack
            currAttackInstance.UpdateQueueTime();
            return;
        }

        currAttackInstance = actionQueuer.QueueAction(attackList[index]);
    }
}
