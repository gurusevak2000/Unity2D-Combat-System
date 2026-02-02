using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Point")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1.2f;  // ← This is DIFFERENT from attackRange!
    [SerializeField] private LayerMask playerLayer;

    private bool isAttacking;
    private Animator animator;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // FIXED: Now takes attackRange from Enemy.cs!
    public bool TryAttack(Transform target, float maxAttackRange)
    {
        if (isAttacking) return false;

        // ✅ SINGLE SOURCE: Use Enemy's attackRange
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget > maxAttackRange)
        {
            return false; // Won't even trigger attack animation!
        }

        isAttacking = true;
        animator.SetTrigger("attack");
        return true;
    }

    // Animation Event
    public void PerformAttack()
    {
        if (attackPoint == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        int hitCount = 0;
        foreach (Collider2D hit in hits)
        {
            Health playerHealth = hit.GetComponent<Health>();
            if (playerHealth == null) continue;

            playerHealth.TakeDamage(1);
            Vector2 dir = (hit.transform.position - transform.position).normalized;
            hit.GetComponent<BaseCharacter>()?.ApplyKnockback(dir);
            hitCount++;
        }
        
        if (hitCount > 0)
            Debug.Log($"<color=red>💥 Skeleton hit {hitCount} target(s)!</color>");
    }

    // Animation Event
    public void EndAttack()
    {
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}