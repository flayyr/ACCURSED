using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public enum PlayerControlState
{
    Normal, Disabled, None
}

public class PlayerController : MonoBehaviour
{
    #region References
    PlayerAbilities playerAbilities;

    PlayerAttacker basicAttacker;
    PlayerManager playerManager;
    #endregion

    [SerializeField]private PlayerControlState state = PlayerControlState.Normal;

    public event Action InteractKeyPressed;


    public void SetState(PlayerControlState newState)
    {
        state = newState;
    }

    void Start()
    {
        RefrenceRetreival();
    }

    void RefrenceRetreival()
    {
        playerAbilities = GetComponent<PlayerAbilities>();
        basicAttacker = GetComponent<PlayerAttacker>();
        playerManager = GetComponent<PlayerManager>();
    }

    #region Movement
    public void OnMove(InputValue value)
    {
        playerManager.MoveInput(value.Get<Vector2>());
    }
    public void OnWalk(InputValue value)
    {
        playerManager.SetWalkInput(value.isPressed);
    }
    public void OnSprint(InputValue value)
    {
        playerManager.SetSprintInput( value.isPressed);
    }
    public void OnDash(InputValue value)
    {
        if (state is PlayerControlState.Disabled || !value.isPressed) return; //runs during dodgeonly state
        
        if(playerManager.CueDash())
            SetState(PlayerControlState.Normal);
    }
    #endregion

    #region Combat
    public void OnAttack(InputValue value)
    {
        if (state is not PlayerControlState.Normal && value.isPressed) return;
        basicAttacker.CueAttack(value.isPressed);
    }

    public void OnParry(InputValue value)
    {
        if (state is not PlayerControlState.Normal) return;

        if (value.isPressed)
            playerAbilities.UseParry();
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
