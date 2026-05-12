using UnityEngine;
using Ilumisoft.HealthSystem;

public class PlayerAttack : MonoBehaviour
{
    private enum AttackType
    {
        Light,
        Heavy
    }

    [Header("Light Attack")]
    [SerializeField] private float lightDamage = 50f;
    [SerializeField] private float lightRange = 4f;
    [SerializeField] private float lightAngle = 100f;

    [Header("Heavy Attack")]
    [SerializeField] private float heavyDamage = 90f;
    [SerializeField] private float heavyRange = 4.5f;
    [SerializeField] private float heavyAngle = 120f;
    [SerializeField] private float holdTimeForHeavy = 0.45f;

    [Header("Timing")]
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private float maxAttackTime = 1.2f;

    [Header("Attack Origin")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float attackHeightOffset = 1f;

    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Target Settings")]
    [SerializeField] private LayerMask damageableLayers;

    private float attackTimer;
    private float attackStateTimer;
    private float holdTimer;

    private bool isHoldingAttack;
    private bool isAttacking;
    private bool hasHitDuringThisAttack;
    private bool heavyAlreadyTriggered;

    private AttackType currentAttackType;

    private void Awake()
    {
        if (attackOrigin == null)
            attackOrigin = transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (isHoldingAttack && !isAttacking)
        {
            holdTimer += Time.deltaTime;
        }

        if (isAttacking)
        {
            attackStateTimer -= Time.deltaTime;

            if (attackStateTimer <= 0f)
            {
                AnimationAttackFinished();
            }
        }
    }

    public void StartAttackInput()
    {
        if (attackTimer > 0f) return;
        if (isAttacking) return;

        isHoldingAttack = true;
        heavyAlreadyTriggered = false;
        holdTimer = 0f;
    }

    public void ReleaseAttackInput()
    {
        if (!isHoldingAttack) return;

        isHoldingAttack = false;

        if (attackTimer > 0f) return;
        if (isAttacking) return;

        if (holdTimer >= holdTimeForHeavy)
        {
            StartAttack(AttackType.Heavy);
        }
        else
        {
            StartAttack(AttackType.Light);
        }
    }

    private void StartAttack(AttackType attackType)
    {
        currentAttackType = attackType;

        attackTimer = attackCooldown;
        attackStateTimer = maxAttackTime;

        isAttacking = true;
        hasHitDuringThisAttack = false;

        if (animator != null)
        {
            animator.ResetTrigger("LightAttack");
            animator.ResetTrigger("HeavyAttack");

            if (attackType == AttackType.Light)
            {
                animator.SetTrigger("LightAttack");
            }
            else
            {
                animator.SetTrigger("HeavyAttack");
            }
        }
    }

    public void AnimationAttackHit()
    {
        if (!isAttacking) return;
        if (hasHitDuringThisAttack) return;

        hasHitDuringThisAttack = true;

        if (currentAttackType == AttackType.Light)
        {
            DealConeDamage(lightDamage, lightRange, lightAngle);
        }
        else
        {
            DealConeDamage(heavyDamage, heavyRange, heavyAngle);
        }
    }

    public void AnimationAttackFinished()
    {
        isAttacking = false;
        hasHitDuringThisAttack = false;
    }

    private void DealConeDamage(float damage, float range, float angle)
    {
        Vector3 origin = attackOrigin.position + Vector3.up * attackHeightOffset;

        Collider[] hits = Physics.OverlapSphere(
            origin,
            range,
            damageableLayers
        );

        foreach (Collider hit in hits)
        {
            HealthComponent health = hit.GetComponentInParent<HealthComponent>();

            if (health == null)
                continue;

            Vector3 directionToTarget = hit.transform.position - origin;
            directionToTarget.y = 0f;

            if (directionToTarget.sqrMagnitude <= 0.01f)
                continue;

            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget.normalized);

            if (angleToTarget <= angle * 0.5f)
            {
                health.ApplyDamage(damage);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform originTransform = attackOrigin != null ? attackOrigin : transform;
        Vector3 origin = originTransform.position + Vector3.up * attackHeightOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, lightRange);

        Vector3 lightLeft = Quaternion.Euler(0f, -lightAngle * 0.5f, 0f) * transform.forward;
        Vector3 lightRight = Quaternion.Euler(0f, lightAngle * 0.5f, 0f) * transform.forward;

        Gizmos.DrawRay(origin, lightLeft * lightRange);
        Gizmos.DrawRay(origin, lightRight * lightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, heavyRange);

        Vector3 heavyLeft = Quaternion.Euler(0f, -heavyAngle * 0.5f, 0f) * transform.forward;
        Vector3 heavyRight = Quaternion.Euler(0f, heavyAngle * 0.5f, 0f) * transform.forward;

        Gizmos.DrawRay(origin, heavyLeft * heavyRange);
        Gizmos.DrawRay(origin, heavyRight * heavyRange);
    }
}