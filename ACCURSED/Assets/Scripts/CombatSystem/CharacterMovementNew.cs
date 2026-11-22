using UnityEngine;

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
        if (cData.isIdle && cData.moveInputDirection != Vector2.zero) {
            BaseMove(cData.moveInputDirection);
        } else {
            BaseMove(Vector2.zero);
        }
    }

    #region Basic Movement
    public void BaseMove(Vector2 moveInput) {
        rb.linearVelocity = moveInput.normalized * movementSpeed;// * speedMultiplier;
    }
    #endregion
}
