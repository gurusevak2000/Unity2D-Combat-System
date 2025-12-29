using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    [Header("Death Settings")]
    public float deathDelay = 1f;

    [Header("VFX")]
    [SerializeField] private GameObject hitSparkPrefab;

    private Animator animator;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    // SINGLE TakeDamage method - with shake + sparks!
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hit");

        // SHAKE CAMERA!
        CameraFollowWithLookAhead camera = Object.FindFirstObjectByType<CameraFollowWithLookAhead>();
        //Debug.Log($"Camera found: {(camera != null ? camera.name : "NULL")}");
        if (camera != null)
            camera.TriggerShake(0.15f, 0.5f);

        // SPAWN SPARKS - ONLY ONCE!
        if (hitSparkPrefab != null)
        {
            GameObject sparks = Instantiate(hitSparkPrefab, 
                transform.position + Vector3.up * 0.5f, 
                Quaternion.identity);
            
            // Auto-destroy after 1 second
            Destroy(sparks, 0.5f);
        }

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