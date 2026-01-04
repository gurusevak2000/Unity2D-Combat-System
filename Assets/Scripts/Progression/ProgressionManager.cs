using UnityEngine;
using UnityEngine.Events;

public class ProgressionManager : MonoBehaviour
{
    #if UNITY_EDITOR
        [ContextMenu("Debug: Print Current Progress")]
    #endif
    public void DebugProgress()
    {
        Debug.Log($"<color=cyan>🎮 CURRENT PROGRESS:</color>\n" +
                $"Level: {currentPlayerLevel}\n" +
                $"XP: {currentXp}\n" +
                $"XP to next: {XpNeededForNextLevel()}\n" +
                $"Portal ready? {currentPlayerLevel >= 3}");
    }
    public static ProgressionManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] public LevelingData levelingData;

    [Header("Current Progress")]
    public int currentXp = 0;
    public int currentPlayerLevel = 1;
    public int currentWorldLevel = 1;

    // Events for loose coupling
    public UnityEvent<int> onPlayerLevelUp;      // param: new level
    public UnityEvent<int> onWorldLevelUp;       // param: new world level
    public UnityEvent<int> onXpGained;           // param: xp amount

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Calculate initial level from starting XP (if any)
        RecalculatePlayerLevel();
    }

    public void AddXp(int amount)
    {
        if (amount <= 0) return;
        currentXp += amount;
        onXpGained?.Invoke(amount);

        int newLevel = levelingData.GetLevelForXp(currentXp);
        if (newLevel > currentPlayerLevel)
        {
            currentPlayerLevel = newLevel;
            onPlayerLevelUp?.Invoke(currentPlayerLevel);
        }
        
    }

    public void AdvanceToNextWorld()
    {
        currentWorldLevel++;
        onWorldLevelUp?.Invoke(currentWorldLevel);
        // Optional: give bonus XP or heal player here
    }

    private void RecalculatePlayerLevel()
    {
        currentPlayerLevel = levelingData.GetLevelForXp(currentXp);
    }

    // Helper: XP needed for next player level
    public int XpNeededForNextLevel()
    {
        if (currentPlayerLevel > levelingData.xpToNextLevel.Length) return 0;
        int xpForCurrent = levelingData.GetXpForLevel(currentPlayerLevel);
        int xpForNext = levelingData.GetXpForLevel(currentPlayerLevel + 1);
        return xpForNext - xpForCurrent - (currentXp - levelingData.GetXpForLevel(currentPlayerLevel));
    }
}