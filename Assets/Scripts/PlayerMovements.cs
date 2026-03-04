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
    private float groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask groundMask = ~0; //It makes everything as default

    //Optional for later
    [Header("Crouch")]
    [SerializeField] private Transform bodyToScale;
    private float crouchScaleY = 0.6f;

    private Rigidbody rb;
    private Collider col;
    private Vector2 moveInput;

    private bool isSprinting;
    private bool isCrouching;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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
    //private bool IsGrounded() {
    //    Vector3 origin = transform.position + Vector3.up * 0.1f;
    //    Debug.DrawRay(origin, Vector3.down * groundCheckDistance, Color.red);
    //    return Physics.Raycast(origin,Vector3.down,groundCheckDistance,groundMask);
    //}

    private bool IsGrounded()
    {
        float extraDistance = 0.05f; // small buffer
        Vector3 origin = col.bounds.center;
        float rayLength = col.bounds.extents.y + extraDistance;

        bool hit = Physics.Raycast(origin, Vector3.down, rayLength, groundMask);
        return hit;
    }

    //Player inputs (Move,Sprint,Crouch,Jump)
    public void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value) {
        if(!value.isPressed) return;
        isSprinting = !isSprinting;
        Debug.Log("Sprint");
    }
    public void OnCrouch(InputValue value) {
        if(!value.isPressed) return;
        isCrouching = !isCrouching;
    }

    public void OnJump(InputValue value) {
        if(!value.isPressed) return;
        if(!IsGrounded()) return;

        Vector3 vector = rb.linearVelocity;
        vector.y = 0f;
        rb.linearVelocity = vector;

        rb.AddForce(Vector3.up * jumpImpulse,ForceMode.Impulse);
    }
}
