using UnityEngine;
using UnityEngine.SceneManagement;
using Ilumisoft.HealthSystem;
using System.Collections;

public class BossWinOnDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthComponent health;

    [Header("Win Scene")]
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private float delayBeforeSceneLoad = 3f;

    private bool hasTriggeredWin;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnHealthEmpty += HandleBossDeath;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnHealthEmpty -= HandleBossDeath;
    }

    private void HandleBossDeath()
    {
        if (hasTriggeredWin)
            return;

        hasTriggeredWin = true;

        StartCoroutine(LoadWinSceneAfterDelay());
    }

    private IEnumerator LoadWinSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeSceneLoad);

        SceneManager.LoadScene(winSceneName);
    }
}