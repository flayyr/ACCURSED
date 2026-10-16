using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] ActionSO[] attackList;
    ActionQueuer actionQueuer;

    int currComboIndex;
    ActionInstance currAttackInstance;

    private void Start()
    {
        actionQueuer = GetComponent<ActionQueuer>();
        currComboIndex = 0;
    }

    //called by enemy controller, simply queues attacks
    public void CueAttack()
    {
        //if (currAttackInstance != null)
        //{

        //}
        //currAttackInstance = actionQueuer.QueueAction(attackList[0]);

        ActionInstance lastPlayed = actionQueuer.GetLastPlayedInstance();
        QueueNextAttackInSequence(lastPlayed != null ? lastPlayed.actionSO : null);
    }

    private bool QueueNextAttackInSequence(ActionSO currSO)
    {
        for (int i = 0; i < attackList.Length; i++)
        {
            if (currSO == attackList[i])
            {
                Debug.Log("list: "+i);
                QueueAttack((i + 1) % attackList.Length);
                return true;
            }
        }
        Debug.Log("default");
        QueueAttack(0);
        return false;
    }

    private void QueueAttack(int index)
    {
        ActionInstance lastQueued = actionQueuer.GetLastQueuedInstance();
        if (lastQueued != null && attackList[index] == lastQueued.actionSO)
        {//dont queue new attack if target attack is already the latest queued attack
            currAttackInstance.UpdateQueueTime();
            return;
        }

        currAttackInstance = actionQueuer.QueueAction(attackList[index]);
    }
}
