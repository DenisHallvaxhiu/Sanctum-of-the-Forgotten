using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour {
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float distance = 5f;
    public float height = 3f;
    public float mouseSensitivity = 0.2f;

    [Header("Vertical Clamping")]
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 60f;

    [Header("Height Smoothing")]
    public float heightSmoothSpeed = 10f;

    [Header("Collision")]
    public float collisionRadius = 0.25f;
    public float collisionBuffer = 0.15f;
    public LayerMask collisionMask;

    public float Yaw => yaw;

    private float yaw = 0f;
    private float pitch = 20f;
    private float currentFollowY;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(target != null)
            currentFollowY = target.position.y + height;
    }

    void LateUpdate() {
        if(target == null) return;

        Vector2 mouseDelta = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

        yaw += mouseDelta.x * mouseSensitivity;
        pitch -= mouseDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch,minVerticalAngle,maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(pitch,yaw,0f);

        float desiredY = target.position.y + height;
        currentFollowY = Mathf.Lerp(currentFollowY,desiredY,heightSmoothSpeed * Time.deltaTime);

        Vector3 targetPos = new Vector3(target.position.x,currentFollowY,target.position.z);

        Vector3 desiredCameraPos = targetPos + rotation * new Vector3(0f,0f,-distance);

        Vector3 direction = desiredCameraPos - targetPos;
        float desiredDistance = direction.magnitude;

        if(desiredDistance > 0.01f) {
            direction.Normalize();

            RaycastHit hit;
            if(Physics.SphereCast(
                targetPos,
                collisionRadius,
                direction,
                out hit,
                desiredDistance,
                collisionMask,
                QueryTriggerInteraction.Ignore)) {

                float safeDistance = Mathf.Max(0.05f,hit.distance - collisionBuffer);
                desiredCameraPos = targetPos + direction * safeDistance;
            }
        }

        transform.position = desiredCameraPos;
        transform.rotation = rotation;
    }
}