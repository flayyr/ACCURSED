using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour //replacing PlayerController
{
    #region References
    PlayerData pData;
    PlayerActionKit actionKit;
    #endregion

    public event Action InteractKeyPressed;

    void Start() {
        RefrenceRetreival();
    }

    void RefrenceRetreival() {
        pData = GetComponent<PlayerData>();
        actionKit = GetComponent<PlayerActionKit>();
    }

    #region Movement
    public void OnMove(InputValue value) {
        pData.SetMoveInputDirection(pData.controlEnabled ? value.Get<Vector2>().normalized : Vector2.zero);
    }
    public void OnWalk(InputValue value) {
        if (!pData.controlEnabled) return;

        pData.walkInput = value.isPressed;
        //playerManager.SetWalkInput(value.isPressed);
    }
    public void OnSprint(InputValue value) {
        if (!pData.controlEnabled) return;

        pData.sprintInput = value.isPressed;
        //playerManager.SetSprintInput(value.isPressed);
    }

    public void OnDash(InputValue value) {
        if (!pData.controlEnabled) return;

        actionKit.CueDash();

        //if (playerManager.CueDash())
        //SetState(PlayerControlState.Normal);
    }

    //aim currDirection to align with mouseDirection
    public void UpdateAimingDirection() {
        if (pData.isIdle) {
            //for whatever fucking reason mouse input position is offset, so i added a temporary offset for now
            pData.currDirection = ((Vector2)(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) + new Vector3(-4, -2, 0) - transform.position)).normalized;
        }
    }
    #endregion

    #region Combat
    public void OnAttack(InputValue value) {
        if (!pData.controlEnabled) return;

        UpdateAimingDirection();
        actionKit.CueAttack(value.isPressed);
        //basicAttacker.CueAttack(value.isPressed);
    }

    public void OnParry(InputValue value) {
        //if (state is not PlayerControlState.Normal) return;
        if (!pData.controlEnabled) return;

        UpdateAimingDirection();
        if (value.isPressed)
            actionKit.UseParry();
    }

    public void OnHeal(InputValue value) {
        //if (state is not PlayerControlState.Normal) return;
        if (!pData.controlEnabled) return;

        if (value.isPressed)
            actionKit.UseHeal();
    }

    public void OnVestige(InputValue value) {
        //if (state is not PlayerControlState.Normal) return;
        if (!pData.controlEnabled) return;

        UpdateAimingDirection();
        if (value.isPressed)
            actionKit.CueVestige();
    }

    public void OnRememberance(InputValue value) {
        //if (state is not PlayerControlState.Normal) return;
        if (!pData.controlEnabled) return;

        UpdateAimingDirection();
        if (value.isPressed)
            actionKit.CueRemembrance();
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
