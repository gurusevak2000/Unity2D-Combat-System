using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Animator animator;
    public Rigidbody2D rb;

    [Header("Attack details")]
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private Transform attackingPoint;
    [SerializeField] private LayerMask enemyLayer;

    public float speed = 10f;
    public float jumpForce = 10f;

    private bool facingRight = true;
    public float groundCheckDistance =10f;
    public bool IsGrounded;
    public LayerMask WhatIsGround;
    public bool canMove = true;
    public bool canJump = true;


    void Awake()
    {   
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if(canMove)
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if(Input.GetKeyDown(KeyCode.W) && IsGrounded && canJump)//Jumping
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else if(Input.GetKeyUp(KeyCode.W))//This makes the jump shorter if the player lets go early
        {
            if(rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
            PlayerAttack();

        HandleAnimation();
        HandleFilp();
        HandleGroundCheckCollions();
    }

    public void DamageEnemies()
    {
        Debug.Log("DamageEnemies called!");
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackingPoint.position, attackRadius, enemyLayer);
        foreach(Collider2D enemy in enemyColliders)
        {
            enemy.GetComponent<Enemy>().TakeDamage();
        }
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

    private void PlayerAttack()
    {
        if(IsGrounded)
        {
                animator.SetTrigger("Attack");
                //rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);//Stop moving when attacking//Now it is not useful
        }
        
    }

    void HandleAnimation()
    {  
        animator.SetBool("IsGrounded", IsGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("xVelocity", rb.linearVelocity.x);
        
    }

    private void HandleGroundCheckCollions()
    {
        IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance,WhatIsGround);//result of raycast can true or false
        //either we detect something or not
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance, 0));
        Gizmos.DrawWireSphere(attackingPoint.position, attackRadius);
    }

}
