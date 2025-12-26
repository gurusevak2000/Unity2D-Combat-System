using UnityEngine;

public class EnemyCombat : BaseCharacter
{
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask playerLayer;

    private bool isAttacking;
    private float lastAttackTime;

    public bool IsAttacking => isAttacking;

    public bool TryAttack(Transform target)
    {
        if (isAttacking) return false;
        if (Time.time < lastAttackTime + attackCooldown) return false;

        lastAttackTime = Time.time;
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("attack");
        return true;
    }

    // Animation Event
    public void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            Health playerHealth = hit.GetComponent<Health>();
            if (playerHealth == null) continue;

            playerHealth.TakeDamage(1);

            Vector2 dir = (hit.transform.position - transform.position).normalized;
            hit.GetComponent<BaseCharacter>()?.ApplyKnockback(dir);
        }
    }

    // Animation Event
    public void EndAttack()
    {
        isAttacking = false;
    }
}

