using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region References
    CharacterMovement cMovement;
    CharacterCombat cCombat;
    #endregion

    public bool disableControl;

    void Start()
    {
        RefrenceRetreival();
    }

    void RefrenceRetreival()
    {
        cMovement = GetComponent<CharacterMovement>();
        cCombat = GetComponent<CharacterCombat>();
    }

    #region Movement
    public void OnMove(InputValue value)
    {
        cMovement.movementInput = disableControl ? Vector2.zero : value.Get<Vector2>();
    }
    public void OnWalk(InputValue value)
    {
        if (disableControl) return;
        cMovement.walk = !cMovement.walk;
        cMovement.FigureOutMovementState();
    }
    public void OnSprint(InputValue value)
    {
        if (disableControl) return;
        cMovement.sprint = !cMovement.sprint;
        cMovement.FigureOutMovementState();
    }
    public void OnDash(InputValue value)
    {
        if (disableControl) return;
        cMovement.Dash();
    }
    #endregion

    #region Combat
    public void OnAttack(InputValue value)
    {
        if (disableControl) return;
        cCombat.attackButton = !cCombat.attackButton;
        cCombat.AttackUpdate();
    }
    #endregion
}
