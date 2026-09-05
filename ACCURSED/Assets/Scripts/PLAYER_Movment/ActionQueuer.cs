using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionInstance
{
    public ActionSO actionSO;
    public float queueTime;
    public bool skipWindWhenQueued = false;
    public bool played = false;
    public float finishTime = -1;

    public ActionInstance(ActionSO actionSO, float currTime)
    {
        this.actionSO = actionSO;
        queueTime = currTime;
        skipWindWhenQueued = false;
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

    public ActionInstance QueueAction(ActionSO attack)
    {
        ActionInstance instance = new ActionInstance(attack, Time.time);
        actionQueue.Enqueue(instance);
        lastQueuedInstance = instance;

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
            if(!lastQueuedInstance.played)//if latest wasnt played, meaning it was dequeued
                lastQueuedInstance = null;
            return null;
        }

        lastPlayedInstance = actionQueue.Dequeue();
        return lastPlayedInstance;
    }

    public void ClearActions()
    {
        actionQueue.Clear();
        if (!lastQueuedInstance.played)//if latest wasnt played, meaning it was dequeued
            lastQueuedInstance = null;
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
