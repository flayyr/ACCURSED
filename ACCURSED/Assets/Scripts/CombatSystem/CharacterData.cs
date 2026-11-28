using System;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    const float _horizontalDirectionalPriority = 0.15f;

    [SerializeField] public Collider2D characterCollider;
    [SerializeField] public ContactFilter2D movementCheckContactFilter;
    [SerializeField] public CharacterReference characterRef;
    [Header("Stagger")]
    [SerializeField] public float staggerDuration;
    [SerializeField] protected float maxHP;

    public ActionInstance currentAction { get; set; }

    public ActionState actionState { get; set; }
    public bool isIdle { get { return actionState == ActionState.Idle; } }
    public BaseMoveState movementState { get; set; }

    #region directions
    //current direction, cannot be zero
    public Vector2 currDirection { get; set; } = Vector2.down;

    //currently inputted direction, can be zero
    public Vector2 moveInputDirection {  get; private set; }

    //latest nonzero inputted direction
    public Vector2 prevMoveInputDirection { get; private set; }

    //current direction translated into the othogonal directions
    public Vector2 facingDirection { 
        get {
            Vector2[] orthogonalDirections = {Vector2.up, Vector2.right, Vector2.down, Vector2.left};
            float[] dotToDirections = {
                Vector2.Dot(orthogonalDirections[0], currDirection),
                Vector2.Dot(orthogonalDirections[1], currDirection) + _horizontalDirectionalPriority,
                Vector2.Dot(orthogonalDirections[2], currDirection),
                Vector2.Dot(orthogonalDirections[3], currDirection) + _horizontalDirectionalPriority,
            };

            int outputIndex = 0;
            float largestDot = dotToDirections[0];

            for (int i = 1; i < dotToDirections.Length; i++) {
                if (dotToDirections[i] > largestDot) {
                    outputIndex = i;
                    largestDot = dotToDirections[i];
                }
            }
            return orthogonalDirections[outputIndex];
        }
    }
    #endregion

    #region hp
    public event Action OnHealthUpdate;
    public float currHP { get;set; }
    bool dead = false;
    #endregion


    private void Awake() {
        currHP = maxHP;
    }

    protected virtual void Start() {
        Reset();
    }

    protected virtual void Update() {
        UpdateMovementState();
    }

    protected void UpdateMovementState() {
        if (isIdle && moveInputDirection != Vector2.zero) {
            currDirection = moveInputDirection;
            movementState = BaseMoveState.Run;
        } else {
            movementState = BaseMoveState.None;
        }
    }

    public void SetMoveInputDirection(Vector2 moveDir) {
        moveInputDirection = moveDir.normalized;
        if(moveInputDirection != Vector2.zero) {
            prevMoveInputDirection = moveInputDirection;
        }
    }

    public void UpdateCurrDirection() {
        currDirection = prevMoveInputDirection;
    }

    public float UpdateHealth(float healthChange) {
        if (dead) return currHP;

        currHP += healthChange;
        currHP = Mathf.Clamp(currHP, 0, maxHP);

        if (currHP == 0) {
            characterRef.cDeath.Die();
            dead = true;
        }

        OnHealthUpdate?.Invoke();

        return currHP;
    }

    public virtual void Reset() {
        dead = false;
        UpdateHealth(maxHP);
    }
}
