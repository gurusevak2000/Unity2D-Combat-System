using UnityEngine;

public class Enemy : BaseCharacter
{
    [Header("=== DETECTION ===")]
    [SerializeField] private float detectionRange = 6f;     // 🟡 Yellow - Spots player
    [SerializeField] private float chaseRange = 4f;          // 🟢 Green - Starts chasing
    [SerializeField] private LayerMask playerLayer;

    [Header("=== PATROL ===")]
    [SerializeField] private float patrolDuration = 3f;
    [SerializeField] private float patrolSpeedMultiplier = 1f;

    [Header("=== CHASE ===")]
    [SerializeField] private float chaseSpeedMultiplier = 1.3f;

    [Header("=== ATTACK ===")]
    [SerializeField] private float attackRange = 1.5f;       // 🔴 RED - SINGLE SOURCE OF TRUTH
    [SerializeField] private float attackCooldown = 1.5f;

    private EnemyCombat combat;
    private Transform playerTransform;
    
    private enum EnemyState { Patrolling, Chasing, Attacking }
    private EnemyState currentState = EnemyState.Patrolling;
    
    private float patrolTimer;
    private float patrolDirection = 1f;
    private float lastAttackTime;
    private bool playerInDetection;

    protected override void Awake()
    {
        base.Awake();
        combat = GetComponent<EnemyCombat>();
        if (combat == null) Debug.LogError("EnemyCombat missing!", this);
    }

    protected override void Update()
    {
        if (isKnocked || !canMove) 
        {
            HandleGroundCheckCollions();
            HandleAnimation();
            HandleFlip();
            return;
        }

        HandleDetection();
        UpdateStateMachine();
        HandleMovement();
        HandleAttack();

        HandleGroundCheckCollions();
        HandleAnimation();
        HandleFlip(); // ← This handles ALL flipping perfectly
    }

    private void HandleDetection()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        playerInDetection = hit != null;
        playerTransform = playerInDetection ? hit.transform : null;
    }

    private void UpdateStateMachine()
    {
        if (!playerInDetection)
        {
            currentState = EnemyState.Patrolling;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            currentState = EnemyState.Attacking;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            currentState = EnemyState.Patrolling;
        }
    }

    private void HandlePatrol()
    {
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolDuration)
        {
            patrolDirection *= -1;
            patrolTimer = 0;
        }
    }

    protected override void HandleMovement()
    {
        float targetSpeed = speed;

        switch (currentState)
        {
            case EnemyState.Patrolling:
                HandlePatrol();
                targetSpeed *= patrolSpeedMultiplier;
                rb.linearVelocity = new Vector2(patrolDirection * targetSpeed, rb.linearVelocity.y);
                break;

            case EnemyState.Chasing:
                // Chase player!
                float chaseDirection = Mathf.Sign(playerTransform.position.x - transform.position.x);
                targetSpeed *= chaseSpeedMultiplier;
                rb.linearVelocity = new Vector2(chaseDirection * targetSpeed, rb.linearVelocity.y);
                break;

            case EnemyState.Attacking:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
        }
    }

    private void HandleAttack()
    {
        if (currentState == EnemyState.Attacking && playerTransform != null && combat != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange)
            {
                if (combat.TryAttack(playerTransform, attackRange)) // ← PASS attackRange!
                {
                    lastAttackTime = Time.time;
                    currentState = EnemyState.Chasing; // Resume chasing after attack
                }
                else
                {
                    currentState = EnemyState.Chasing; // Couldn't attack, resume chasing
                }
            }
        }
    }

    // FIXED FLIPPING - Handles ALL cases perfectly!
    protected override void HandleFlip()
    {
        float velocityX = rb.linearVelocity.x;
        
        // Case 1: Patrolling - flip based on patrol direction
        if (currentState == EnemyState.Patrolling && Mathf.Abs(velocityX) > flipDeadZone)
        {
            bool shouldFaceRight = velocityX > 0;
            if (shouldFaceRight != facingRight)
                Flip();
        }
        // Case 2: Chasing/Attacking - face player direction
        else if ((currentState == EnemyState.Chasing || currentState == EnemyState.Attacking) 
                 && playerTransform != null)
        {
            bool shouldFaceRight = playerTransform.position.x > transform.position.x;
            if (shouldFaceRight != facingRight)
                Flip();
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * (detectionRange + 0.5f), "🟡 Detect");
        UnityEditor.Handles.Label(transform.position + Vector3.up * (chaseRange + 0.5f), "🟢 Chase");
        UnityEditor.Handles.Label(transform.position + Vector3.right * (attackRange + 0.5f), "🔴 Attack");
        #endif
    }
}