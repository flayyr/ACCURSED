using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //DEPRECATED SCRIPT

    //enum MovementState
    //{
    //    normal, // can move
    //    launched, // when player is moved by foreign object (knockback / dash) - disables basic movement
    //}


    //#region Movement Toggles (Walk/Sprint)
    //[SerializeField] private bool walk; // checks if the player toggled walk
    //[SerializeField] private bool sprint; // checks if the player is holding spring
    //#endregion


    //#region Dash
    //[SerializeField] private bool canDash; // bool to check if can dash (modded by dash cooldown stuff)
    //[SerializeField] private bool dashing; // bool to check if you are dashing

    //[SerializeField] private float dashPower; // the force magnetitude

    //[SerializeField] private float dashLength; // seconds that the dash lasts
    //[SerializeField] private float dashLengthTimer; // timer used to track the length

    //[SerializeField] private float dashCoolDown; // seconds to wait after dashing to dash again
    //[SerializeField] private float dashCoolDownTimer; // timer used to track the cooldown
    //#endregion


    //#region Movment
    //[SerializeField] private float movementSpeed = 10; // USE THIS MOVEMENT SPEED
    //[SerializeField] public float walkSpeed = 5; // changes movementSpeed to this when walking
    //[SerializeField] public float runSpeed = 10; // changes movementSpeed to this when running (normal)
    //[SerializeField] public float sprintSpeed = 15; // changes movementSpeed to this when sprinting
    //#endregion


    //#region Input
    //private Vector2 movementInput; // input

    //#endregion


    //#region State Control
    //private MovementState movementState; // used to manage when the player should have full control vs when they are knocked back/ dashing

    //#endregion


    //#region Components
    //Rigidbody2D rb;
    //#endregion


    //void Start()
    //{
    //    GetComponents();
    //}
    //void GetComponents()
    //{
    //    rb = GetComponent<Rigidbody2D>();
    //}

    //void Update()
    //{
    //    BaseMovement();
    //    TimerUpdates();
    //}

    //void TimerUpdates()
    //{
    //    // dashing cooldown
    //    if (dashCoolDownTimer <= 0) canDash = true;
    //    else dashCoolDownTimer -= Time.deltaTime;

    //    // dashing length
    //    if (dashLengthTimer <= 0 && dashing == true)
    //    {
    //        dashing = false;
    //        movementState = MovementState.normal;
    //    }
    //    else dashLengthTimer -= Time.deltaTime;
    //}

    //#region Input
    //public void OnMove(InputValue value)
    //{
    //    movementInput = value.Get<Vector2>().normalized;
    //}
    //public void OnWalk(InputValue value)
    //{
    //    walk = !walk;
    //    FigureOutMovementState();
    //}
    //public void OnSprint(InputValue value)
    //{
    //    sprint = !sprint;
    //    FigureOutMovementState();
    //}
    //public void OnDash(InputValue value)
    //{
    //    Dash();
    //}
    //#endregion

    //#region Basic Movement
    //// from input method --> passes state and if its held down or not
    //void FigureOutMovementState()
    //{
    //    if (sprint)
    //    {
    //        walk = false;
    //        movementSpeed = sprintSpeed;
    //    }
    //    else
    //    {
    //        if (walk)
    //        {
    //            movementSpeed = walkSpeed;
    //        }
    //        else
    //        {
    //            movementSpeed = runSpeed;
    //        }
    //    }
    //}
    //void BaseMovement()
    //{
    //    switch (movementState)
    //    {
    //        case MovementState.normal:
    //            rb.linearVelocity = movementInput * movementSpeed;
    //            break;
    //    }
    //}
    //#endregion

    //#region Special
    //void Dash()
    //{
    //    if (canDash)
    //    {
    //        canDash = false;
    //        dashing = true;
    //        dashCoolDownTimer = dashCoolDown;
    //        dashLengthTimer = dashLength;
    //        movementState = MovementState.launched;
    //        rb.linearVelocity = dashPower * movementInput;
    //    }
    //}
    //#endregion
}
