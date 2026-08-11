using System.Collections;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        neutral,   // idling and patrolling within a radius
        pursuing,  // chasing and attacking the player
    }

    #region State
    [LayoutStart("State", ELayout.FoldoutBox)]
    public EnemyState enemyState;
    #endregion

    #region Neutral Patrol
    [LayoutStart("Patrolling", ELayout.FoldoutBox)]
    [SerializeField] private float patrolRadius;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private float waypointTolerance;
    [SerializeField] private float patrolWaitMin;
    [SerializeField] private float patrolWaitMax;
    [SerializeField][ReadOnly] private float patrolWaitTimer;
    [SerializeField][ReadOnly] private Vector2 patrolOrigin;
    [SerializeField][ReadOnly] private Vector2 patrolTarget;
    [SerializeField][ReadOnly] private bool waitingAtWaypoint = true;
    #endregion

    #region Pursuing
    [LayoutStart("Pursuit Ranges", ELayout.FoldoutBox)]
    [SerializeField] private float detectionRadius;
    [SerializeField] private float attackRange;
    [SerializeField] private float loseSightRadius;
    #endregion

    #region Pathfinding
    [LayoutStart("Pathfinding", ELayout.FoldoutBox)]
    [SerializeField][ReadOnly] private NavMeshAgent agent;
    [SerializeField] private float stoppingDistance = 0.2f;
    #endregion

    #region References
    [SerializeField][ReadOnly] private Transform playerTransform;

    CharacterManager combatManager;
    EnemyAttacker enemyAttacker;
    #endregion


    void Start()
    {
        waitingAtWaypoint = true;
        patrolTarget = Vector2.zero;
        GetComponents();
        patrolOrigin = transform.position;
        playerTransform = GameObject.FindWithTag("Player").transform;

        // The agent only supplies path corners here; CharacterMovement's Rigidbody2D does the actual moving,
        // so we drive movement/rotation ourselves and keep the agent's internal position in sync each frame.
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = stoppingDistance;
    }

    void GetComponents()
    {
        agent = GetComponent<NavMeshAgent>();

        combatManager = GetComponent<CharacterManager>();
        enemyAttacker = GetComponent<EnemyAttacker>();
    }

    void Update()
    {
        StateUpdate();
        TimerUpdates();
    }

    void TimerUpdates()
    {
        if (waitingAtWaypoint)
        {
            if (enemyState == EnemyState.neutral)
                StopMoving();
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0)
            {
                waitingAtWaypoint = false;
                PickNewPatrolTarget();
            }
        }
    }
    void StateUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.neutral:
                NeutralUpdate();
                CheckForPlayer();
                HandleMovement();
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
            combatManager.MoveInput(Vector2.zero);
            return;
        }

        float distToWaypoint = Vector2.Distance(transform.position, patrolTarget);
        if (distToWaypoint <= waypointTolerance)
        {
            ArriveAtWaypoint();
        }
        else
        {
            SetTargetPosition(patrolTarget);
        }
    }

    void PickNewPatrolTarget()
    {
        Vector2 offset = Random.insideUnitCircle * patrolRadius;
        while (Physics2D.OverlapPoint(offset, obstacleLayers))
        {
            offset = Random.insideUnitCircle * patrolRadius;
        }
        patrolTarget = patrolOrigin + offset;
    }

    void ArriveAtWaypoint()
    {
        combatManager.MoveInput(Vector2.zero);
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
            combatManager.MoveInput(toPlayer);

            if (combatManager.GetCombatState() is ActionState.Idle)
            {
                enemyAttacker.CueAttack();
            }
        }
        else
        {
            // Out of range: only break off if we aren't mid-combo, then close the gap.
            if (combatManager.GetCombatState() is ActionState.Idle)
            {
                HandleMovement();
                SetTargetPosition(playerTransform.position);
            }
        }
    }

    void CheckLoseSight()
    {
        if (playerTransform == null) return;

        float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distToPlayer > loseSightRadius)
        {
            enemyState = EnemyState.neutral;
            StopMoving();
            PickNewPatrolTarget();
        }
    }
    #endregion


    #region Pathfinding
    public void HandleMovement()
    {
        // CharacterMovement's Rigidbody2D is what actually moves us, so keep the agent's
        // internal position pinned to our real position instead of letting it move us.
        agent.nextPosition = transform.position;

        if (!agent.hasPath) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StopMoving();
            return;
        }

        Vector2 moveDir = (Vector2)agent.steeringTarget - (Vector2)transform.position;
        combatManager.MoveInput(moveDir);
    }

    public void StopMoving()
    {
        if (agent.hasPath)
            agent.ResetPath();
        //cMovement.movementInput = Vector2.zero;
        combatManager.MoveInput(Vector2.zero);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }
    #endregion

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (agent != null && agent.hasPath)
        {
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolOrigin, patrolRadius);
    }
}