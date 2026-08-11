using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] ActionSO attack;
    ActionQueuer actionQueuer;

    private void Start()
    {
        actionQueuer = GetComponent<ActionQueuer>();
    }

    //called by enemy controller, simply queues attacks
    public void CueAttack()
    {
        actionQueuer.QueueAction(attack);
    }
}
