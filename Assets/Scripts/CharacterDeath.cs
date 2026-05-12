using UnityEngine;
using UnityEngine.AI;
using Ilumisoft.HealthSystem;

public class CharacterDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthComponent health;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Rigidbody rb;

    [Header("Scripts To Disable On Death")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    [Header("Animation")]
    [SerializeField] private string deathTriggerName = "Death";

    [Header("Colliders")]
    [SerializeField] private bool disableCollidersOnDeath = true;
    [SerializeField] private Collider[] collidersToDisable;

    [Header("Destroy")]
    [SerializeField] private bool destroyAfterDeath = false;
    [SerializeField] private float destroyDelay = 5f;

    private bool isDead;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<HealthComponent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (collidersToDisable == null || collidersToDisable.Length == 0)
            collidersToDisable = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnHealthEmpty += Die;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnHealthEmpty -= Die;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger(deathTriggerName);
        }

        if (disableCollidersOnDeath)
        {
            foreach (Collider col in collidersToDisable)
            {
                if (col != null)
                    col.enabled = false;
            }
        }

        if (destroyAfterDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}