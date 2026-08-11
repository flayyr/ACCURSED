using UnityEngine;

public class PlayerHitter : MonoBehaviour
{
    [SerializeField] ActionSO attack;
    ActionQueuer attackQueuer;
    CharacterManager combatManager;

    bool buttonPressed;
    bool buttonHeld;
    bool buttonReleased;

    private ActionInstance currAttackInstance;

    private void Start()
    {
        attackQueuer = GetComponent<ActionQueuer>();
        combatManager = GetComponent<CharacterManager>();
    }

    private void Update()
    {
        if (buttonPressed)
        {
            currAttackInstance = attackQueuer.QueueAction(attack);
        }

        if (buttonReleased)
        {
            currAttackInstance.skipWindWhenQueued = true;
            combatManager.SkipWind(currAttackInstance);
        }

        //pressed and released is only true for one frame
        buttonPressed = false;
        buttonReleased = false;
    }

    //called by player controller
    public void CueAttack(bool pressed)
    {
        buttonPressed = pressed;
        buttonHeld = pressed;
        buttonReleased = !pressed;
    }
}
