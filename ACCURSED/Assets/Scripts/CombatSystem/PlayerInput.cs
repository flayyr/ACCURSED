using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour //replacing PlayerController
{
    #region References
    //PlayerAbilities playerAbilities;

    //PlayerAttacker basicAttacker;
    //PlayerManager playerManager;
    PlayerData pData;
    #endregion

    public event Action InteractKeyPressed;

    void Start() {
        RefrenceRetreival();
    }

    void RefrenceRetreival() {
        //playerAbilities = GetComponent<PlayerAbilities>();
        //basicAttacker = GetComponent<PlayerAttacker>();
        //playerManager = GetComponent<PlayerManager>();
        pData = GetComponent<PlayerData>();
    }

    #region Movement
    public void OnMove(InputValue value) {
        //playerManager.MoveInput(state == PlayerControlState.Normal ? value.Get<Vector2>() : Vector2.zero);
        pData.SetMoveInputDirection(pData.controlEnabled ? value.Get<Vector2>().normalized : Vector2.zero);
    }
    public void OnWalk(InputValue value) {
        if (!pData.controlEnabled) return;
        //playerManager.SetWalkInput(value.isPressed);
    }
    public void OnSprint(InputValue value) {
        if (!pData.controlEnabled) return;
        //playerManager.SetSprintInput(value.isPressed);
    }
    public void OnDash(InputValue value) {
        //if (state is PlayerControlState.Disabled || !value.isPressed) return; //runs during dodgeonly state
        if (!pData.controlEnabled) return;

        //if (playerManager.CueDash())
            //SetState(PlayerControlState.Normal);
    }
    #endregion

    #region Combat
    public void OnAttack(InputValue value) {
        //if (state is not PlayerControlState.Normal && value.isPressed) return;
        if (!pData.controlEnabled) return;

        //basicAttacker.CueAttack(value.isPressed);
    }

    public void OnParry(InputValue value) {
        //if (state is not PlayerControlState.Normal) return;
        if (!pData.controlEnabled) return;

        //if (value.isPressed)
        //    playerAbilities.UseParry();
    }

    public void OnHeal(InputValue value) {
        //if (state is not PlayerControlState.Normal) return;
        if (!pData.controlEnabled) return;

        //if (value.isPressed)
        //    playerAbilities.UseHeal();
    }

    public void OnVestige(InputValue value) {
        //if (state is not PlayerControlState.Normal) return;
        if (!pData.controlEnabled) return;

        //if (value.isPressed)
        //    playerAbilities.UseVestige();
    }

    public void OnRememberance(InputValue value) {
        //if (state is not PlayerControlState.Normal) return;
        if (!pData.controlEnabled) return;

        //if (value.isPressed)
        //    playerAbilities.UseRemembrance();
    }

    #endregion

    public void OnInteract(InputValue value) {
        //if (state is PlayerControlState.Disabled) return;
        if (!pData.controlEnabled) return;

        if (value.isPressed)
            InteractKeyPressed?.Invoke();
    }

    public void OnMousePosition(InputValue value) {
        Vector2 screePos = value.Get<Vector2>();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screePos);
        //playerManager.UpdateMouseDirection(worldPos);
    }
}
