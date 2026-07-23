using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public enum PlayerControlState
{
    Normal, Disabled, DodgeOnly, None
}

public class PlayerController : MonoBehaviour
{
    #region References
    CharacterMovement cMovement;
    CharacterCombat cCombat;
    #endregion

    Vector2 currentMovementInput = Vector2.zero;

    [SerializeField]private PlayerControlState state = PlayerControlState.Normal;
    private Queue< PlayerControlState> nextStates = new Queue<PlayerControlState>();

    public void SetState(PlayerControlState newState)
    {
        state = newState;
        nextStates.Clear();

        //UpdateCharacterMovementState();
    }
    public void SetStateDelayed(PlayerControlState newState, float waitDuration)
    {
        nextStates.Enqueue(newState);
        Invoke("NextState", waitDuration);
    }
    private void NextState()
    {
        if(nextStates.Count>0)
            state = nextStates.Dequeue();
        //UpdateCharacterMovementState();
    }

    //private void UpdateCharacterMovementState()
    //{
    //    cMovement.movementState = state is PlayerControlState.Normal ? CharacterMovement.MovementState.normal : CharacterMovement.MovementState.stunned;
    //}

    void Start()
    {
        RefrenceRetreival();
    }

    private void Update()
    {
        cMovement.movementInput = state is PlayerControlState.Normal ? currentMovementInput : Vector2.zero;
    }

    void RefrenceRetreival()
    {
        cMovement = GetComponent<CharacterMovement>();
        cCombat = GetComponent<CharacterCombat>();
    }

    #region Movement
    public void OnMove(InputValue value)
    {
        currentMovementInput = value.Get<Vector2>();
    }
    public void OnWalk(InputValue value)
    {
        cMovement.walk = !cMovement.walk;
        cMovement.FigureOutMovementState();
    }
    public void OnSprint(InputValue value)
    {
        cMovement.sprint = !cMovement.sprint;
        cMovement.FigureOutMovementState();
    }
    public void OnDash(InputValue value)
    {
        if (state is PlayerControlState.Disabled) return; //runs during dodgeonly state
        cMovement.Dash(currentMovementInput);
        SetState(PlayerControlState.Normal);
    }
    #endregion

    #region Combat
    public void OnAttack(InputValue value)
    {
        if (state is not PlayerControlState.Normal) return;
        cCombat.attackButton = !cCombat.attackButton;
        cCombat.AttackUpdate();
    }
    #endregion
}
