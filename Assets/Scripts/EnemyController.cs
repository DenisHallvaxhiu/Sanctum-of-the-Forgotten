using UnityEngine;

public class EnemyController : MonoBehaviour {
    private EnemyEvents enemyEvents;

    public Transform player;

    public float visionDistance = 10f;
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;

    public bool canSeePlayer;

    [Header("Wander")]
    public float wanderRadius = 5f;
    public float waitTimeMin = 1f;
    public float waitTimeMax = 3f;
    public float wanderStopDistance = 0.2f;
    public float maxWanderTime = 1f;
    private float wanderTimer = 0f;

    private Vector3 startPosition;
    private Vector3 wanderTarget;
    private float waitTimer;
    private bool isWaiting;

    private bool lastCanSeePlayer;
    private bool isMoving;

    private void Awake() {
        enemyEvents = GetComponent<EnemyEvents>();
        startPosition = transform.position;
        PickNewWanderTarget();
    }

    void Update() {
        CheckVision();

        bool isMoving = false;

        if(canSeePlayer) {
            isMoving = MoveTowardsPlayer();
        }
        else {
            isMoving = Wander();
        }

        enemyEvents.RaiseMove(isMoving);
    }

    void CheckVision() {
        canSeePlayer = false;

        if(player == null) return;

        Vector3 origin = transform.position;
        Vector3 target = player.position;

        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        if(distance > visionDistance) return;

        direction.Normalize();

        RaycastHit hit;
        if(Physics.Raycast(origin,direction,out hit,visionDistance)) {
            if(hit.transform == player || hit.transform.root == player.root) {
                canSeePlayer = true;
            }
        }

        Debug.DrawRay(origin,direction * visionDistance,canSeePlayer ? Color.green : Color.red);
    }

    bool MoveTowardsPlayer() {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if(distance > stopDistance) {
            direction.Normalize();
            transform.position += direction * moveSpeed * Time.deltaTime;

            if(direction != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            return true; // ✅ moving
        }

        return false; // ❌ stopped (close to player)
    }

    bool Wander() {
        if(isWaiting) {
            waitTimer -= Time.deltaTime;

            if(waitTimer <= 0f) {
                isWaiting = false;
                PickNewWanderTarget();
                wanderTimer = 0f;
            }

            return false; // ❌ not moving
        }

        wanderTimer += Time.deltaTime;

        Vector3 direction = wanderTarget - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if(distance <= wanderStopDistance) {
            isWaiting = true;
            waitTimer = Random.Range(waitTimeMin,waitTimeMax);
            return false; // ❌ reached target → waiting
        }

        if(wanderTimer >= maxWanderTime) {
            PickNewWanderTarget();
            wanderTimer = 0f;
            return false; // ❌ switching target (pause frame)
        }

        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;

        if(direction != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        return true; // ✅ moving
    }

    void PickNewWanderTarget() {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = transform.position + new Vector3(randomCircle.x,0f,randomCircle.y);

        wanderTimer = 0f;
    }
}