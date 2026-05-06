using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;
    public float closeAngle = 0f;
    public float speed = 2f;
    public bool opensInward = true;
    public float interactRange = 2.5f;
    public KeyCode interactKey = KeyCode.E;
    public enum RotationAxis { Y, Z }
    public RotationAxis axis = RotationAxis.Z;
    public GameObject nextRoom;
    public bool requiresKey = false;
    public bool loadsScene = false;
    public string sceneToLoad = "";
    public bool noPivot = false;

    private bool isOpen = false;
    private float targetAngle;
    private Transform player;
    private PlayerInventory playerInventory;

    void Start()
    {
        targetAngle = closeAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerInventory = player.GetComponent<PlayerInventory>();
        if (nextRoom != null) nextRoom.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactRange && Input.GetKeyDown(interactKey))
        {
            /*if (requiresKey && (playerInventory == null || !playerInventory.hasKey))
            {
                Debug.Log("This door requires a key.");
                return;
            }*/

            if (loadsScene && sceneToLoad != "")
            {
                SceneManager.LoadScene(sceneToLoad);
                return;
            }

            isOpen = !isOpen;
            targetAngle = isOpen ? (opensInward ? openAngle : -openAngle) : closeAngle;
            if (nextRoom != null) nextRoom.SetActive(isOpen);
        }

        if (!noPivot)
        {
            if (axis == RotationAxis.Z)
            {
                float angle = Mathf.LerpAngle(transform.localEulerAngles.z, targetAngle, Time.deltaTime * speed);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
            }
            else
            {
                float angle = Mathf.LerpAngle(transform.localEulerAngles.y, targetAngle, Time.deltaTime * speed);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
            }
        }
    }
}