using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("What item will player get")]
    public Item item;
    public int amount = 1;

    [Header("Visual feedback")]
    [SerializeField] private float bobSpeed = 1.8f;
    [SerializeField] private float bobHeight = 0.25f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // simple floating effect
        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPosition + new Vector3(0, y, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Safety check
        if (item == null)
        {
            Debug.LogWarning("ItemPickup has no item assigned!", this);
            return;
        }

        bool success = PlayerInventory.Instance?.TryAddItem(item, amount) ?? false;

        if (success)
        {
            // Optional: sound, particles, animation...
            Debug.Log($"Picked up {amount}× {item.itemName}");
            Destroy(gameObject);
        }
    }
}