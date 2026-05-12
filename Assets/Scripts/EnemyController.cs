using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private EnemyEvents enemyEvents;
    private NavMeshAgent agent;

    public Transform player;

    public float visionDistance = 10f;
    public float stopDistance = 1.5f;

    public bool canSeePlayer;

    [Header("Enemy Stats")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float angularSpeed = 720f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 1.8f;
    [SerializeField] private float attackCooldown = 1.2f;

    [Header("Wander")]
    public float wanderRadius = 5f;
    public float waitTimeMin = 1f;
    public float waitTimeMax = 3f;
    public float wanderStopDistance = 0.8f;
    public float maxWanderTime = 3f;

    private float attackTimer;
    private float wanderTimer;

    private Vector3 startPosition;
    private Vector3 wanderTarget;
    private float waitTimer;
    private bool isWaiting;
    private bool isMoving;

    private void Awake()
    {
        enemyEvents = GetComponent<EnemyEvents>();
        agent = GetComponent<NavMeshAgent>();

        startPosition = transform.position;

        agent.stoppingDistance = stopDistance;
        agent.speed = speed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;

        // Important: we rotate manually to avoid NavMeshAgent fighting our attack rotation
        agent.updateRotation = false;

        attackTimer = 0f;

        PickNewWanderTarget();
    }

    private void Update()
    {
        CheckVision();

        attackTimer -= Time.deltaTime;
        isMoving = false;

        if (canSeePlayer && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackDistance)
            {
                isMoving = AttackPlayer();
            }
            else
            {
                isWaiting = false;
                isMoving = MoveTowardsPlayer();
            }
        }
        else
        {
            isMoving = Wander();
        }

        enemyEvents.RaiseMove(isMoving);
    }

    private bool AttackPlayer()
    {
        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        RotateTowards(player.position - transform.position);

        if (attackTimer <= 0f)
        {
            enemyEvents.RaiseAttack();
            attackTimer = attackCooldown;
        }

        return false;
    }

    private bool MoveTowardsPlayer()
    {
        if (player == null) return false;

        if (agent.isStopped)
            agent.isStopped = false;

        agent.stoppingDistance = stopDistance;
        agent.SetDestination(player.position);

        RotateTowards(agent.desiredVelocity);

        if (agent.pathPending) return true;

        return agent.remainingDistance > agent.stoppingDistance;
    }

    private bool Wander()
    {
        if (agent.isStopped)
            agent.isStopped = false;

        agent.stoppingDistance = wanderStopDistance;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            agent.ResetPath();

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                PickNewWanderTarget();
                wanderTimer = 0f;
            }

            return false;
        }

        wanderTimer += Time.deltaTime;
        agent.SetDestination(wanderTarget);

        RotateTowards(agent.desiredVelocity);

        if (agent.pathPending) return true;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            waitTimer = Random.Range(waitTimeMin, waitTimeMax);
            return false;
        }

        if (wanderTimer >= maxWanderTime)
        {
            PickNewWanderTarget();
            wanderTimer = 0f;
            return false;
        }

        return true;
    }

    private void RotateTowards(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void CheckVision()
    {
        canSeePlayer = false;

        if (player == null) return;

        Vector3 origin = transform.position + Vector3.up * 0.8f;
        Vector3 target = player.position + Vector3.up * 0.8f;

        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        if (distance > visionDistance) return;

        direction.Normalize();

        if (Physics.Raycast(origin, direction, out RaycastHit hit, visionDistance))
        {
            if (hit.transform == player || hit.transform.root == player.root)
            {
                canSeePlayer = true;
            }
        }
    }

    private void PickNewWanderTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            Vector3 randomPoint = startPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                wanderTarget = hit.position;
                wanderTimer = 0f;
                return;
            }
        }

        wanderTarget = transform.position;
        wanderTimer = 0f;
    }
}