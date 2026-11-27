using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class CharacterMovementNew : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    CharacterData cData;

    #region Refrences
    Rigidbody2D rb;
    #endregion

    void Start() {
        GetComponents();
    }
    void GetComponents() {
        rb = GetComponent<Rigidbody2D>();
        cData = GetComponent<CharacterData>();
    }

    private void Update() {
        if (cData.isIdle) {
            BaseMove(cData.moveInputDirection);
        } else {
            BaseMove(Vector2.zero);

            if(cData.currentAction != null) {
                HandleHorizontalDisplacement(cData.currentAction.loadTime, cData.currentAction.actionSO.HorizontalDisplacement);
            }
        }
    }

    #region Basic Movement
    public void BaseMove(Vector2 moveInput) {
        rb.linearVelocity = moveInput.normalized * movementSpeed;// * speedMultiplier;
    }

    private void HandleHorizontalDisplacement(float startTime, AnimationCurve displacementCurve) {
        float timeSincePlayed = Time.time - startTime;
        float moveAmt = displacementCurve.Evaluate(timeSincePlayed + Time.deltaTime) - displacementCurve.Evaluate(timeSincePlayed);

        Vector3 moveDist = cData.currDirection * moveAmt;

        RaycastHit2D[] raycastInfos = new RaycastHit2D[10];
        if (cData.characterCollider.Cast(moveDist, cData.movementCheckContactFilter, raycastInfos, moveDist.magnitude) == 0) {
            transform.position += moveDist;
        }
    }
    #endregion
}
