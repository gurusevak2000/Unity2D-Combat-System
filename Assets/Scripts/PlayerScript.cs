using System;
using UnityEngine;

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
    private bool facingRight = true;
    public bool canMove = true;
    public bool canJump = true;

    [Header("Ground Check details")]
    [SerializeField] protected float groundCheckDistance =10f;
    [SerializeField] protected bool IsGrounded;
    [SerializeField] protected LayerMask WhatIsGround;

    private void Awake()
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
        float horizontalInput = Input.GetAxis("Horizontal");
        HandleMovement(horizontalInput);

        if (Input.GetKeyDown(KeyCode.W) && IsGrounded && canJump)//Jumping
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if (Input.GetKeyUp(KeyCode.W))//This makes the jump shorter if the player lets go early
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            PlayerAttack();

        HandleAnimation();
        HandleFilp();
        HandleGroundCheckCollions();
    }

    protected virtual void HandleMovement(float horizontalInput)
    {
        if (canMove)
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public void DamageTarget()
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
    }

    //Enable or disable player movement and jumping
    public void EnableJumpandMovement(bool enable)
    {
        canMove = enable;
        canJump = enable;
    }

    private void HandleFilp()
    {
        if(rb.linearVelocity.x > 0 && !facingRight)
            Flip();

        else if(rb.linearVelocity.x < 0 && facingRight)
            Flip();
    }
    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }

    protected virtual void PlayerAttack()
    {
        if(IsGrounded)
        {
                animator.SetTrigger("Attack");
                //rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);//Stop moving when attacking//Now it is not useful
        }
        
    }

    protected void HandleAnimation()
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance, 0));
        if (attackingPoint != null)
        {
            Gizmos.DrawWireSphere(attackingPoint.position, attackRadius);
        }
    }

}
