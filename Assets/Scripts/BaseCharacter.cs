using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    protected Animator animator;

    [Header("Knockback")]
    [SerializeField] protected float knockbackForce = 12f;
    protected bool isKnocked;

    [Header("Movement")]
    public float speed = 3f;
    protected bool canMove = true;

    [Header("Ground Check")]
    public bool IsGrounded;
    [SerializeField] protected float groundCheckDistance = 0.1f; // Fixed: was 10f!
    [SerializeField] protected LayerMask whatIsGround;

    // Flipping
    protected int facingDirection = 1;
    protected bool facingRight = true;
    [SerializeField] protected float flipDeadZone = 0.05f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void TakeDamage(Vector2 hitDirection)
    {
        ApplyKnockback(hitDirection);
    }

    public virtual void ApplyKnockback(Vector2 dir)
    {
        if (isKnocked) return;

        isKnocked = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir.normalized * knockbackForce, ForceMode2D.Impulse);
        Invoke(nameof(EndKnockback), 0.2f);
    }

    protected void EndKnockback()
    {
        isKnocked = false;
    }

    public bool IsKnocked => isKnocked;

    protected virtual void Update()
    {
        if (!isKnocked)
        {
            HandleMovement();
            HandleAnimation();
            HandleGroundCheckCollions();
            HandleFlip();
        }
    }

    // === These are now virtual so children can override ===
    protected virtual void HandleMovement() { }
    protected virtual void HandleAnimation()
    {
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", IsGrounded);
    }

    protected virtual void HandleGroundCheckCollions()
    {
        IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    protected virtual void HandleFlip()
    {
        if (Mathf.Abs(rb.linearVelocity.x) < flipDeadZone) return;

        if (rb.linearVelocity.x > 0 && !facingRight)
            Flip();
        else if (rb.linearVelocity.x < 0 && facingRight)
            Flip();
    }

    protected virtual void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDirection *= -1;
    }

    public int FacingDirection => facingDirection;

    // Optional gizmos
    protected virtual void OnDrawGizmosSelected()
    {
        if (whatIsGround != 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        }
    }
}