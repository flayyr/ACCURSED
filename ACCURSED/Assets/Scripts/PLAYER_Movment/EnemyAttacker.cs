using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] ActionSO[] attackList;
    [SerializeField] bool pickRandomAttack;
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
        if (pickRandomAttack) {
            QueueAttack(Random.Range(0, attackList.Length));
            return;
        }

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

        currAttackInstance = actionQueuer.QueueAction(attackList[index]);
    } 
}
