using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float crouchMultiplier = 0.3f;

    [Header("Jump")]
    [SerializeField] private float jumpImpulse = 6f;
    private float groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("Crouch")]
    [SerializeField] private Transform bodyToScale;
    private float crouchScaleY = 0.6f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody rb;
    private Collider col;
    private Vector2 moveInput;

    private bool isSprinting;
    private bool isCrouching;

    private CameraFollow cameraFollow;
    private PlayerEvents playerEvents;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.freezeRotation = true;
        playerEvents = GetComponent<PlayerEvents>();
        cameraFollow = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : FindFirstObjectByType<CameraFollow>();
    }

    private void FixedUpdate() {
        float speed = walkSpeed;
        if(isSprinting) speed *= sprintMultiplier;
        if(isCrouching) speed *= crouchMultiplier;

        float camYaw = cameraFollow != null ? cameraFollow.Yaw : 0f;
        Quaternion camRotation = Quaternion.Euler(0f,camYaw,0f);
        Vector3 inputDir = new Vector3(moveInput.x,0f,moveInput.y);
        Vector3 worldDir = camRotation * inputDir;

        Vector3 velocity = worldDir * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        if(worldDir.sqrMagnitude > 0.01f) {
            Quaternion targetRot = Quaternion.LookRotation(worldDir);
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private bool IsGrounded() {
        float extraDistance = 0.05f;
        Vector3 origin = col.bounds.center;
        float rayLength = col.bounds.extents.y + extraDistance;
        return Physics.Raycast(origin,Vector3.down,rayLength,groundMask);
    }

    public void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
        playerEvents.RaiseMove(!(moveInput == Vector2.zero));
    }

    public void OnSprint(InputValue value) {
        isSprinting = value.isPressed;
        playerEvents.RaiseSprint(isSprinting);
    }

    public void OnCrouch(InputValue value) {
        isCrouching = !isCrouching;
        playerEvents.RaiseCrouch(isCrouching);
    }

    public void OnJump(InputValue value) {
        if(!value.isPressed) return;
        if(!IsGrounded()) return;

        Vector3 vector = rb.linearVelocity;
        vector.y = 0f;
        rb.linearVelocity = vector;

        rb.AddForce(Vector3.up * jumpImpulse,ForceMode.Impulse);
        playerEvents.RaiseJump();
    }
}
