using UnityEngine;

public class PlayerHitter : MonoBehaviour
{
    [SerializeField] AttackSO attack;
    AttackQueuer attackPlayer;

    bool buttonPressed;
    bool buttonHeld;
    bool buttonReleased;


    private void Start()
    {
        attackPlayer = GetComponent<AttackQueuer>();
    }

    private void Update()
    {




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
