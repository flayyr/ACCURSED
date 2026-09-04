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

    ActionInstance latestIntance;

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

        latestIntance = instance;

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
            latestIntance = null;
            return null;
        }

        ActionInstance nextInstance= actionQueue.Dequeue();

        if (nextInstance == latestIntance)
            latestIntance = null;

        return nextInstance;
    }

    public void ClearActions()
    {
        actionQueue.Clear();
        latestIntance = null;
    }

    public ActionInstance GetLatestAction() { return latestIntance; }
}
