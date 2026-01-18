using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI countText;
    
    private InventorySlot slotData;
    
    public void UpdateSlot(InventorySlot slot)
    {
        slotData = slot;
        
        if (slot == null || slot.item == null || slot.currentCount <= 0)
        {
            iconImage.enabled = false;
            countText.text = "";
            return;
        }
        
        // Show item!
        iconImage.enabled = true;
        iconImage.sprite = slot.item.icon;
        countText.text = slot.currentCount > 1 ? slot.currentCount.ToString() : "";
    }
}