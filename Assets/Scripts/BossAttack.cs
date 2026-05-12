using UnityEngine;
using Ilumisoft.HealthSystem;

public class BossAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackOrigin;

    [Header("Punch")]
    [SerializeField] private float punchDamage = 20f;
    [SerializeField] private float punchRange = 2.2f;
    [SerializeField] private float punchAngle = 80f;

    [Header("Swipe")]
    [SerializeField] private float swipeDamage = 30f;
    [SerializeField] private float swipeRange = 3f;
    [SerializeField] private float swipeAngle = 130f;

    [Header("Jump Attack")]
    [SerializeField] private float jumpDamage = 45f;
    [SerializeField] private float jumpRange = 3.5f;
    [SerializeField] private float jumpAngle = 160f;

    private void Awake()
    {
        if (attackOrigin == null)
            attackOrigin = transform;
    }

    public void AnimationPunchHit()
    {
        DealConeDamage(punchDamage, punchRange, punchAngle, "Punch");
    }

    public void AnimationSwipeHit()
    {
        DealConeDamage(swipeDamage, swipeRange, swipeAngle, "Swipe");
    }

    public void AnimationJumpAttackHit()
    {
        DealConeDamage(jumpDamage, jumpRange, jumpAngle, "Jump Attack");
    }

    private void DealConeDamage(float damage, float range, float angle, string attackName)
    {
        Collider[] hits = Physics.OverlapSphere(
            attackOrigin.position,
            range
        );

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            Vector3 directionToTarget = (hit.transform.position - attackOrigin.position).normalized;
            float targetAngle = Vector3.Angle(transform.forward, directionToTarget);

            if (targetAngle <= angle * 0.5f)
            {
                HealthComponent health = hit.GetComponentInParent<HealthComponent>();

                if (health != null)
                {
                    health.ApplyDamage(damage);
                    Debug.Log("Boss " + attackName + " hit player for " + damage + " damage.");
                    return;
                }
            }
        }

        Debug.Log("Boss " + attackName + " missed.");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackOrigin == null)
            attackOrigin = transform;

        DrawAttackGizmo(punchRange, punchAngle, Color.red);
        DrawAttackGizmo(swipeRange, swipeAngle, Color.yellow);
        DrawAttackGizmo(jumpRange, jumpAngle, Color.magenta);
    }

    private void DrawAttackGizmo(float range, float angle, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(attackOrigin.position, range);

        Vector3 left = Quaternion.Euler(0, -angle / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, angle / 2f, 0) * transform.forward;

        Gizmos.DrawRay(attackOrigin.position, left * range);
        Gizmos.DrawRay(attackOrigin.position, right * range);
    }
}