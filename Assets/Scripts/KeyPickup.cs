using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public float floatHeight = 0.3f;
    public float floatSpeed = 2f;
    public float rotateSpeed = 90f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * floatSpeed) * floatHeight, 0);
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.hasKey = true;
            Destroy(gameObject);
        }
    }
}