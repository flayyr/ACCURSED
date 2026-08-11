using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SaintsField;
using SaintsField.Playa;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Combo
{
    [SerializeField] public string name;
    [Table] public List<Attack> attacks;
}
[System.Serializable]
public class Attack
{
    [SerializeField] public string name;
    [SerializeField] public float enemyWindMin;
    [SerializeField] public float enemyWindMax;
}

public class CharacterCombat : MonoBehaviour
{
    #region Modifiers
    [LayoutStart("Combos", ELayout.FoldoutBox)]
    public List<Combo> combos;
    #endregion

    #region States
    [LayoutStart("Debug", ELayout.FoldoutBox)]
    [ReadOnly] public bool winding;
    [ReadOnly] public int currentCombo = 0;
    [ReadOnly] public int currentAttack = 0;
    [ReadOnly] public bool attackButton; // shows when the button is pressed
    [ReadOnly] public bool attacking;
    [ReadOnly] public bool releasing;
    [ReadOnly] public bool attackCue;
    #endregion

    #region Refrences
    CharacterMovement cMovement;
    CharacterAnimator cAnimator;
    Rigidbody2D rb;
    #endregion


    void Start()
    {
        GetComponents();
    }
    void GetComponents()
    {
        cMovement = GetComponent<CharacterMovement>();
        cAnimator = GetComponent<CharacterAnimator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        TimerUpdates();
    }

    void TimerUpdates()
    {

    }

    public void AttackUpdate()
    {
        if (attackButton && !attacking)
        {
            rb.linearVelocity = Vector2.zero;
            attacking = true;
            winding = true;
            releasing = false;
            Wind();
        }
        else if (!attackButton && winding)
        {
            attacking = true;
            winding = false;
            releasing = true;
            Attack();
        }
        else if (attackButton && attacking)
        {
            attackCue = true;
        }
    }

    void Wind()
    {
        cMovement.UpdateRotation();
        cMovement.AttackForwardStep();

        //cMovement.movementState = CharacterMovement.MovementState.launched;

        ////////////////cAnimator.Play(combos[currentCombo].attacks[currentAttack].name + "Wind");
    }

    void Attack()
    {
        cMovement.UpdateRotation();
        cMovement.AttackForwardStep();

        //cMovement.movementState = CharacterMovement.MovementState.launched;

        ///////////cAnimator.Play(combos[currentCombo].attacks[currentAttack].name);

        if (combos[currentCombo].attacks.Count - 1 > currentAttack)
        {
            currentAttack++;
        }
        else
        {
            currentAttack = 0;
        }
    }

    //must place event 
    public void OnAttackAnimationComplete()
    {
        if (attackButton)
        {
            rb.linearVelocity = Vector2.zero;
            attackCue = false;
            attacking = true;
            winding = true;
            releasing = false;
            Wind();
        }
        else if (attackCue)
        {
            rb.linearVelocity = Vector2.zero;
            attackCue = false;
            attacking = true;
            winding = false;
            releasing = true;
            Attack();
        }
        else
        {
            attacking = false;
            releasing = false;
            currentAttack = 0;
            //cMovement.movementState = CharacterMovement.MovementState.normal;
        }
    }
}
