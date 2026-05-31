using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool walk;
    [SerializeField] private bool sprint;

    private Vector2 groundNormal = new Vector2(0, 1);
    [SerializeField] private float movementSpeed = 10;
    [SerializeField] public float walkSpeed = 5;
    [SerializeField] public float runSpeed = 10;
    [SerializeField] public float sprintSpeed = 15;
    private Vector2 movementInput;

    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        BaseMovement();
    }

    #region Input
    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }
    public void OnWalk(InputValue value)
    {
        walk = !walk;
        FigureOutMovementState();
    }
    public void OnSprint(InputValue value)
    {
        sprint = !sprint;
        FigureOutMovementState();
    }
    public void OnDash(InputValue value)
    {
        Debug.Log("Dash Action Triggered");
    }
    #endregion

    #region Basic Movement
    // from input method --> passes state and if its held down or not
    void FigureOutMovementState()
    {
        if (sprint)
        {
            walk = false;
            movementSpeed = sprintSpeed;
        }
        else
        {
            if (walk)
            {
                movementSpeed = walkSpeed;
            }
            else
            {
                movementSpeed = runSpeed;
            }
        }
    }
    void BaseMovement()
    {
        Vector2 tangent = new Vector2(groundNormal.y, -groundNormal.x).normalized;
        float targetSpeed = movementInput.x * movementSpeed;
        float currentSpeed = Vector2.Dot(rb.linearVelocity, tangent);
        float speedDif = targetSpeed - currentSpeed;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? S.acceleration : S.decceleration;
        float movement = speedDif * accelRate;
        rb.AddForce(movement * tangent, ForceMode2D.Force);
    }
    #endregion
    #region Special
    void Dash()
    {

    }
    #endregion
}
