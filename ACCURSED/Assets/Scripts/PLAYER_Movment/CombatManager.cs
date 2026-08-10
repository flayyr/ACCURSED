using System.ComponentModel;
using UnityEngine;

public enum CombatState { Idle, Winding, Attacking, Stunned }
public enum MoveState { None = 0, Walk=1, Run=2, Sprint=3}
public class CombatManager : MonoBehaviour
{
    AttackQueuer attackQueuer;
    CharacterAnimator cAnim;
    CharacterMovement cMove;

    public AttackInstance currAttack = null;

    [SerializeField]CombatState combatState = CombatState.Idle;

    float windTimer = 0;

    private void Awake()
    {
        attackQueuer = GetComponent<AttackQueuer>();
        cAnim = GetComponent<CharacterAnimator>();
        cMove = GetComponent<CharacterMovement>();
    }

    private void OnEnable()
    {
        attackQueuer.OnAttackQueued += PlayNextAttack;
        cAnim.OnAttackFinished += OnAttackFinish;
    }

    private void OnDisable()
    {
        attackQueuer.OnAttackQueued -= PlayNextAttack;
        cAnim.OnAttackFinished -= OnAttackFinish;
    }

    private void Update()
    {
        UpdateWindTimer();
        UpdateMovement();
    }

    void UpdateWindTimer()
    {
        if (windTimer > 0)
        {
            windTimer -= Time.deltaTime;
            if (windTimer <= 0)
            {
                windTimer = 0;
                cAnim.SetWind(false);
                combatState = CombatState.Attacking;
            }
        }
    }

    void OnAttackFinish()
    {
        Debug.Log("attack finished");
        combatState = CombatState.Idle;
        //currAttack = null;
        PlayNextAttack();
    }

    private void PlayNextAttack()
    {
        if(combatState is CombatState.Idle)
        {
            currAttack = attackQueuer.NextAttack();

            if (currAttack != null)
            {
                cAnim.SetWind(true);

                combatState = CombatState.Winding;
                windTimer = currAttack.attackSO.windDuration;

                cAnim.SwitchAnimationState(currAttack.attackSO.windAnimationState);

                if (currAttack.skipWindWhenQueued)
                {
                    SkipWind(currAttack);
                }

                return;
            }
        }
    }

    public void SkipWind(AttackInstance attackInstance)
    {
        if (attackInstance != currAttack || combatState is not CombatState.Winding) return; //make sure currAttack is the same instance to be skipped

        Debug.Log("skipping wind");

        windTimer = 0;
        cAnim.SetWind(false);
        combatState = CombatState.Attacking;
    }




    //Movement

    [SerializeField] MoveState moveState = MoveState.None;
    bool moving = false;
    Vector2 moveInput;

    bool walkInput;
    bool sprintInput;

    public void MoveInput(Vector2 input)
    {
        moveInput = input;
        cAnim.SetFacingDirection(moveInput);
    }

    void UpdateMovement()
    {
        if(combatState is CombatState.Idle)
        {
            FigureOutMovementState();

            cMove.SetMoveSpeed(moveState);
            cAnim.SetMoveState((int)moveState);
            cMove.BaseMove(moveInput);
        }
        else
        {
            moveState = MoveState.None;
            cAnim.SetMoveState(0);
        }
    }


    public void SetWalkInput(bool walk)
    {
        walkInput = !walkInput;
    }
    public void SetSprintInput(bool sprint)
    {
        sprintInput = sprint;
    }

    void FigureOutMovementState()
    { 
        if(moveInput == Vector2.zero)
        {
            moveState = MoveState.None;
            return;
        }

        if (sprintInput)
        {
            moveState = MoveState.Sprint;
        }
        else
        {
            if (walkInput)
            {
                moveState = MoveState.Walk;
            }
            else
            {
                moveState = MoveState.Run;
            }
        }
    }


}
