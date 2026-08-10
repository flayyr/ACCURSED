using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackInstance
{
    public AttackSO attackSO;
    public float queueTime;
    public bool skipWindWhenQueued;

    public AttackInstance(AttackSO attackSO, float currTime)
    {
        this.attackSO = attackSO;
        queueTime = currTime;
        skipWindWhenQueued = false;
    }
}

//processes attack, including special abilities
public class AttackQueuer : MonoBehaviour
{

    public Action OnAttackQueued;

    [SerializeField] float queueWindow = 0f;

    Queue<AttackInstance> attackQueue;

    //CharacterAnimator cAnim;

    private void Awake()
    {
        attackQueue = new Queue<AttackInstance>();
        //cAnim = GetComponent<CharacterAnimator>();
    }

    public AttackInstance QueueAttack(AttackSO attack)
    {
        AttackInstance instance = new AttackInstance(attack, Time.time);
        attackQueue.Enqueue(instance);

        OnAttackQueued?.Invoke();

        return instance;
    }

    public AttackInstance NextAttack()
    {
        //clear attacks that were queued too early
        while(attackQueue.Count > 0 && attackQueue.Peek().queueTime + queueWindow < Time.time)
        {
            attackQueue.Dequeue();
        }

        if (attackQueue.Count == 0)
        {
            //state = CombatState.Idle;
            return null;
        }

        return attackQueue.Dequeue();
    }


    public void ClearAttacks()
    {
        attackQueue.Clear();
    }

    //public void SkipWind(AttackInstance attackInstance)
    //{
    //    if (attackInstance.queueTime != currAttack.queueTime) return; //make sure currAttack is the same instance to be skipped


    //}
}
