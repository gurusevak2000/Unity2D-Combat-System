using UnityEngine;
using UnityEngine.Events;
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
    public UnityEvent OnDeath = new UnityEvent();
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
        // ────────────────────────────────────────────────
        //  NEW: DASH INVINCIBILITY CHECK
        // ────────────────────────────────────────────────
        if (CompareTag("Player"))
        {
            PlayerScript player = GetComponent<PlayerScript>();
            if (player != null && player.IsInvincible)
            {
                // Optional: visual/audio feedback that attack was "blocked"
                // animator.SetTrigger("DashBlock");  // if you want later
                // AudioSource.PlayClipAtPoint(blockSound, transform.position);
                return;  // ← completely ignore damage during dash
            }
        }

        // ────────────────────────────────────────────────
        //  Existing code below — only runs if NOT invincible
        // ────────────────────────────────────────────────

        // Always play hit feedback (shake + sparks)
        animator.SetTrigger("Hit");
        var camFollow = Object.FindFirstObjectByType<CameraFollowWithLookAhead>();
        if (camFollow != null)
            camFollow.TriggerShake(0.15f);

        if (hitSparkPrefab != null)
        {
            GameObject sparks = Instantiate(hitSparkPrefab,
                transform.position + Vector3.up * 0.5f,
                Quaternion.identity);
            Destroy(sparks, 0.5f);
        }

    #if ENABLE_CHEATS
        // PLAYER-ONLY Infinite Health
        if (CheatManager.Instance != null && CheatManager.Instance.PlayerInfiniteHealth && CompareTag("Player"))
        {
            return; // Player invincible (cheat mode)
        }
    #endif

        // Normal damage
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
    #if ENABLE_CHEATS
        if (CompareTag("Player") && CheatManager.Instance != null && CheatManager.Instance.PlayerInfiniteHealth)
            return;
    #endif

        // Decide XP reward
        int xpReward = 0;

        if (CompareTag("Enemy"))
            xpReward = 50;
        else if (CompareTag("BossEnemy"))
            xpReward = 500;           // or 1000, 2000, whatever makes sense

        if (xpReward > 0 && ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.AddXp(xpReward);
            string type = CompareTag("BossEnemy") ? "Boss" : "Enemy";
            Debug.Log($"<color=green>💀 {type} died → +{xpReward} XP</color>");
        }

        OnDeath?.Invoke();

        if (animator != null)
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