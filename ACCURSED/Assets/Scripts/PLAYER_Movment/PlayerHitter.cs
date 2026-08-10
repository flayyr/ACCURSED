using UnityEngine;

public class PlayerHitter : MonoBehaviour
{
    [SerializeField] AttackSO attack;
    AttackQueuer attackQueuer;
    CombatManager combatManager;

    bool buttonPressed;
    bool buttonHeld;
    bool buttonReleased;

    private AttackInstance currAttackInstance;

    private void Start()
    {
        attackQueuer = GetComponent<AttackQueuer>();
        combatManager = GetComponent<CombatManager>();
    }

    private void Update()
    {
        if (buttonPressed)
        {
            Debug.Log("pressed button");
            currAttackInstance = attackQueuer.QueueAttack(attack);
        }

        if (buttonReleased)
        {
            Debug.Log("released button");
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
