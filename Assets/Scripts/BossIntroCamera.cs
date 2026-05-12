using UnityEngine;

public class BossIntroCamera : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera introCamera;
    [SerializeField] private Camera mainCamera;

    [Header("Target")]
    [SerializeField] private Transform bossTarget;
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 2f, 0f);

    [Header("Camera Motion")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 2.5f;
    [SerializeField] private float orbitDegrees = 90f;
    [SerializeField] private float orbitDuration = 6f;
    [SerializeField] private float startAngle = 180f;

    [Header("Gameplay Lock")]
    [SerializeField] private MonoBehaviour[] playerScriptsToDisable;

    private bool isPlaying;
    private float timer;

    private void Awake()
    {
        if (introCamera == null)
            introCamera = GetComponent<Camera>();

        if (introCamera != null)
            introCamera.enabled = false;

        if (mainCamera != null)
            mainCamera.enabled = true;
    }

    public void StartIntroCamera()
    {
        timer = 0f;
        isPlaying = true;

        if (mainCamera != null)
            mainCamera.enabled = false;

        if (introCamera != null)
            introCamera.enabled = true;

        SetPlayerScripts(false);
    }

    public void EndIntroCamera()
    {
        isPlaying = false;

        if (introCamera != null)
            introCamera.enabled = false;

        if (mainCamera != null)
            mainCamera.enabled = true;

        SetPlayerScripts(true);
    }

    private void LateUpdate()
    {
        if (!isPlaying || bossTarget == null)
            return;

        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / orbitDuration);
        float angle = startAngle + orbitDegrees * t;

        Vector3 focusPoint = bossTarget.position + lookOffset;

        Vector3 horizontalOffset = Quaternion.Euler(0f, angle, 0f) * Vector3.back * distance;
        Vector3 cameraPosition = focusPoint + horizontalOffset + Vector3.up * height;

        transform.position = cameraPosition;
        transform.LookAt(focusPoint);
    }

    private void SetPlayerScripts(bool value)
    {
        foreach (MonoBehaviour script in playerScriptsToDisable)
        {
            if (script != null)
                script.enabled = value;
        }
    }
}