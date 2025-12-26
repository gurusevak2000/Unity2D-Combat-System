using System;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class PlayerScript : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;

    [Header("Attack details")]
    [SerializeField] protected float attackRadius = 1f;
    [SerializeField] protected Transform attackingPoint;
    [SerializeField] protected LayerMask WhatIsTarget;

    [Header("Movement details")]
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float jumpForce = 10f;
    protected int faceing_Direction = 1;//Dame i am stupid
    private bool facingRight = true;
    protected bool canMove = true;
    private bool canJump = true;
    private float xInput;
    [SerializeField] private float flipDeadZone = 0.05f;

    [Header("Ground Check details")]
    [SerializeField] private float groundCheckDistance =10f;
    protected bool IsGrounded;
    [SerializeField] private LayerMask WhatIsGround;

    protected virtual void Awake()
    {   
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //float horizontalInput = Input.GetAxis("Horizontal");
        //HandleMovement(horizontalInput);

        //if (Input.GetKeyDown(KeyCode.W) && IsGrounded && canJump)//Jumping
        //{
        //    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        //}
        //else if (Input.GetKeyUp(KeyCode.W))//This makes the jump shorter if the player lets go early
        //{
        //    if (rb.linearVelocity.y > 0)
        //    {
        //        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //    PlayerAttack();

        HandleAnimation();
        HandleFilp();
        HandleGroundCheckCollions();
        HandleInput();
        HandleMovement();
    }

    private void Jump()
    {
        if(IsGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.W))
            HandleAttack();

    }

    protected override void HandleMovement()
    {
        if (!canMove) return;

        // STOP movement during knockback
        if (TryGetComponent<BaseCharacter>(out var baseChar))
        {
            if (baseChar.IsKnocked) return;
        }

        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    }


    /* public void DamageTarget()
    {
        Debug.Log("DamageEnemies called!");
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackingPoint.position, attackRadius, WhatIsTarget);
        foreach(Collider2D enemy in enemyColliders)
        {
            PlayerScript entityTarget = enemy.GetComponent<PlayerScript>();
            entityTarget.TakeDamage();
        }
    }

    private void TakeDamage()
    {
        throw new NotImplementedException();
    } */

    //Enable or disable player movement and jumping
    public virtual void EnableJumpandMovement(bool enable)
    {
        canMove = enable;
        canJump = enable;
    }

    protected void HandleFilp()
    {
        if (Mathf.Abs(rb.linearVelocity.x) < flipDeadZone)
            return;

        if (rb.linearVelocity.x > 0 && !facingRight)
            Flip();

        else if(rb.linearVelocity.x < 0 && facingRight)
            Flip();
    }
    protected void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        faceing_Direction *= -1;
    }

    protected virtual void HandleAttack()
    {
        if(IsGrounded)
        {
                animator.SetTrigger("Attack");
                //rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);//Stop moving when attacking//Now it is not useful
        }
    }

    protected virtual void HandleAnimation()
    {  
        animator.SetBool("IsGrounded", IsGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("xVelocity", rb.linearVelocity.x);
        
    }

    protected virtual void HandleGroundCheckCollions()
    {
        IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance,WhatIsGround);//result of raycast can true or false
        //either we detect something or not
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance, 0));
        if (attackingPoint != null)
        {
            Gizmos.DrawWireSphere(attackingPoint.position, attackRadius);
        }
    }

}
