using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] ActionSO attack;
    ActionQueuer attackQueuer;

    private void Start()
    {
        attackQueuer = GetComponent<ActionQueuer>();
    }

    //called by enemy controller
    public void CueAttack()
    {
        attackQueuer.QueueAction(attack);
    }
}
