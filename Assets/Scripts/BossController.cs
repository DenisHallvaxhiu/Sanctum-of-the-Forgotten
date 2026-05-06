using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossController : MonoBehaviour
{
    private enum BossState
    {
        Intro,
        Idle,
        Chasing,
        Attacking
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private BossIntroCamera bossIntroCamera;

    [Header("Intro")]
    [SerializeField] private bool playIntroOnStart = true;

    [Header("Movement")]
    [SerializeField] private float visionDistance = 25f;
    [SerializeField] private float stopDistance = 3.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack Distances")]
    [SerializeField] private float closeAttackDistance = 4.5f;
    [SerializeField] private float jumpAttackMinDistance = 10f;
    [SerializeField] private float jumpAttackMaxDistance = 14f;

    [Header("Attack Cooldowns")]
    [SerializeField] private float globalAttackCooldown = 1.5f;
    [SerializeField] private float punchCooldown = 2f;
    [SerializeField] private float swipeCooldown = 3f;
    [SerializeField] private float jumpAttackCooldown = 6f;

    [Header("Jump Leap Settings")]
    [SerializeField] private float jumpLeapDuration = 1.2f;
    [SerializeField] private float jumpHeight = 3.5f;
    [SerializeField] private float landingDistanceFromPlayer = 2.5f;
    [SerializeField] private float navMeshSampleRadius = 4f;

    private BossState currentState;

    private bool introFinished;
    private bool isLeaping;

    private float globalAttackTimer;
    private float punchTimer;
    private float swipeTimer;
    private float jumpAttackTimer;

    private Coroutine leapCoroutine;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stopDistance;
            agent.updateRotation = false;
        }
    }

    private void Start()
    {
        if (playIntroOnStart)
        {
            BeginIntro();
        }
        else
        {
            FinishIntro();
        }
    }

    private void Update()
    {
        TickTimers();

        if (!introFinished)
        {
            StopMovement();
            return;
        }

        if (player == null)
        {
            SetIdle();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Intro:
                StopMovement();
                break;

            case BossState.Idle:
                HandleIdle(distanceToPlayer);
                break;

            case BossState.Chasing:
                HandleChasing(distanceToPlayer);
                break;

            case BossState.Attacking:
                HandleAttacking();
                break;
        }
    }

    private void BeginIntro()
    {
        introFinished = false;
        currentState = BossState.Intro;

        StopMovement();

        animator.SetBool("IsMoving", false);
        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("Intro");

        if (bossIntroCamera != null)
            bossIntroCamera.StartIntroCamera();
    }

    public void FinishIntro()
    {
        introFinished = true;
        currentState = BossState.Idle;

        animator.SetBool("IsMoving", false);
        animator.SetFloat("Speed", 0f);

        if (bossIntroCamera != null)
            bossIntroCamera.EndIntroCamera();
    }

    private void TickTimers()
    {
        globalAttackTimer -= Time.deltaTime;
        punchTimer -= Time.deltaTime;
        swipeTimer -= Time.deltaTime;
        jumpAttackTimer -= Time.deltaTime;
    }

    private void HandleIdle(float distanceToPlayer)
    {
        StopMovement();

        if (TryChooseAttack(distanceToPlayer))
            return;

        if (distanceToPlayer <= visionDistance)
        {
            currentState = BossState.Chasing;
        }
    }

    private void HandleChasing(float distanceToPlayer)
    {
        if (distanceToPlayer > visionDistance)
        {
            SetIdle();
            return;
        }

        if (TryChooseAttack(distanceToPlayer))
            return;

        MoveTowardsPlayer();
    }

    private void HandleAttacking()
    {
        StopMovement(false);

        if (player != null)
            RotateTowards(player.position - transform.position);
    }

    private bool TryChooseAttack(float distanceToPlayer)
    {
        if (globalAttackTimer > 0f)
            return false;

        if (distanceToPlayer >= jumpAttackMinDistance &&
            distanceToPlayer <= jumpAttackMaxDistance &&
            jumpAttackTimer <= 0f)
        {
            StartAttack("JumpAttack");
            jumpAttackTimer = jumpAttackCooldown;
            return true;
        }

        if (distanceToPlayer <= closeAttackDistance)
        {
            bool canPunch = punchTimer <= 0f;
            bool canSwipe = swipeTimer <= 0f;

            if (!canPunch && !canSwipe)
                return false;

            if (canPunch && canSwipe)
            {
                if (Random.value < 0.5f)
                {
                    StartAttack("Punch");
                    punchTimer = punchCooldown;
                }
                else
                {
                    StartAttack("Swipe");
                    swipeTimer = swipeCooldown;
                }

                return true;
            }

            if (canPunch)
            {
                StartAttack("Punch");
                punchTimer = punchCooldown;
                return true;
            }

            if (canSwipe)
            {
                StartAttack("Swipe");
                swipeTimer = swipeCooldown;
                return true;
            }
        }

        return false;
    }

    private void StartAttack(string triggerName)
    {
        currentState = BossState.Attacking;
        globalAttackTimer = globalAttackCooldown;

        // Stop physical movement, but do not always force idle animation.
        // This keeps Run -> JumpAttack from popping through Idle.
        StopMovement(false);

        animator.ResetTrigger("Punch");
        animator.ResetTrigger("Swipe");
        animator.ResetTrigger("JumpAttack");

        animator.SetTrigger(triggerName);

        if (triggerName != "JumpAttack")
        {
            animator.SetBool("IsMoving", false);
            animator.SetFloat("Speed", 0f);
        }

        if (triggerName == "JumpAttack")
        {
            if (leapCoroutine != null)
                StopCoroutine(leapCoroutine);

            leapCoroutine = StartCoroutine(JumpLeapToPlayer());
        }
    }

    private IEnumerator JumpLeapToPlayer()
    {
        if (player == null)
            yield break;

        isLeaping = true;

        Vector3 startPosition = transform.position;

        Vector3 directionFromPlayerToBoss = transform.position - player.position;
        directionFromPlayerToBoss.y = 0f;
        directionFromPlayerToBoss.Normalize();

        if (directionFromPlayerToBoss.sqrMagnitude <= 0.01f)
            directionFromPlayerToBoss = -transform.forward;

        Vector3 desiredLandingPosition =
            player.position + directionFromPlayerToBoss * landingDistanceFromPlayer;

        if (NavMesh.SamplePosition(
                desiredLandingPosition,
                out NavMeshHit navHit,
                navMeshSampleRadius,
                NavMesh.AllAreas))
        {
            desiredLandingPosition = navHit.position;
        }

        float baseY = startPosition.y;
        desiredLandingPosition.y = baseY;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.updatePosition = false;
        }

        float elapsed = 0f;

        while (elapsed < jumpLeapDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / jumpLeapDuration);

            Vector3 flatPosition = Vector3.Lerp(
                startPosition,
                desiredLandingPosition,
                t
            );

            float heightOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            flatPosition.y = baseY + heightOffset;

            transform.position = flatPosition;

            if (player != null)
                RotateTowards(player.position - transform.position);

            yield return null;
        }

        transform.position = desiredLandingPosition;

        if (agent != null)
        {
            agent.updatePosition = true;

            if (agent.isOnNavMesh)
                agent.Warp(desiredLandingPosition);

            agent.isStopped = true;
            agent.ResetPath();
        }

        isLeaping = false;
    }

    public void AnimationAttackFinished()
    {
        if (leapCoroutine != null)
        {
            StopCoroutine(leapCoroutine);
            leapCoroutine = null;
        }

        if (agent != null)
        {
            agent.updatePosition = true;

            if (agent.isOnNavMesh)
                agent.Warp(transform.position);
        }

        isLeaping = false;

        currentState = BossState.Idle;

        animator.SetBool("IsMoving", false);
        animator.SetFloat("Speed", 0f);
    }

    private void MoveTowardsPlayer()
    {
        if (player == null || agent == null)
            return;

        currentState = BossState.Chasing;

        if (agent.isStopped)
            agent.isStopped = false;

        agent.stoppingDistance = stopDistance;
        agent.SetDestination(player.position);

        animator.SetBool("IsMoving", true);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        RotateTowards(agent.desiredVelocity);
    }

    private void SetIdle()
    {
        currentState = BossState.Idle;

        StopMovement();

        animator.SetBool("IsMoving", false);
        animator.SetFloat("Speed", 0f);
    }

    private void StopMovement(bool updateAnimator = true)
    {
        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        if (!agent.isStopped)
            agent.isStopped = true;

        agent.ResetPath();

        if (updateAnimator && animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.SetFloat("Speed", 0f);
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}