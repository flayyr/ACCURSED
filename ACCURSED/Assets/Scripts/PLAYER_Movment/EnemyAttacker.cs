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
    public void CueAttack(bool continueCombo)
    {
        if (continueCombo)
        {
            ActionInstance lastPlayed = actionQueuer.GetLastPlayedInstance();
            QueueNextAttackInSequence(lastPlayed != null ? lastPlayed.actionSO : null);
        } else
        {
            QueueAttack(0);
        }
    }

    private bool QueueNextAttackInSequence(ActionSO currSO)
    {
        for (int i = 0; i < attackList.Length; i++)
        {
            if (currSO == attackList[i])
            {
                QueueAttack((i + 1));// % attackList.Length
                return true;
            }
        }
        QueueAttack(0);
        return false;
    }

    private void QueueAttack(int index)
    {
        if (index >= attackList.Length)
        {
            return;
        }

        //ActionInstance lastQueued = actionQueuer.GetLastQueuedInstance();
        //if (lastQueued != null && attackList[index] == lastQueued.actionSO)
        //{//dont queue new attack if target attack is already the latest queued attack
        //    lastQueued.UpdateQueueTime();
        //    Debug.Log(lastQueued.actionSO.actionName);
        //    return;
        //}

        currAttackInstance = actionQueuer.QueueAction(attackList[index]);
    }
}
