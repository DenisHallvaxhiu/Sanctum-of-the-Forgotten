using UnityEngine;
using System.Collections;

public class LightSwitch : MonoBehaviour
{
    public float interactRange = 1f;
    public KeyCode interactKey = KeyCode.E;
    public float targetIntensity = 3f;
    public float fadeSpeed = 1.5f;
    public int lightID;

    private Light pointLight;
    private Transform player;
    private LightPuzzleManager puzzleManager;
    private bool isLit = false;

    void Start()
    {
        pointLight = GetComponent<Light>();
        pointLight.intensity = 0f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        puzzleManager = FindObjectOfType<LightPuzzleManager>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactRange && Input.GetKeyDown(interactKey))
        {
            StopAllCoroutines();
            if (!isLit)
                StartCoroutine(FadeIn());
            else
                StartCoroutine(FadeOut());
        }
    }

   IEnumerator FadeIn()
    {
        isLit = true;
        while (pointLight.intensity < targetIntensity)
        {
            pointLight.intensity += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        pointLight.intensity = targetIntensity;
        Debug.Log("Light " + lightID + " is now lit");
        if (puzzleManager != null) puzzleManager.CheckPuzzle();
    }

    IEnumerator FadeOut()
    {
        isLit = false;
        while (pointLight.intensity > 0f)
        {
            pointLight.intensity -= Time.deltaTime * fadeSpeed;
            if (puzzleManager != null) puzzleManager.CheckPuzzle();
            yield return null;
        }
        pointLight.intensity = 0f;
        if (puzzleManager != null) puzzleManager.CheckPuzzle();
    }

    public bool IsLit()
    {
        return isLit;
    }
}