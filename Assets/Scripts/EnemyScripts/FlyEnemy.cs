using UnityEngine;

public class FlyEnemy : BaseCharacter
{
    [Header("=== DETECTION ===")]
    [SerializeField] private float detectionRange = 8f;        // 🟡 Big detection circle
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;           // Walls/platforms

    [Header("=== PATROL ===")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f;

    [Header("=== CHASE & ATTACK ===")]
    [SerializeField] private float raycastDistance = 10f;       // How far raycast checks
    [SerializeField] private float idealAttackDistance = 4f;    // 🟢 Maintains this distance!
    [SerializeField] private float minAttackDistance = 2.5f;    // Too close = back off
    [SerializeField] private float chaseSpeed = 4f;

    [Header("=== PROJECTILE ===")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootRange = 7f;
    [SerializeField] private float shootCooldown = 2f;

    [Header("=== DIVE ===")]
    [SerializeField] private float diveRange = 3f;              // Emergency close-range dive
    [SerializeField] private float diveSpeed = 8f;
    [SerializeField] private float diveCooldown = 5f;

    [Header("PLAYER")]
    [SerializeField] private Transform player;

    private Health health;
    private Vector3 startPosition;
    private bool movingRight = true;
    private float lastShootTime;
    private float lastDiveTime;

    // State machine
    private enum FlyState { Patrolling, Chasing, Shooting, Diving }
    private FlyState currentState = FlyState.Patrolling;

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Health>();
        startPosition = transform.position;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override void Update()
    {
        if (canMove == false || player == null) 
        {
            Patrol(); // Even when stunned, keep hovering
            return;
        }

        // 1. RAYCAST DETECTION (Your brilliant idea!)
        bool playerVisible = RaycastDetectsPlayer();
        
        if (!playerVisible)
        {
            currentState = FlyState.Patrolling;
            Patrol();
            return;
        }

        // 2. PLAYER VISIBLE - DECIDE ACTION
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= diveRange && Time.time > lastDiveTime + diveCooldown)
        {
            currentState = FlyState.Diving;
            DiveAttack();
        }
        else if (distanceToPlayer <= shootRange && Time.time > lastShootTime + shootCooldown)
        {
            currentState = FlyState.Shooting;
            ShootProjectile();
        }
        else
        {
            currentState = FlyState.Chasing;
            ChaseAndMaintainDistance();
        }
    }

    // 🔥 YOUR IDEA: RAYCAST DETECTION!
    private bool RaycastDetectsPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, 
                                           raycastDistance, playerLayer | obstacleLayer);
        
        // Player hit AND no obstacles in between
        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void Patrol()
    {
        Vector3 target = movingRight ? 
            startPosition + Vector3.right * patrolDistance : 
            startPosition + Vector3.left * patrolDistance;

        transform.position = Vector3.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            movingRight = !movingRight;
            Flip();
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
    }

    // 🧠 SMART CHASING: Maintains perfect attack distance!
    private void ChaseAndMaintainDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        float moveSpeed = chaseSpeed;
        
        // Too far? → Chase forward
        if (distanceToPlayer > idealAttackDistance)
        {
            rb.linearVelocity = directionToPlayer * moveSpeed;
        }
        // Too close? → Back off slightly
        else if (distanceToPlayer < minAttackDistance)
        {
            rb.linearVelocity = -directionToPlayer * (moveSpeed * 0.7f);
        }
        // Perfect distance → Hover and face player
        else
        {
            rb.linearVelocity = new Vector2(0, Mathf.Sin(Time.time * 2f) * 1f); // Gentle bobbing
        }

        // Always face player
        if ((directionToPlayer.x > 0 && !facingRight) || (directionToPlayer.x < 0 && facingRight))
            Flip();
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        
        // Hover while shooting
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // FIRE!
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
            ep.Initialize(dirToPlayer);

        lastShootTime = Time.time;
        animator?.SetTrigger("Shoot");
    }

    private void DiveAttack()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * diveSpeed;
        
        if ((dir.x > 0 && !facingRight) || (dir.x < 0 && facingRight))
            Flip();

        lastDiveTime = Time.time;
        animator?.SetTrigger("Dive");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(2); // Dive does MORE damage!

            rb.linearVelocity = Vector2.zero;
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // PERFECT GIZMOS!
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // Detection Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Raycast line to player
        if (player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, dir * raycastDistance);
        }
        
        // Ideal attack distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, idealAttackDistance);
        
        // Shoot range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"State: {currentState}");
        #endif
    }
}