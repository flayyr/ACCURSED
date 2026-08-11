using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionInstance
{
    public ActionSO actionSO;
    public float queueTime;
    public bool skipWindWhenQueued;

    public ActionInstance(ActionSO actionSO, float currTime)
    {
        this.actionSO = actionSO;
        queueTime = currTime;
        skipWindWhenQueued = false;
    }
}

//processes attack, including special abilities
public class ActionQueuer : MonoBehaviour
{

    public Action OnActionQueued;

    [SerializeField] float queueWindow = 0f;

    Queue<ActionInstance> actionQueue;

    //CharacterAnimator cAnim;

    private void Awake()
    {
        actionQueue = new Queue<ActionInstance>();
        //cAnim = GetComponent<CharacterAnimator>();
    }

    public ActionInstance QueueAction(ActionSO attack)
    {
        ActionInstance instance = new ActionInstance(attack, Time.time);
        actionQueue.Enqueue(instance);

        OnActionQueued?.Invoke();

        return instance;
    }

    public ActionInstance NextAction()
    {
        //clear attacks that were queued too early
        while(actionQueue.Count > 0 && actionQueue.Peek().queueTime + queueWindow < Time.time)
        {
            actionQueue.Dequeue();
        }

        if (actionQueue.Count == 0)
        {
            //state = CombatState.Idle;
            return null;
        }

        return actionQueue.Dequeue();
    }


    public void ClearActions()
    {
        actionQueue.Clear();
    }

    //public void SkipWind(AttackInstance attackInstance)
    //{
    //    if (attackInstance.queueTime != currAttack.queueTime) return; //make sure currAttack is the same instance to be skipped


    //}
}
