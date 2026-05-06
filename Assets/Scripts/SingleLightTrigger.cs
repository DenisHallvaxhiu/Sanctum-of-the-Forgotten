using UnityEngine;
using System.Collections;

public class SingleLightTrigger : MonoBehaviour
{
    public GameObject wall;
    public float wallLowerDistance = 5f;
    public float wallLowerSpeed = 0.2f;
    public GameObject nextRoom;

    private LightSwitch lightSwitch;
    private bool triggered = false;
    private Vector3 wallStartPos;
    private Vector3 wallEndPos;

    void Start()
    {
        lightSwitch = GetComponent<LightSwitch>();
        wallStartPos = wall.transform.position;
        wallEndPos = wallStartPos - new Vector3(0, wallLowerDistance, 0);
        if (nextRoom != null) nextRoom.SetActive(false);
    }

    void Update()
    {
        if (!triggered && lightSwitch.IsLit())
        {
            triggered = true;
            StartCoroutine(LowerWall());
        }
    }

    IEnumerator LowerWall()
    {
        if (nextRoom != null) nextRoom.SetActive(true);
        while (Vector3.Distance(wall.transform.position, wallEndPos) > 0.01f)
        {
            wall.transform.position = Vector3.MoveTowards(
                wall.transform.position,
                wallEndPos,
                Time.deltaTime * wallLowerSpeed
            );
            yield return null;
        }
        wall.transform.position = wallEndPos;
    }
}