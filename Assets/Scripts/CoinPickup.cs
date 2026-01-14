using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private int coinValue = 1;
    //[SerializeField] private GameObject pickupEffectPrefab;

    [Header("Visual Effects")]
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Bobbing animation (satisfying feedback!)
        transform.position = startPos + Vector3.up * 
            Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // CONNECT TO INVENTORY (Step 2)
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.AddCoins(coinValue);

        // Particles (optional - create simple ParticleSystem prefab)
        //if (pickupEffectPrefab != null)
        //    Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

        // AUDIO (optional - add AudioSource + coin sound)
        Destroy(gameObject);
    }

    // Scene view helper
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}