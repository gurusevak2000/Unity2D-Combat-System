using UnityEngine;

public class PlayerCombat : BaseCharacter
{
    
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isAttacking;
    private float lastAttackTime;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && CanAttack())
            StartAttack();
    }

    private bool CanAttack()
    {
        return !isAttacking && Time.time >= lastAttackTime + attackCooldown;
    }

    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");
    }

    // Animation Event
    public void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponent<Health>();
            if (enemyHealth == null) continue;

            enemyHealth.TakeDamage(1);

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
