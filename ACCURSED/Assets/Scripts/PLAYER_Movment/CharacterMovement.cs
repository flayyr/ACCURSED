using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public enum MovementState
    {
        normal, // can move
        launched, // when player is moved by foreign object (knockback / dash) - disables basic movement
    }


    #region Movement Toggles (Walk/Sprint)
    [SerializeField] public bool walk; // checks if the player toggled walk
    [SerializeField] public bool sprint; // checks if the player is holding spring
    #endregion

    #region Animations
    [SerializeField] private float idleTime;
    [SerializeField] private float timeTillSpecialIdle;
    [SerializeField] private bool doingIdleSpecial;
    [SerializeField] public List<string> idles; //first is the normal idle the rest are the special ones that happen randomly
    [SerializeField] public List<string> movements; // 0 is walk, 1 is run, 2 is sprint (will be multiples of of this for each direction bc the animations later)
    #endregion


    #region Dash
    [SerializeField] private bool canDash; // bool to check if can dash (modded by dash cooldown stuff)
    [SerializeField] private bool dashing; // bool to check if you are dashing

    [SerializeField] private float dashPower; // the force magnetitude

    [SerializeField] private float dashLength; // seconds that the dash lasts
    [SerializeField] private float dashLengthTimer; // timer used to track the length

    [SerializeField] private float dashCoolDown; // seconds to wait after dashing to dash again
    [SerializeField] private float dashCoolDownTimer; // timer used to track the cooldown
    #endregion

    #region Knockback
    [SerializeField] private bool gettingKnockBack;
    [SerializeField] private float knockBackLengths;
    [SerializeField] private float knockBackTimer;
    #endregion

    #region Movment
    [SerializeField] private float movementSpeed = 10; // USE THIS MOVEMENT SPEED
    [SerializeField] public float walkSpeed = 5; // changes movementSpeed to this when walking
    [SerializeField] public float runSpeed = 10; // changes movementSpeed to this when running (normal)
    [SerializeField] public float sprintSpeed = 15; // changes movementSpeed to this when sprinting
    [SerializeField] float facing;
    [SerializeField] float attackStep;
    #endregion


    #region Input
    public Vector2 movementInput; // input

    #endregion


    #region State Control
    public MovementState movementState; // used to manage when the player should have full control vs when they are knocked back/ dashing

    #endregion


    #region Refrences
    CharacterCombat cCombat;
    CharacterAnimator cAnimator;
    Rigidbody2D rb;
    #endregion


    void Start()
    {
        GetComponents();
    }
    void GetComponents()
    {
        cCombat = GetComponent<CharacterCombat>();
        cAnimator = GetComponent<CharacterAnimator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Knockback(Vector2 direction, float pushPower, bool resetVelocity)
    {
        knockBackTimer = knockBackLengths;
        movementState = MovementState.launched;
        if (resetVelocity)
            rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * pushPower, ForceMode2D.Impulse);
        gettingKnockBack = true;
    }

    void Update()
    {
        BaseMovement();
        TimerUpdates();
        if(cAnimator!=null)
            AnimationUpdate();
    }

    void AnimationUpdate()
    {
        if (!cCombat.attacking)
        {
            if (movementInput == Vector2.zero) // not moving
            {
                idleTime -= Time.deltaTime; // timer for when the play the special idle animation

                if (idleTime < 0) // time to play the special idle animation
                {
                    idleTime = timeTillSpecialIdle; // reset timer
                    cAnimator.Play(idles[UnityEngine.Random.Range(1, idles.Count)]); // play special animation
                    doingIdleSpecial = true;
                }
                else if (!cAnimator.IsCurrentState(idles[0])) // if the animation if currently not the normal idle animation
                {
                    if (doingIdleSpecial)
                    {
                        if (cAnimator.GetCurrentNormalizedTime() >= 1f)
                        {
                            cAnimator.Play(idles[0]);
                            doingIdleSpecial = false;
                        }
                    }
                    else
                        cAnimator.Play(idles[0]);
                }
            }
            else
            {
                doingIdleSpecial = false;

                idleTime = timeTillSpecialIdle;

                string targetAnim = sprint ? movements[2] : walk ? movements[0] : movements[1];

                if (!cAnimator.IsCurrentState(targetAnim))
                    cAnimator.Play(targetAnim);
            }
        }
        else
        {
            doingIdleSpecial = false;
        }
    }

    void TimerUpdates()
    {
        // dashing cooldown
        if (dashCoolDownTimer <= 0) canDash = true;
        else dashCoolDownTimer -= Time.deltaTime;

        // dashing length
        if (dashLengthTimer <= 0 && dashing == true)
        {
            dashing = false;
            movementState = MovementState.normal;
        }
        else dashLengthTimer -= Time.deltaTime;

        // knockback
        if (knockBackTimer <= 0 && gettingKnockBack)
        {
            gettingKnockBack = false;
            movementState = MovementState.normal;
        }
        else
        {
            knockBackTimer -= Time.deltaTime;
        }
    }

    #region Basic Movement
    // from input method --> passes state and if its held down or not
    public void FigureOutMovementState()
    {
        if (sprint)
        {
            walk = false;
            movementSpeed = sprintSpeed;
        }
        else
        {
            if (walk)
            {
                movementSpeed = walkSpeed;
            }
            else
            {
                movementSpeed = runSpeed;
            }
        }
    }
    void BaseMovement()
    {
        switch (movementState)
        {
            case MovementState.normal:
                rb.linearVelocity = movementInput.normalized * movementSpeed;

                UpdateRotation();

                break;
            case MovementState.launched:
                //rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.01f);
                break;
        }
    }

    public void UpdateRotation()
    {
        if(cAnimator != null)
        cAnimator.SetFacingDirection(movementInput);

        /* for shaun :P
        if (movementInput.x == 0 && movementInput.y > 0) facing = 180;
        else if (movementInput.x > 0 && movementInput.y > 0) facing = 135;
        else if (movementInput.x > 0 && movementInput.y == 0) facing = 90;
        else if (movementInput.x > 0 && movementInput.y < 0) facing = 45;
        else if (movementInput.x == 0 && movementInput.y < 0) facing = 0;
        else if (movementInput.x < 0 && movementInput.y < 0) facing = 315;
        else if (movementInput.x < 0 && movementInput.y == 0) facing = 270;
        else if (movementInput.x < 0 && movementInput.y > 0) facing = 225;
        transform.rotation = Quaternion.Euler(0, 0, facing);
        */
    }

    public void AttackForwardStep()
    {
        rb.AddForce(movementInput.normalized * attackStep, ForceMode2D.Impulse);
    }
    #endregion

    #region Special
    public void Dash()
    {
        if (canDash)
        {
            canDash = false;
            dashing = true;
            dashCoolDownTimer = dashCoolDown;
            dashLengthTimer = dashLength;
            movementState = MovementState.launched;
            rb.linearVelocity = dashPower * movementInput;
        }
    }
    #endregion
}
