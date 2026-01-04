using UnityEngine;

public class PlayerStatsScaler : MonoBehaviour
{
    [SerializeField] private Health health;

    [Header("Stat Scaling")]
    public int baseMaxHealth = 5;
    public int healthPerLevel = 2; // +2 max HP per player level (you can tweak this)

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        // Try immediately
        TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.onPlayerLevelUp.AddListener(ApplyStats);
            ApplyStats(ProgressionManager.Instance.currentPlayerLevel);
        }
        else
        {
            // Retry after a brief delay — gives ProgressionManager time to awaken
            Invoke(nameof(TrySubscribe), 0.2f);
        }
    }

    private void OnDisable()
    {
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.onPlayerLevelUp.RemoveListener(ApplyStats);
        }
    }

    // THIS IS THE METHOD THAT WAS MISSING!
    private void ApplyStats(int newLevel)
    {
        if (health == null)
        {
            Debug.LogError("Health component not found on Player!");
            return;
        }

        int newMaxHp = baseMaxHealth + (newLevel - 1) * healthPerLevel;
        health.SetMaxHealth(newMaxHp, healToFull: false);
        
        // Optional: Debug to confirm it's working
        Debug.Log($"Player leveled up to {newLevel}! Max Health increased to {newMaxHp}");
    }
}