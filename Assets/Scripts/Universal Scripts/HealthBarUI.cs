using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Colors")]
    [SerializeField] private Color maxHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    private Health playerHealth;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("HealthBarUI: No Player with Health component found! Add 'Player' tag.");
        }
    }

    void Update()
    {
        if (playerHealth == null) return;

        float fillAmount = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        
        healthFill.fillAmount = fillAmount;                    // ← FILL BAR
        healthFill.color = Color.Lerp(lowHealthColor, maxHealthColor, fillAmount);  // ← COLOR
        healthText.text = $"{playerHealth.CurrentHealth} / {playerHealth.MaxHealth}";  // ← TEXT (ONCE)
    }
}