using UnityEngine;

public class Enemy : BaseCharacter
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f; // Increased a bit for easier testing
    [SerializeField] private LayerMask playerLayer;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolDuration = 3f;

    private EnemyCombat combat;
    private Transform playerTransform;
    private bool playerDetected;

    private float patrolTimer;
    private float patrolDirection = 1f; // 1 right, -1 left

    protected override void Awake()
    {
        base.Awake();
        combat = GetComponent<EnemyCombat>();
    }

    protected override void Update()
    {
        if (isKnocked) 
        {
            // Still update ground and animation during knockback
            HandleGroundCheckCollions();
            HandleAnimation();
            HandleFlip();
            return;
        }

        // Main logic
        HandleDetection();
        HandlePatrol();
        HandleMovement();
        HandleAttack();  // NEW: Actually try to attack

        // Base handles the rest (ground, animation, flip) - called at end for correct order
        HandleGroundCheckCollions();
        HandleAnimation();
        HandleFlip();
    }

    private void HandleDetection()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        playerDetected = hit != null;
        playerTransform = playerDetected ? hit.transform : null;
    }

    private void HandlePatrol()
    {
        if (!playerDetected && canMove)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolDuration)
            {
                patrolDirection *= -1;
                patrolTimer = 0;
                // Force flip when changing patrol direction
                facingRight = patrolDirection > 0;
                Flip();
            }
        }
    }

    protected override void HandleMovement()
    {
        if (!canMove) return;

        float targetVelocityX;

        if (playerDetected && combat != null && combat.IsAttacking)
        {
            targetVelocityX = 0; // Stop completely during attack
        }
        else if (playerDetected)
        {
            targetVelocityX = 0; // Stop when player near (face them)
        }
        else
        {
            targetVelocityX = patrolDirection * speed; // Patrol
        }

        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    private void HandleAttack()  // NEW: Not override, just private method
    {
        if (playerDetected && playerTransform != null && combat != null && canMove)
        {
            combat.TryAttack(playerTransform);
        }
    }

    // REMOVE your override HandleAnimation() entirely - let base do it!

    // Optional: Better gizmo
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}