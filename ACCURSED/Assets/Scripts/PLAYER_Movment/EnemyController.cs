using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        neutral,   // idling and patrolling within a radius
        pursuing,  // chasing and attacking the player
    }

    #region State
    public EnemyState enemyState;
    #endregion

    #region Neutral Patrol
    [SerializeField] private float patrolRadius;         // how far from origin the enemy can wander
    [SerializeField] private float waypointTolerance;    // how close is "close enough" to a waypoint
    [SerializeField] private float patrolWaitMin;        // minimum seconds to idle at a waypoint
    [SerializeField] private float patrolWaitMax;        // maximum seconds to idle at a waypoint
    [SerializeField] private float patrolWaitTimer;      // counts down while waiting at a waypoint
    private Vector2 patrolOrigin;                        // the spawn point, patrol is relative to this
    private Vector2 patrolTarget;                        // current waypoint
    private bool waitingAtWaypoint;
    #endregion

    #region Pursuing
    [SerializeField] private float detectionRadius;      // radius to detect player and enter pursuing
    [SerializeField] private float attackRange;           // radius to begin attacking
    [SerializeField] private float loseSightRadius;       // radius at which enemy gives up and returns to neutral
    #endregion

    #region Attack Behaviour
    // The enemy mimics a player's hold-to-wind / release-to-swing input. To chain a combo,
    // CharacterCombat.OnAttackAnimationComplete() requires attackButton == true when the
    // swing animation ends, so it winds the next attack instead of resetting. We therefore
    // hold continuously and only release for a single frame to trigger each swing, as long
    // as there are attacks left in the current combo.
    [SerializeField] private float windUpTimer;           // counts down while winding, set from the current attack's wind range
    [SerializeField][Range(0f, 1f)] private float comboContinueChance = 1f; // chance to chain the next attack

    private enum AttackPhase
    {
        idle,       // not attacking
        winding,    // button held, waiting out the wind time
        releasing,  // button let go for one frame to fire the swing
    }
    private AttackPhase attackPhase = AttackPhase.idle;

    private int comboLength;     // cached attack count for the active combo
    private int attacksFired;    // how many swings we've released this combo
    private bool holding;        // current state of the simulated button
    #endregion

    #region Pathfinding
    // goalPoint is where the enemy wants to move; MoveTowardGoal will be
    // replaced with A* logic when that system is added
    private Vector2 goalPoint;
    #endregion

    #region References
    [SerializeField] private Transform playerTransform;
    private CharacterStatistics cStatistics;
    private CharacterMovement cMovement;
    private CharacterCombat cCombat;
    #endregion


    void Start()
    {
        GetComponents();
        patrolOrigin = transform.position;
        PickNewPatrolTarget();
    }

    void GetComponents()
    {
        cStatistics = GetComponent<CharacterStatistics>();
        cMovement = GetComponent<CharacterMovement>();
        cCombat = GetComponent<CharacterCombat>();
    }

    void Update()
    {
        StateUpdate();
        TimerUpdates();
        AttackPhaseUpdate();

        // CharacterCombat reads attackButton + calls AttackUpdate() to act on it.
        // We drive attackButton from our own "holding" intent, then let combat process it.
        cCombat.attackButton = holding;
        cCombat.AttackUpdate();
    }

    void TimerUpdates()
    {
        if (waitingAtWaypoint)
        {
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0)
            {
                waitingAtWaypoint = false;
                PickNewPatrolTarget();
            }
        }
    }

    // Drives the wind -> release -> (chain or stop) cycle for the active combo.
    void AttackPhaseUpdate()
    {
        switch (attackPhase)
        {
            case AttackPhase.winding:
                // Hold the button and wait out the current attack's wind time.
                holding = true;
                if (cCombat.winding)
                {
                    windUpTimer -= Time.deltaTime;
                    if (windUpTimer <= 0)
                    {
                        attackPhase = AttackPhase.releasing;
                    }
                }
                break;

            case AttackPhase.releasing:
                // Let go for this frame so CharacterCombat.AttackUpdate() fires the swing.
                holding = false;
                attacksFired++;

                if (attacksFired < comboLength && PlayerInAttackRange() && Random.value <= comboContinueChance)
                {
                    // More attacks left: re-hold so the button is DOWN when the swing's
                    // animation completes, which makes CharacterCombat wind the next attack.
                    BeginWind();
                }
                else
                {
                    // Combo done (or breaking off): stay released so the swing resolves
                    // and CharacterCombat returns to idle on its own.
                    attackPhase = AttackPhase.idle;
                }
                break;

            case AttackPhase.idle:
                holding = false;
                break;
        }
    }

    void StateUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.neutral:
                NeutralUpdate();
                CheckForPlayer();
                break;

            case EnemyState.pursuing:
                PursuingUpdate();
                CheckLoseSight();
                break;
        }
    }


    #region Neutral
    void NeutralUpdate()
    {
        if (waitingAtWaypoint)
        {
            cMovement.movementInput = Vector2.zero;
            return;
        }

        float distToWaypoint = Vector2.Distance(transform.position, patrolTarget);
        if (distToWaypoint <= waypointTolerance)
        {
            ArriveAtWaypoint();
        }
        else
        {
            goalPoint = patrolTarget;
            MoveTowardGoal();
        }
    }

    void PickNewPatrolTarget()
    {
        Vector2 offset = Random.insideUnitCircle * patrolRadius;
        patrolTarget = patrolOrigin + offset;
    }

    void ArriveAtWaypoint()
    {
        cMovement.movementInput = Vector2.zero;
        waitingAtWaypoint = true;
        patrolWaitTimer = Random.Range(patrolWaitMin, patrolWaitMax);
    }

    void CheckForPlayer()
    {
        if (playerTransform == null) return;

        float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distToPlayer <= detectionRadius)
        {
            enemyState = EnemyState.pursuing;
        }
    }
    #endregion


    #region Pursuing
    void PursuingUpdate()
    {
        if (playerTransform == null) return;

        // Aim movementInput at the player so attacks step/face the right way,
        // even on frames where we hold position.
        Vector2 toPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position);
        float distToPlayer = toPlayer.magnitude;

        if (distToPlayer <= attackRange)
        {
            // In range: face the player, hold position, and start a combo if idle.
            cMovement.movementInput = toPlayer.normalized;

            if (attackPhase == AttackPhase.idle && !cCombat.attacking)
            {
                StartCombo();
            }
        }
        else
        {
            // Out of range: only break off if we aren't mid-combo, then close the gap.
            if (attackPhase == AttackPhase.idle)
            {
                goalPoint = playerTransform.position;
                MoveTowardGoal();
            }
        }
    }

    // Begins a fresh combo from the start of the current combo list.
    void StartCombo()
    {
        comboLength = cCombat.combos[cCombat.currentCombo].attacks.Count;
        attacksFired = 0;
        BeginWind();
    }

    // Hold the button and start winding the attack that is about to play.
    void BeginWind()
    {
        attackPhase = AttackPhase.winding;
        holding = true;
        windUpTimer = GetCurrentAttackWind();
    }

    float GetCurrentAttackWind()
    {
        Combo combo = cCombat.combos[cCombat.currentCombo];
        Attack attack = combo.attacks[cCombat.currentAttack];
        return Random.Range(attack.enemyWindMin, attack.enemyWindMax);
    }

    bool PlayerInAttackRange()
    {
        if (playerTransform == null) return false;
        return Vector2.Distance(transform.position, playerTransform.position) <= attackRange;
    }

    void CheckLoseSight()
    {
        if (playerTransform == null) return;

        float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distToPlayer > loseSightRadius)
        {
            attackPhase = AttackPhase.idle;
            holding = false;
            enemyState = EnemyState.neutral;
            PickNewPatrolTarget();
        }
    }
    #endregion


    #region Pathfinding
    // Drives movementInput toward goalPoint.
    // Replace the body of this method with A* steering when that system is ready.
    void MoveTowardGoal()
    {
        Vector2 direction = (goalPoint - (Vector2)transform.position).normalized;
        cMovement.movementInput = direction;
    }
    #endregion
}