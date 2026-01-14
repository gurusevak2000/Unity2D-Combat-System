using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    // We keep coins from Phase 1
    public int coins;

    // New: items storage
    public List<InventorySlot> slots = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            DebugShowInventory();
        }
    }

    public bool TryAddItem(Item item, int count)
    {
        if (item == null || count <= 0) return false;

        // Try to stack with existing slot
        foreach (var slot in slots)
        {
            if (slot.item == item && slot.currentCount < item.maxStackSize)
            {
                int canAdd = item.maxStackSize - slot.currentCount;
                int willAdd = Mathf.Min(canAdd, count);

                slot.currentCount += willAdd;
                count -= willAdd;

                Debug.Log($"Added to existing stack: {willAdd}× {item.itemName}");

                if (count <= 0) return true;
            }
        }

        // Create new slot(s)
        while (count > 0)
        {
            int addNow = Mathf.Min(count, item.maxStackSize);
            slots.Add(new InventorySlot(item, addNow));
            count -= addNow;
            Debug.Log($"Created new stack: {addNow}× {item.itemName}");
        }

        return true;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        
        coins += amount;
        CoinUI coinUI = FindFirstObjectByType<CoinUI>();
        if (coinUI != null)
        {
            coinUI.UpdateCoinDisplay();
        }
    }

    public int GetCoinCount() => coins;

    // Very simple way to see content
    public void DebugShowInventory()
    {
        Debug.Log("───────────── INVENTORY ─────────────");
        Debug.Log($"Coins: {coins}");

        if (slots.Count == 0)
        {
            Debug.Log("   (empty)");
        }
        else
        {
            foreach (var slot in slots)
            {
                Debug.Log($"   {slot.currentCount,3}× {slot.item.itemName}");
            }
        }
        Debug.Log("─────────────────────────────────────");
    }
}

[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int currentCount;

    public InventorySlot(Item item, int count)
    {
        this.item = item;
        this.currentCount = count;
    }
}