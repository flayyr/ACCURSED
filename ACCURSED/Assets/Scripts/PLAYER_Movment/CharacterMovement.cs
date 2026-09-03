using System;
using System.Collections;
using System.Collections.Generic;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public enum MovementState
    {
        normal, // can move
        launched, // when player is moved by foreign object (knockback / dash) - disables basic movement
    }

    #region State Control
    [LayoutStart("State", ELayout.FoldoutBox)]
    public MovementState movementState; // used to manage when the player should have full control vs when they are knocked back/ dashing
    #endregion

    #region Animations
    [LayoutStart("Animation Names", ELayout.FoldoutBox)]
    private float idleTime;
    [SerializeField] private float timeTillSpecialIdle;
    private bool doingIdleSpecial;
    [SerializeField] public List<string> idles; //first is the normal idle the rest are the special ones that happen randomly
    [SerializeField] public List<string> movements; // 0 is walk, 1 is run, 2 is sprint (will be multiples of of this for each direction bc the animations later)
    #endregion


    #region Dash
    [LayoutStart("Dash", ELayout.FoldoutBox)]
    [ReadOnly] private bool canDash; // bool to check if can dash (modded by dash cooldown stuff)
    [ReadOnly] private bool dashing; // bool to check if you are dashing

    [SerializeField] private float dashPower; // the force magnetitude

    [SerializeField] private float dashLength; // seconds that the dash lasts
    private float dashLengthTimer; // timer used to track the length

    [SerializeField] private float dashCoolDown; // seconds to wait after dashing to dash again
    private float dashCoolDownTimer; // timer used to track the cooldown

    [SerializeField] private float dashInvincinbleTime = 0.5f;
    [SerializeField] private HurtBox hurtBox;
    #endregion

    #region Movment
    [LayoutStart("Movement Modification", ELayout.FoldoutBox)]
    [ReadOnly] private float movementSpeed = 10; // USE THIS MOVEMENT SPEED
    [SerializeField] public float walkSpeed = 5; // changes movementSpeed to this when walking
    [SerializeField] public float runSpeed = 10; // changes movementSpeed to this when running (normal)
    [SerializeField] public float sprintSpeed = 15; // changes movementSpeed to this when sprinting
    #endregion

    #region Refrences
    Rigidbody2D rb;
    #endregion


    void Start()
    {
        GetComponents();
    }
    void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Knockback(Vector2 direction, float pushPower, bool resetVelocity)
    {
        if (resetVelocity)
            rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * pushPower, ForceMode2D.Impulse);
    }

    void Update()
    {
        TimerUpdates();
    }

    void TimerUpdates()
    {
        // dashing cooldown
        if (dashCoolDownTimer <= 0)
            canDash = true;
        else
            dashCoolDownTimer -= Time.deltaTime;

        // dashing length
        if (dashLengthTimer <= 0 && dashing == true)
        {

            dashing = false;
            movementState = MovementState.normal;
            BaseMove(rb.linearVelocity);
        }
        else dashLengthTimer -= Time.deltaTime;
    }

    #region Basic Movement

    public void SetMoveSpeed(BaseMoveState moveState)
    {
        switch (moveState)
        {
            case BaseMoveState.Walk:
                movementSpeed = walkSpeed;
                break;
            case BaseMoveState.Run:
                movementSpeed = runSpeed;
                break;
            case BaseMoveState.Sprint:
                movementSpeed = sprintSpeed;
                break;
            default:
                break;
        }
    }

    public void BaseMove(Vector2 moveInput)
    {
        if (movementState is MovementState.normal)
            rb.linearVelocity = moveInput.normalized * movementSpeed;
    }
    #endregion

    #region Special
    public void Dash(Vector2 movementInput)
    {
        if (canDash)
        {
            canDash = false;
            dashing = true;
            dashCoolDownTimer = dashCoolDown;
            dashLengthTimer = dashLength;
            movementState = MovementState.launched;
            rb.linearVelocity = dashPower * movementInput;

            hurtBox.InvincibleForSeconds(dashInvincinbleTime);
        }
    }
    #endregion
}
