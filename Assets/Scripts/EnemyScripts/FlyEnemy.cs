using UnityEngine;

public class FlyEnemy : BaseCharacter
{
    [Header("=== FLYING PATROL ===")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolDistance = 5f;

    [Header("=== ATTACK ===")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float diveSpeed = 6f;
    [SerializeField] private float attackDamage = 1;
    [SerializeField] private float attackCooldown = 2f;

    [Header("=== PLAYER ===")]
    [SerializeField] private Transform player;

    private Health health;
    
    // Private variables
    private Vector3 startPosition;
    private bool movingRight = true;
    private float lastAttackTime;
    private bool isDiving;

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
        if (canMove == false) return; // Respect BaseCharacter canMove

        if (player != null && Vector2.Distance(transform.position, player.position) < detectionRange)
        {
            if (Time.time > lastAttackTime + attackCooldown)
                DiveAttack();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        Vector3 target = movingRight ? 
            startPosition + Vector3.right * patrolDistance : 
            startPosition + Vector3.left * patrolDistance;

        // FIXED: Use Vector3.MoveTowards (not VectorMoveTowards)
        transform.position = Vector3.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            movingRight = !movingRight;
            Flip();
        }

        // Keep Y velocity at 0 (flying)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
    }

    private void DiveAttack()
    {
        if (isDiving) return;
        isDiving = true;

        // Dive towards player
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * diveSpeed;

        // Face player direction
        if ((dir.x > 0 && !facingRight) || (dir.x < 0 && facingRight))
            Flip();

        lastAttackTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage((int)attackDamage);

            isDiving = false;
            rb.linearVelocity = Vector2.zero;
        }
        else if (collision.gameObject.CompareTag("Ground") && isDiving)
        {
            isDiving = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Animation override for BaseCharacter
    protected override void HandleAnimation()
    {
        base.HandleAnimation();

        // Movement blend: -1 = left, 0 = idle/hover, 1 = right
        float moveInput = 0f;
        if (rb.linearVelocity.x > 0.1f) moveInput = 1f;
        else if (rb.linearVelocity.x < -0.1f) moveInput = -1f;

        animator.SetFloat("MoveSpeed", moveInput);
    }

    public override void TakeDamage(Vector2 hitDirection)
    {
        base.TakeDamage(hitDirection); // Applies knockback from BaseCharacter

        // Use the Health component's public property
        if (health.CurrentHealth > 0)
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            animator.SetTrigger("Die");
        }
    }
}