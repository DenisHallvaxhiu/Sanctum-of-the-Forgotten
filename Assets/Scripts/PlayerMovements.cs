using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    //Parameters for player input action
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float crouchMultiplier = 0.5f;

    [Header("Jump")]
    [SerializeField] private float jumpImpulse = 6f;
    private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask = ~0; //It makes everything as default

    //Optional for later
    [Header("Crouch")]
    [SerializeField] private Transform bodyToScale;
    private float crouchScaleY = 0.6f;

    private Rigidbody rb;
    private Vector2 moveInput;

    private bool isSprinting;
    private bool isCrouching;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate() {
        //handles speed movements and its multipliers
        float speed = walkSpeed;
        if(isSprinting) {
            speed *= sprintMultiplier;
        }
        if(isCrouching) {
            speed *= crouchMultiplier;
        }

        Vector3 location = new Vector3(moveInput.x,0f,moveInput.y) * speed;

        location.y = rb.linearVelocity.y;
        rb.linearVelocity = location;
    }

    //Checks if player is grounded
    private bool IsGrounded() {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin,Vector3.down,groundCheckDistance,groundMask);
    }

    //Player inputs (Move,Sprint,Crouch,Jump)
    public void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();

    }

    public void OnSprint(InputValue value) {
        isSprinting = value.isPressed;
        Debug.Log("Sprint");
    }
    public void OnCrouch(InputValue value) {
        if(!value.isPressed) return;
        isCrouching = !isCrouching;
        Debug.Log("Crouch");

    }

    public void OnJump(InputValue value) {
        if(!value.isPressed) return;
        if(!IsGrounded()) return;

        Vector3 vector = rb.linearVelocity;
        vector.y = 0f;
        rb.linearVelocity = vector;

        rb.AddForce(Vector3.up * jumpImpulse,ForceMode.Impulse);
        Debug.Log("Jump");

    }
}
