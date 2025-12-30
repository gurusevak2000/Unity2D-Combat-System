using UnityEngine;
using UnityEngine.SceneManagement;


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

    public void TakeDamage(int damage)
    {
        // Always play hit feedback
        animator.SetTrigger("Hit");

        // Find camera once (modern + fast way)
        var camFollow = Object.FindFirstObjectByType<CameraFollowWithLookAhead>();

        // Shake if we found it
        if (camFollow != null)
            camFollow.TriggerShake(0.15f, 0.5f);

        // Sparks
        if (hitSparkPrefab != null)
        {
            GameObject sparks = Instantiate(hitSparkPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity);
            Destroy(sparks, 0.5f);
        }

#if ENABLE_CHEATS
        // CHEAT: Infinite Health → ignore damage completely
        if (CheatManager.Instance != null && CheatManager.Instance.InfiniteHealth)
        {
            return; // Exit early — no damage applied
        }
#endif

        // Normal damage (only runs if no infinite health cheat)
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (gameObject.CompareTag("Enemy"))
            {
                var manager = FindFirstObjectByType<ProgressionManager>();  
                if (manager != null)
                {
                    manager.AddXp(50);
                }
                else
                {
                    Debug.LogWarning("ProgressionManager not found in scene!");
                }
            }

            Die();
        }
    }

    private void Die()
    {
#if ENABLE_CHEATS
        if (CheatManager.Instance != null && CheatManager.Instance.InfiniteHealth)
            return;
#endif

        animator.SetTrigger("Die");
        Invoke(nameof(DestroySelf), deathDelay);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void SetMaxHealth(int newMax, bool healToFull = false)
    {
        maxHealth = newMax;
        if (healToFull) currentHealth = maxHealth;
        else currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}