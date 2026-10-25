using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionInstance
{
    public delegate void FunctionCall();

    public ActionSO actionSO;
    public float queueTime;
    public bool skipWindWhenQueued = false;
    public bool loaded = false;
    public float finishTime = -1;
    public float startTime = -1;
    public float loadTime = -1;
    public bool interrupted = false;
    public FunctionCall onFinish;

    public ActionInstance(ActionSO actionSO, float currTime, FunctionCall func=null)
    {
        this.actionSO = actionSO;
        queueTime = currTime;
        skipWindWhenQueued = false;
        onFinish = func;
    }

    public void UpdateQueueTime() {
        queueTime = Time.time;
    }
}

public class ActionQueuer : MonoBehaviour
{

    public Action OnActionQueued;

    [SerializeField] float queueWindow = 0f;

    Queue<ActionInstance> actionQueue;

    ActionInstance lastQueuedInstance;
    ActionInstance lastPlayedInstance;

    private void Awake()
    {
        actionQueue = new Queue<ActionInstance>();
    }

    public ActionInstance QueueAction(ActionSO attack, ActionInstance.FunctionCall func = null)
    {
        ActionInstance instance = new ActionInstance(attack, Time.time, func);
        actionQueue.Enqueue(instance);
        lastQueuedInstance = instance;

        //broadcasts event for character manager, which will process the next attack
        OnActionQueued?.Invoke();

        return instance;
    }
    public ActionInstance QueueAction(ActionSO attack, ref bool condition, ActionInstance.FunctionCall func = null) {
        if (condition) {
            return QueueAction(attack, func);
        }
        return null;
    }

    public ActionInstance GetNextAction()
    {
        //clear actions that were queued too early
        while(actionQueue.Count > 0 && actionQueue.Peek().queueTime + queueWindow < Time.time)
        {
            actionQueue.Dequeue().interrupted = true;
        }

        //if no actions in queue
        if (actionQueue.Count == 0)
        {
            if(lastQueuedInstance != null && !lastQueuedInstance.loaded)//if latest wasnt played, meaning it was dequeued
                lastQueuedInstance = null;
            return null;
        }

        lastPlayedInstance = actionQueue.Dequeue();
        return lastPlayedInstance;
    }

    public void ClearActions()
    {
        while (actionQueue.Count != 0) {
            actionQueue.Dequeue().interrupted = true;
        }

        if(lastPlayedInstance!=null)
            lastPlayedInstance.interrupted = true;

        lastQueuedInstance = null;
        lastPlayedInstance = null;
    }

    public ActionInstance GetLastQueuedInstance() {
        if (lastQueuedInstance == null)
            return null;

        if(lastQueuedInstance.finishTime != -1) {//latest instance had finished playing;
            if (lastQueuedInstance.finishTime + queueWindow < Time.time) {//it finished too long ago to be chained
                lastQueuedInstance = null;
                return null;
            }
        }

        return lastQueuedInstance; 
    }

    public ActionInstance GetLastPlayedInstance() {
        if (lastPlayedInstance == null)
            return null;

        if (lastPlayedInstance.finishTime != -1) {//latest instance had finished playing;
            if (lastPlayedInstance.finishTime + queueWindow < Time.time) {//it finished too long ago to be chained
                lastPlayedInstance = null;
                return null;
            }
        }

        return lastPlayedInstance;
    }
}
