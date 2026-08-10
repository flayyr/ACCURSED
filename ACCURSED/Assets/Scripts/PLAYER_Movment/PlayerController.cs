using System;
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
    //CharacterMovement cMovement;
    CharacterCombat cCombat;
    PlayerAbilities playerAbilities;

    PlayerHitter basicAttacker;
    CombatManager combatManager;
    #endregion

    [SerializeField]private PlayerControlState state = PlayerControlState.Normal;
    private Queue< PlayerControlState> nextStates = new Queue<PlayerControlState>();

    public event Action InteractKeyPressed;


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
        //cMovement.movementInput = state is PlayerControlState.Normal ? currentMovementInput : Vector2.zero;
    }

    void RefrenceRetreival()
    {
        //cMovement = GetComponent<CharacterMovement>();
        cCombat = GetComponent<CharacterCombat>();
        playerAbilities = GetComponent<PlayerAbilities>();
        basicAttacker = GetComponent<PlayerHitter>();
        combatManager = GetComponent<CombatManager>();
    }

    #region Movement
    public void OnMove(InputValue value)
    {
        //if (state is not PlayerControlState.Normal) return;

        combatManager.MoveInput(value.Get<Vector2>());
    }
    public void OnWalk(InputValue value)
    {
        combatManager.SetWalkInput(value.isPressed);
        //cMovement.FigureOutMovementState();
    }
    public void OnSprint(InputValue value)
    {
        combatManager.SetSprintInput( value.isPressed);
        //cMovement.FigureOutMovementState();
    }
    public void OnDash(InputValue value)
    {
        if (state is PlayerControlState.Disabled || !value.isPressed) return; //runs during dodgeonly state
        combatManager.Dash();
        SetState(PlayerControlState.Normal);
    }
    #endregion

    #region Combat
    public void OnAttack(InputValue value)
    {
        if (state is not PlayerControlState.Normal && value.isPressed) return;
        //cCombat.attackButton = value.isPressed;
        //cCombat.AttackUpdate();
        basicAttacker.CueAttack(value.isPressed);
    }

    public void OnHeal(InputValue value)
    {
        if (state is not PlayerControlState.Normal) return;
        if(value.isPressed)
            playerAbilities.UseHeal();
    }

    public void OnVestige(InputValue value)
    {
        if (state is not PlayerControlState.Normal) return;
        if (value.isPressed)
            playerAbilities.UseVestige();
    }

    public void OnRememberance(InputValue value)
    {
        if (state is not PlayerControlState.Normal) return;
        if (value.isPressed)
            playerAbilities.UseRemembrance();
    }

    #endregion

    public void OnInteract(InputValue value)
    {
        if (state is PlayerControlState.Disabled) return;
        if (value.isPressed)
            InteractKeyPressed?.Invoke();
    }
}
