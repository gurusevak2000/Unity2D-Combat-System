using System.Collections;
using UnityEngine;

public class PlayerCombat : BaseCharacter
{
    [Header("Attack")]
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected float attackRadius = 1.2f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Dash Attack")]
    [SerializeField] private float dashAttackDamageMultiplier = 2f;   // 2× damage
    [SerializeField] private float dashAttackKnockbackMultiplier = 1.5f;

    private bool isAttacking;
    private float lastAttackTime;

    private PlayerScript playerScript;

    protected override void Awake()
    {
        base.Awake();
        playerScript = GetComponent<PlayerScript>();
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && CanAttack())
        {
            StartAttack();
        }
    }

    private bool CanAttack()
    {
        return !isAttacking && Time.time >= lastAttackTime + attackCooldown;
    }

    private void StartAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        // Brief stop for wind-up
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        if (playerScript != null)
            playerScript.EnableMovement(false);

        // ────────────────────────────────────────────────
        // DECIDE: normal attack OR dash attack?
        // ────────────────────────────────────────────────
        bool isDashAttack = playerScript != null && playerScript.isDashing;

        if (isDashAttack)
        {
            animator.SetTrigger("DashAttack");     // Dash attack animation
            // Optional: shorter cooldown during dash
            lastAttackTime = Time.time - attackCooldown * 0.4f;
        }
        else
        {
            animator.SetTrigger("Attack");         // Normal attack
        }

        CancelInvoke(nameof(ForceEndAttack));
        Invoke(nameof(ForceEndAttack), 2f);
    }

    private void ForceEndAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            if (playerScript != null)
                playerScript.EnableMovement(true);
        }
    }

    public void EndAttack()
    {
        CancelInvoke(nameof(ForceEndAttack));
        isAttacking = false;
        if (playerScript != null)
            playerScript.EnableMovement(true);
    }

    public void PerformAttack()
    {
        bool isDashAttack = playerScript != null && playerScript.isDashing;

        float damage = isDashAttack ? 1 * dashAttackDamageMultiplier : 1;
        float knockbackMult = isDashAttack ? dashAttackKnockbackMultiplier : 1f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponent<Health>();
            if (enemyHealth == null) continue;

            enemyHealth.TakeDamage(Mathf.RoundToInt(damage));

            Vector2 dir = (hit.transform.position - transform.position).normalized;
            hit.GetComponent<BaseCharacter>()?.ApplyKnockback(dir * knockbackMult);
        }
    }

#if UNITY_EDITOR
    public void DrawAttackGizmos()
    {
        if (attackPoint == null) return;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
#endif
}