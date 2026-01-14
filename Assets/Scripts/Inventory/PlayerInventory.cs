using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Coins")]
    [SerializeField] private int coins = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        
        coins += amount;
        //Debug.Log($"<color=yellow>💰 Collected {amount} coins! Total: {coins}</color>");

        UpdateAllCoinUIs();
    }

    private void UpdateAllCoinUIs()
    {
        // AllocateExact = PERFECT for small arrays like UI
        CoinUI[] coinUIs = Object.FindObjectsByType<CoinUI>(FindObjectsSortMode.None);
        
        //Debug.Log($"📊 Found {coinUIs.Length} CoinUI components"); // DEBUG
        
        foreach (CoinUI ui in coinUIs)
        {
            ui.UpdateCoinDisplay();
        }
    }

    public int GetCoinCount() => coins;

    // ✅ BONUS: Manual UI update (call from other scripts)
    public void RefreshCoinUI()
    {
        UpdateAllCoinUIs();
    }
}