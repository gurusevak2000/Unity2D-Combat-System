using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Death Settings")]
    public float deathDelay = 1f;

    private Animator animator;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Invoke(nameof(DestroySelf), deathDelay);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}