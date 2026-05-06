using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightPuzzleManager : MonoBehaviour
{
    public LightSwitch[] allLights;
    public int[] requiredLightIDs = { 1, 2, 4, 6, 7 };
    public GameObject wall;
    public float wallLowerDistance = 5f;
    public float wallLowerSpeed = 2f;
    public GameObject nextRoom;

    private bool puzzleSolved = false;
    private Vector3 wallStartPos;
    private Vector3 wallEndPos;

    void Start()
    {
        wallStartPos = wall.transform.position;
        wallEndPos = wallStartPos - new Vector3(0, wallLowerDistance, 0);
        if (nextRoom != null) nextRoom.SetActive(false);
    }

    public void CheckPuzzle()
    {
        if (puzzleSolved) return;

        HashSet<int> litIDs = new HashSet<int>();
        foreach (LightSwitch ls in allLights)
        {
            if (ls.IsLit()) litIDs.Add(ls.lightID);
        }


        if (litIDs.Count != requiredLightIDs.Length) return;

        HashSet<int> requiredSet = new HashSet<int>(requiredLightIDs);
        if (!litIDs.SetEquals(requiredSet)) return;

        puzzleSolved = true;
        StartCoroutine(LowerWall());
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