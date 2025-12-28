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
   // Add this method at the bottom of PlayerScript.cs (after OnDrawGizmosSelected if you have one)
    protected override void OnDrawGizmosSelected()
    {
        // 1. GROUND CHECK RAY (RED)
        Gizmos.color = Color.red;
        Vector3 groundCheckStart = transform.position;
        Vector3 groundCheckEnd = transform.position + Vector3.down * 0.2f; // Slightly longer for visibility
        Gizmos.DrawLine(groundCheckStart, groundCheckEnd);
        
        // Draw ground hit point if grounded
        if (IsGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, whatIsGround);
            if (hit.collider != null)
            {
                Gizmos.DrawWireSphere(hit.point, 0.05f);
            }
        }

        // 2. ATTACK RANGE (ORANGE) - from PlayerCombat
        if (TryGetComponent<PlayerCombat>(out PlayerCombat combat))
        {
            Gizmos.color = Color.orange;
            if (combat.attackPoint != null)
            {
                Gizmos.DrawWireSphere(combat.attackPoint.position, combat.attackRadius);
            }
        }

        // 3. VELOCITY DIRECTION (CYAN)
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, rb.linearVelocity.normalized * 1.5f);

        // 4. FACING DIRECTION (YELLOW)
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.right * facingDirection * 1.2f);
    }
}