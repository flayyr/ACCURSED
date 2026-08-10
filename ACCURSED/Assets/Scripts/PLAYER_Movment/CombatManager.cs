using UnityEngine;

public enum CombatState { Idle, Winding, Attacking, Stunned }
public class CombatManager : MonoBehaviour
{
    AttackQueuer attackQueuer;
    CharacterAnimator cAnim;
    CharacterMovement cMove;

    public AttackInstance currAttack = null;

    [SerializeField]CombatState state = CombatState.Idle;

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
                state = CombatState.Attacking;
            }
        }
    }

    void OnAttackFinish()
    {
        Debug.Log("attack finished");
        state = CombatState.Idle;
        //currAttack = null;
        PlayNextAttack();
    }

    private void PlayNextAttack()
    {
        if(state is CombatState.Idle)
        {
            currAttack = attackQueuer.NextAttack();

            if (currAttack != null)
            {
                cAnim.SetWind(true);

                state = CombatState.Winding;
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
        if (attackInstance != currAttack || state is not CombatState.Winding) return; //make sure currAttack is the same instance to be skipped

        Debug.Log("skipping wind");

        windTimer = 0;
        cAnim.SetWind(false);
        state = CombatState.Attacking;
    }

}
