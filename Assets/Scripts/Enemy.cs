// 24-12-2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class EnemyAI : PlayerScript
{
    private bool playerDetected;
    private bool isAttacking;
    private float patrolTimer;
    private float patrolDirection = 1; // 1 for right, -1 for left

    [Header("Patrol Settings")]
    [SerializeField] private float patrolDuration = 3f;

    protected override void Update()
    {
        HandlePatrol();
        HandleMovement();
        HandleAnimation();
        HandleGroundCheckCollions();
        HandleFilp();
        HandleAttack();
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
        if (playerDetected || isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop moving when attacking or player detected
        }
        else
        {
            rb.linearVelocity = new Vector2(patrolDirection * speed, rb.linearVelocity.y); // Patrol left and right
        }
    }

    protected override void HandleGroundCheckCollions()
    {
        base.HandleGroundCheckCollions();
        playerDetected = Physics2D.OverlapCircle(
            attackingPoint.position,
            attackRadius,
            WhatIsTarget
        ) != null;
    }

    protected override void HandleAttack()
    {
        if (playerDetected && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("attack");
            Invoke(nameof(ResetAttack), 1f); // Reset attack state after 1 second
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    protected override void HandleAnimation()
    {
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", IsGrounded);
    }
}