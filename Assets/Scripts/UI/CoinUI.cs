using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Image coinIcon;

    private void Start()
    {
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        // ✅ NULL SAFE
        if (coinText != null && PlayerInventory.Instance != null)
        {
            coinText.text = PlayerInventory.Instance.GetCoinCount().ToString();
        }
        else
        {
            Debug.LogWarning("CoinUI: Missing Text or Inventory!");
        }
    }
}