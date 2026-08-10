using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] AttackSO attack;
    AttackQueuer attackQueuer;

    private void Start()
    {
        attackQueuer = GetComponent<AttackQueuer>();
    }

    //called by enemy controller
    public void CueAttack()
    {
        attackQueuer.QueueAttack(attack);
    }
}
