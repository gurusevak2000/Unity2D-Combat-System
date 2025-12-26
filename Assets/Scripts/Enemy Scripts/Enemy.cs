using UnityEngine;

public class Enemy : PlayerScript
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 2f;
    [SerializeField] private LayerMask playerLayer;

    private EnemyCombat combat;
    private Transform player;
    private bool playerDetected;
    private float patrolTimer;
    private float patrolDirection = 1; // 1 for right, -1 for left

    [Header("Patrol Settings")]
    [SerializeField] private float patrolDuration = 3f;

    protected override void Awake()
    {
        base.Awake(); // Initialize rb and animator from PlayerScript
        combat = GetComponent<EnemyCombat>();
    }

    protected override void Update()
    {
        HandleDetection();
        HandlePatrol();
        HandleMovement();
        HandleAnimation();
        HandleGroundCheckCollions();
        HandleFilp();
        HandleAttack();
    }

    private void HandleDetection()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        playerDetected = hit != null;
        if (playerDetected)
        {
            player = hit.transform;
        }
        else
        {
            player = null;
        }
    }

    private void HandlePatrol()
    {
        if (!playerDetected && canMove)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolDuration)
            {
                patrolDirection *= -1; // Switch direction
                patrolTimer = 0;
            }
        }
    }

    protected override void HandleMovement()
    {
        if (playerDetected && combat != null && combat.IsAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving when attacking
        }
        else if (playerDetected)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving when player detected
        }
        else
        {
            rb.linearVelocity = new Vector2(patrolDirection * speed, rb.linearVelocity.y); // Patrol left and right
        }
    }

    protected override void HandleGroundCheckCollions()
    {
        base.HandleGroundCheckCollions();
    }

    protected override void HandleAttack()
    {
        if (playerDetected && player != null && combat != null)
        {
            combat.TryAttack(player);
        }
    }

    protected override void HandleAnimation()
    {
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", IsGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}