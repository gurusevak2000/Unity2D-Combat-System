using UnityEngine;

public class PlayerCombat : BaseCharacter
{
    [Header("Attack")]
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected float attackRadius = 1.2f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isAttacking;
    private float lastAttackTime;

    private PlayerScript playerScript;  // NEW: reference to PlayerScript

    protected override void Awake()
    {
        base.Awake();
        playerScript = GetComponent<PlayerScript>();  // NEW: get the PlayerScript component
    }

    protected override void Update()
    {
        /* base.Update(); */ // Calls HandleMovement and HandleAnimation from BaseCharacter

        if (Input.GetKeyDown(KeyCode.W) && CanAttack())
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

    // NEW: These two methods are called from Player_KaAnimationEvents
    public void DisableMovementAndJump()
    {
        playerScript.EnableMovement(false);  // Changed to match the actual method name
    }

    public void EnableMovementAndJump()
    {
        playerScript.EnableMovement(true);   // Changed to match the actual method name
    }

    // Draw Gizmo for attack range
    #if UNITY_EDITOR  // Only compiles in Editor — zero runtime cost
        public void DrawAttackGizmos()
        {
            if (attackPoint == null) return;
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f);
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
            Gizmos.DrawLine(transform.position, attackPoint.position);
        }
    #endif
}