using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region References
    CharacterMovement cMovement;
    CharacterCombat cCombat;
    #endregion

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
        cMovement.movementInput = value.Get<Vector2>();
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
        cMovement.Dash();
    }
    #endregion

    #region Combat
    public void OnAttack(InputValue value)
    {
        cCombat.attackButton = !cCombat.attackButton;
        cCombat.AttackUpdate();
    }
    #endregion
}
