using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] ActionSO attack;
    ActionQueuer actionQueuer;
    CharacterManager combatManager;

    private ActionInstance currAttackInstance;

    private void Start()
    {
        actionQueuer = GetComponent<ActionQueuer>();
        combatManager = GetComponent<CharacterManager>();
    }

    //called by player controller, simply queues attacks
    public void CueAttack(bool pressed)
    {
        if (pressed)
        {
            currAttackInstance = actionQueuer.QueueAction(attack);
        }
        else if(currAttackInstance != null)
        {
            //skip wind when let go
            currAttackInstance.skipWindWhenQueued = true;
            combatManager.SkipWind(currAttackInstance);
        }
    }
}
