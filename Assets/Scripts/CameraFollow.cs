using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float distance = 5f;
    public float height = 3f;
    public float mouseSensitivity = 0.2f;

    [Header("Vertical Clamping")]
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 60f;

    public float Yaw => yaw;

    private float yaw = 0f;
    private float pitch = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 mouseDelta = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

        yaw += mouseDelta.x * mouseSensitivity;
        pitch -= mouseDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPos = new Vector3(target.position.x, target.position.y + height, target.position.z);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        transform.position = targetPos + offset;

        transform.rotation = rotation;
    }
}
