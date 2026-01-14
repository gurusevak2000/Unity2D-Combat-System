using UnityEngine;

public class FlyEnemy : BaseCharacter
{
    [Header("=== FLYING PATROL ===")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f;

    [Header("=== PROJECTILE ATTACK ===")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootRange = 8f;
    [SerializeField] private float shootCooldown = 2.5f;

    [Header("=== DIVE ATTACK ===")]
    [SerializeField] private float diveRange = 4f; // Closer range than shooting
    [SerializeField] private float diveSpeed = 6f;
    [SerializeField] private float diveCooldown = 4f; // Longer cooldown than shooting

    [Header("=== PLAYER ===")]
    [SerializeField] private Transform player;

    private Health health;
    
    // State machine
    private Vector3 startPosition;
    private bool movingRight = true;
    private float lastShootTime;
    private float lastDiveTime;
    private bool isDiving;
    private enum EnemyState { Patrolling, Shooting, Diving }
    private EnemyState currentState = EnemyState.Patrolling;

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Health>();
        if (health == null)
            Debug.LogError("Health component missing on FlyingEnemy!", this);
            
        startPosition = transform.position;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override void Update()
    {
        if (canMove == false || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // State machine - PRIORITY: Dive > Shoot > Patrol
        if (distanceToPlayer <= diveRange && Time.time > lastDiveTime + diveCooldown && !isDiving)
        {
            currentState = EnemyState.Diving;
            DiveAttack();
        }
        else if (distanceToPlayer <= shootRange && Time.time > lastShootTime + shootCooldown)
        {
            currentState = EnemyState.Shooting;
            ShootProjectile();
        }
        else
        {
            currentState = EnemyState.Patrolling;
            Patrol();
        }
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

    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Face player
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        if ((dirToPlayer.x > 0 && !facingRight) || (dirToPlayer.x < 0 && facingRight))
            Flip();

        // Stop moving, hover while shooting
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Fire!
        Vector2 shootDir = dirToPlayer;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
            ep.Initialize(shootDir);

        //Debug.Log("🟢 SHOOTING PROJECTILE! Distance: " + Vector2.Distance(transform.position, player.position));
        lastShootTime = Time.time;
        animator.SetTrigger("Shoot");
    }

    private void DiveAttack()
    {
        if (isDiving) return;
        isDiving = true;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * diveSpeed;

        if ((dir.x > 0 && !facingRight) || (dir.x < 0 && facingRight))
            Flip();

        lastDiveTime = Time.time;
        animator.SetTrigger("Dive");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(1);

            isDiving = false;
            rb.linearVelocity = Vector2.zero;
        }
        else if (collision.gameObject.CompareTag("Ground") && isDiving)
        {
            isDiving = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    protected override void HandleAnimation()
    {
        base.HandleAnimation();

        float moveInput = 0f;
        if (rb.linearVelocity.x > 0.1f) moveInput = 1f;
        else if (rb.linearVelocity.x < -0.1f) moveInput = -1f;

        animator.SetFloat("MoveSpeed", moveInput);
    }

    public override void TakeDamage(Vector2 hitDirection)
    {
        base.TakeDamage(hitDirection);

        if (health.CurrentHealth > 0)
            animator.SetTrigger("Hit");
        else
            animator.SetTrigger("Die");
    }
}
