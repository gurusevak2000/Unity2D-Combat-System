using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelText;   // Or legacy Text
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI xpText;     

    private LevelingData levelingData; // Cache for calculations

    private void OnEnable()
    {
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.onPlayerLevelUp.AddListener(UpdateUI_WithAmount);
            ProgressionManager.Instance.onXpGained.AddListener(UpdateUI_WithAmount);
            levelingData = ProgressionManager.Instance.GetComponent<ProgressionManager>().levelingData; // Or expose publicly
            UpdateUI(); // Initial update
        }
    }

    private void OnDisable()
    {
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.onPlayerLevelUp.RemoveListener(UpdateUI_WithAmount);
            ProgressionManager.Instance.onXpGained.RemoveListener(UpdateUI_WithAmount);
        }
    }

    public void UpdateUI_WithAmount(int amount)
    {
        int level = ProgressionManager.Instance.currentPlayerLevel;
        int currentXp = ProgressionManager.Instance.currentXp;

        // Update level text
        if (levelText != null)
            levelText.text = $"Level: {level}";

        // Calculate XP for current and next level
        int xpForCurrentLevel = levelingData.GetXpForLevel(level);
        int xpForNextLevel = levelingData.GetXpForLevel(level + 1);
        int xpIntoCurrentLevel = currentXp - xpForCurrentLevel;
        int xpNeededForNext = xpForNextLevel - xpForCurrentLevel;

        // Slider (0 to 1)
        if (xpSlider != null)
        {
            float fill = xpNeededForNext > 0 ? (float)xpIntoCurrentLevel / xpNeededForNext : 1f;
            xpSlider.value = fill;
        }

        // Optional detailed text
        if (xpText != null)
            xpText.text = $"{xpIntoCurrentLevel} / {xpNeededForNext} XP";
    }

    public void UpdateUI()
    {
        UpdateUI_WithAmount(0);           // calls the version below
    }
}