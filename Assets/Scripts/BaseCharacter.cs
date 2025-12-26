using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;

    [Header("Knockback")]
    [SerializeField] protected float knockbackForce = 12f;
    protected bool isKnocked;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void TakeDamage(Vector2 hitDirection)
    {
        ApplyKnockback(hitDirection);
    }

    protected virtual void ApplyKnockback(Vector2 dir)
    {
        if (isKnocked) return;

        isKnocked = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir.normalized * knockbackForce, ForceMode2D.Impulse);

        Invoke(nameof(EndKnockback), 0.2f);
    }

    protected void EndKnockback()
    {
        isKnocked = false;
    }

    public bool IsKnocked => isKnocked;
}
