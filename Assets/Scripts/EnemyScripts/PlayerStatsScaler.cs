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
        // Safely subscribe — in case ProgressionManager isn't ready yet
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.onPlayerLevelUp.AddListener(ApplyStats);
            // Important: Apply stats immediately using current level
            ApplyStats(ProgressionManager.Instance.currentPlayerLevel);
        }
        else
        {
            // Try again soon if manager not ready (e.g., execution order issue)
            Invoke(nameof(TrySubscribe), 0.1f);
        }
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
            Debug.LogError("ProgressionManager not found! Make sure there's a GameObject with ProgressionManager in the scene.");
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