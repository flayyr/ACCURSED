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

public class ActionQueuer : MonoBehaviour
{

    public Action OnActionQueued;

    [SerializeField] float queueWindow = 0f;

    Queue<ActionInstance> actionQueue;

    private void Awake()
    {
        actionQueue = new Queue<ActionInstance>();
    }

    public ActionInstance QueueAction(ActionSO attack)
    {
        ActionInstance instance = new ActionInstance(attack, Time.time);
        actionQueue.Enqueue(instance);

        //broadcasts event for character manager, which will process the next attack
        OnActionQueued?.Invoke();

        return instance;
    }

    public ActionInstance GetNextAction()
    {
        //clear actions that were queued too early
        while(actionQueue.Count > 0 && actionQueue.Peek().queueTime + queueWindow < Time.time)
        {
            actionQueue.Dequeue();
        }

        //if no actions in queue
        if (actionQueue.Count == 0)
        {
            return null;
        }

        return actionQueue.Dequeue();
    }

    public void ClearActions()
    {
        actionQueue.Clear();
    }
}
