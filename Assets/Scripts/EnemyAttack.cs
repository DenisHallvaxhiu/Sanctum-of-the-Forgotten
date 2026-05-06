using UnityEngine;
using Ilumisoft.HealthSystem;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackAngle = 90f; // cone angle

    public void AnimationAttackHit()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange
        );

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle <= attackAngle * 0.5f)
            {
                HealthComponent health = hit.GetComponentInParent<HealthComponent>();

                if (health != null)
                {
                    health.ApplyDamage(damage);
                    Debug.Log("Zombie hit PLAYER only!");
                    return;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 left = Quaternion.Euler(0, -attackAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, attackAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, left * attackRange);
        Gizmos.DrawRay(transform.position, right * attackRange);
    }
}