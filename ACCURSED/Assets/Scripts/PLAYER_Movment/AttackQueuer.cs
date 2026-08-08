using System;
using System.Collections.Generic;
using UnityEngine;

//processes attack, including special abilities
public class AttackQueuer : MonoBehaviour
{
    public class AttackInstance
    {
        public AttackSO attackSO;
        public float queueTime;

        public AttackInstance(AttackSO attackSO, float currTime)
        {
            this.attackSO = attackSO;
            queueTime = currTime;
        }
    }

    public Action OnWindQueued;
    public Action OnWindFinish;

    [SerializeField] float queueWindow = 1f;

    public AttackInstance currAttack = null;

    Queue<AttackInstance> attackQueue;

    CharacterAnimator cAnim;

    private void Awake()
    {
        attackQueue = new Queue<AttackInstance>();
        cAnim = GetComponent<CharacterAnimator>();
    }

    public AttackInstance QueueAttack(AttackSO attack)
    {
        AttackInstance instance = new AttackInstance(attack, Time.time);
        attackQueue.Enqueue(instance);

        

        return instance;
    }

    public void NextAttack()
    {
        //clear attacks that were queued too early
        while(attackQueue.Count > 0 && attackQueue.Peek().queueTime + queueWindow < Time.time)
        {
            attackQueue.Dequeue();
        }

        if (attackQueue.Count == 0)
        {
            //state = CombatState.Idle;
            return;
        }

        currAttack = attackQueue.Dequeue();

        PlayWind();
    }

    private void PlayWind()
    {
        //state = CombatState.Winding;

        AttackSO currSO = currAttack.attackSO;

        float windDuration = cAnim.Play(currSO.windAnimationState, currSO.animatorController);
        windDuration = Mathf.Max(windDuration, currSO.windDuration);
    }

    private void PlayAttack()
    {
        //state = CombatState.Attacking;
        
    }


    public void ClearAttacks()
    {
        attackQueue.Clear();
    }

    public void SkipWind(AttackInstance attackInstance)
    {
        if (attackInstance.queueTime != currAttack.queueTime) return; //make sure currAttack is the same instance to be skipped


    }
}
