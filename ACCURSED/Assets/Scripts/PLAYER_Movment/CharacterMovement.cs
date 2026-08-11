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

    //#region Movement Toggles (Walk/Sprint)
    //[LayoutStart("Move Toggles", ELayout.FoldoutBox)]
    //[SerializeField][ReadOnly] public bool walk; // checks if the player toggled walk
    //[SerializeField][ReadOnly] public bool sprint; // checks if the player is holding spring
    //#endregion

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

    #region Knockback
    private bool gettingKnockBack;
    private float knockBackLengths;
    private float knockBackTimer;
    #endregion

    #region Movment
    [LayoutStart("Movement Modification", ELayout.FoldoutBox)]
    [ReadOnly] private float movementSpeed = 10; // USE THIS MOVEMENT SPEED
    [SerializeField] public float walkSpeed = 5; // changes movementSpeed to this when walking
    [SerializeField] public float runSpeed = 10; // changes movementSpeed to this when running (normal)
    [SerializeField] public float sprintSpeed = 15; // changes movementSpeed to this when sprinting
    //[SerializeField] float attackStep;
    #endregion


    #region Input
    //[HideInInspector] public Vector2 movementInput; // input

    #endregion

    #region Refrences
    //CharacterCombat cCombat;
    CharacterAnimator cAnimator;
    Rigidbody2D rb;
    #endregion


    void Start()
    {
        GetComponents();
    }
    void GetComponents()
    {
        //cCombat = GetComponent<CharacterCombat>();
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
        //BaseMovement();
        TimerUpdates();
        //if (cAnimator != null)
        //    AnimationUpdate();
    }

    void AnimationUpdate()
    {
        //if (!cCombat.attacking)
        //{
        //    if (movementInput == Vector2.zero) // not moving
        //    {
        //        idleTime -= Time.deltaTime; // timer for when the play the special idle animation

        //        cAnimator.SetMoveState(0);

        //        //if (idleTime < 0) // time to play the special idle animation
        //        //{
        //        //    idleTime = timeTillSpecialIdle; // reset timer
        //        //    cAnimator.Play(idles[UnityEngine.Random.Range(1, idles.Count)]); // play special animation
        //        //    doingIdleSpecial = true;
        //        //}
        //        //else if (!cAnimator.IsCurrentState(idles[0])) // if the animation if currently not the normal idle animation
        //        //{
        //        //    if (doingIdleSpecial)
        //        //    {
        //        //        if (cAnimator.GetCurrentNormalizedTime() >= 1f)
        //        //        {
        //        //            cAnimator.Play(idles[0]);
        //        //            doingIdleSpecial = false;
        //        //        }
        //        //    }
        //        //    else
        //        //        cAnimator.Play(idles[0]);
        //        //}
        //    }
        //    else
        //    {
        //        doingIdleSpecial = false;

        //        idleTime = timeTillSpecialIdle;

        //        //string targetAnim = sprint ? movements[2] : walk ? movements[0] : movements[1];

        //        //if (!cAnimator.IsCurrentState(targetAnim))
        //        //    cAnimator.Play(targetAnim);

        //        cAnimator.SetMoveState(sprint ? 3 : walk ? 1 : 2); //1=walk, 2=run, 3=sprint, 0=not moving
        //    }
        //}
        //else
        //{
        //    doingIdleSpecial = false;
        //}
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

        //knockback
        if (knockBackTimer <= 0 && gettingKnockBack)
        {
            gettingKnockBack = false;
            //if (!cCombat.attacking) // don't hand movement back mid-attack, or the animation stays stuck on the wind-up/swing pose while the character walks around
                movementState = MovementState.normal;
        }
        else
        {
            knockBackTimer -= Time.deltaTime;
        }
    }

    #region Basic Movement
    // from input method --> passes state and if its held down or not
    //public void FigureOutMovementState()
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

    //void BaseMovement()
    //{
    //    switch (movementState)
    //    {
    //        case MovementState.normal:
    //            rb.linearVelocity = movementInput.normalized * movementSpeed;

    //            UpdateRotation();

    //            break;
    //        case MovementState.launched:
    //            //rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.01f);
    //            break;
    //    }
    //}

    public void BaseMove(Vector2 moveInput)
    {
        if(movementState is MovementState.normal)
        rb.linearVelocity = moveInput.normalized * movementSpeed;
    }

    //public void UpdateRotation(Vector2 moveInput)
    //{
    //    if (cAnimator != null)
    //        cAnimator.SetFacingDirection(moveInput);
    //}

    public void AttackForwardStep(Vector2 moveInput, float stepAmount)
    {
        rb.AddForce(moveInput.normalized * stepAmount, ForceMode2D.Impulse);
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
