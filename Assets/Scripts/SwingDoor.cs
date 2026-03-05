using UnityEngine;

public class SwingDoor : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private bool isLeftDoor = true;

    private bool isOpen = false;
    private float targetAngle = 0f;
    private float currentAngle = 0f;
    private Transform hinge;

    private void Awake()
    {
        GameObject hingeObj = new GameObject("Hinge");
        hingeObj.transform.SetParent(transform.parent);

        if (isLeftDoor)
            hingeObj.transform.position = transform.position + transform.right * (GetWidth() * 0.5f);
        else
            hingeObj.transform.position = transform.position - transform.right * (GetWidth() * 0.5f);

        hingeObj.transform.rotation = transform.rotation;
        transform.SetParent(hingeObj.transform);
        hinge = hingeObj.transform;
    }

    private float GetWidth()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null) return r.bounds.size.x;
        Collider c = GetComponent<Collider>();
        if (c != null) return c.bounds.size.x;
        return 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isOpen) return;

        Vector3 toPlayer = other.transform.position - hinge.position;
        float dot = Vector3.Dot(hinge.forward, toPlayer);
        float direction = dot >= 0f ? 1f : -1f;

        if (!isLeftDoor) direction *= -1f;

        targetAngle = openAngle * direction;
        isOpen = true;
    }

    private void Update()
    {
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, openSpeed * Time.deltaTime);
        hinge.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }
}
