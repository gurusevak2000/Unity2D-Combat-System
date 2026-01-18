using UnityEngine;
using System.Collections.Generic;

public class InventoryPanelUI : MonoBehaviour
{
    [Header("Grid Setup")]
    public Transform slotParent;     // Drag your grid panel here
    public GameObject slotPrefab;    // Your ItemSlot prefab
    
    private InventorySlotUI[] slotUIs;
    
    private void Start()
    {
        // Create 20 slots automatically
        slotUIs = new InventorySlotUI[20];
        for (int i = 0; i < 20; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            slotUIs[i] = slotObj.GetComponent<InventorySlotUI>();
        }
    }
    
    public void RefreshInventory()
    {
        // Get data from PlayerInventory
        List<InventorySlot> allSlots = PlayerInventory.Instance.GetAllSlots();
        
        // Update each UI slot
        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i < allSlots.Count)
                slotUIs[i].UpdateSlot(allSlots[i]);
            else
                slotUIs[i].UpdateSlot(null);
        }
    }
}