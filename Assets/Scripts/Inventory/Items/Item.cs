using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Basic information")]
    public string itemName = "New Item";
    
    [TextArea(2,4)]
    public string description = "";
    
    [Header("Appearance")]
    public Sprite icon;
    
    [Header("Stacking")]
    public bool canStack = true;
    public int maxStackSize = 99;
}