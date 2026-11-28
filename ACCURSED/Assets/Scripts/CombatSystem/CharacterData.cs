using System;
using System.Collections.Generic;
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
    public bool sprintInput { private get; set; } = false;
    public bool walkInput { private get; set; } = false;

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
        UpdateBuffs();
    }

    protected void UpdateMovementState() {
        if (isIdle && moveInputDirection != Vector2.zero) {
            currDirection = moveInputDirection;
            if(sprintInput) {
                movementState = BaseMoveState.Sprint;
            } else if (walkInput) {
                movementState = BaseMoveState.Walk;
            } else {
                movementState = BaseMoveState.Run;
            }
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

    #region Buffs
    struct BuffInstance {
        public BuffData data;
        public float startTime;
        public BuffInstance(BuffData data, float startTime) {
            this.data = data;
            this.startTime = startTime;
        }
    }

    List<BuffInstance> buffInstances = new List<BuffInstance>();

    public void AddBuff(BuffData buffData) {
        AddBuff(new BuffInstance(buffData, Time.time));
        ResolveBuff(buffData, true);
    }

    protected virtual void ResolveBuff(BuffData buffData, bool isApplying) {
        switch (buffData.type) {
            case BuffType.DefenseUp:
                characterRef.hurtBox.SetDefenseModifier(isApplying ? buffData.amount : 1);
                break;
            case BuffType.StairsSlow:
                characterRef.cMovementNew.SetSpeedMultiplier(isApplying ? buffData.amount : 1);
                break;
            default:
                break;
        }
    }

    void AddBuff(BuffInstance newInstance) {
        for (int i = 0; i < buffInstances.Count; i++) {
            BuffInstance currBuff = buffInstances[i];

            if (currBuff.data.type == newInstance.data.type) {
                currBuff.startTime = newInstance.startTime;
                currBuff.data.duration = Mathf.Max(currBuff.data.duration, newInstance.data.duration);
                return;
            }
        }
        buffInstances.Add(newInstance);
    }

    void UpdateBuffs() {
        for (int i = 0; i < buffInstances.Count; i++) {
            BuffInstance currBuff = buffInstances[i];
            if (currBuff.data.duration < 0) {
                buffInstances.RemoveAt(i);
                i--;
            } else if (currBuff.startTime + currBuff.data.duration < Time.time) {
                ResolveBuff(currBuff.data, false);
                buffInstances.RemoveAt(i);
                i--;
            }
        }
    }
    #endregion
}
