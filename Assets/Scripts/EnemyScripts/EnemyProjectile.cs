using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 9f;              // faster than typical ground enemy projectiles
    [SerializeField] private float lifetime = 4.5f;         // shorter lifetime → feels more aggressive

    [Header("Damage")]
    [SerializeField] private int damage = 1;                // default 1 — can be 2 on stronger flyers

    [Header("Collision")]
    [SerializeField] private LayerMask whatIsGround;        // walls / platforms
    [SerializeField] private string playerTag = "Player";   // more readable than magic string

    [Header("Visuals / Feedback")]
    [SerializeField] private GameObject hitEffectPrefab;    // sparks / explosion / smoke
    [SerializeField] private bool rotateToVelocity = true;  // looks better when enabled

    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("EnemyProjectile missing Rigidbody2D!", this);
            enabled = false;
            return;
        }

        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 shootDirection, int overrideDamage = -1)
    {
        direction = shootDirection.normalized;
        rb.linearVelocity = direction * speed;

        if (overrideDamage > 0)
            damage = overrideDamage;

        // Face direction (very important for flying enemy feel)
        if (rotateToVelocity)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ── Hit player ────────────────────────────────────────
        if (other.CompareTag(playerTag))
        {
            if (other.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(damage);
            }

            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        // ── Hit ground / wall ─────────────────────────────────
        if (((1 << other.gameObject.layer) & whatIsGround) != 0)
        {
            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            var effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            // Optional: rotate effect to match impact normal (if you have good VFX)
            Destroy(effect, 1.2f);
        }
    }

    // Optional: debug line in scene view
    private void OnDrawGizmosSelected()
    {
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.6f);
            Gizmos.DrawRay(transform.position, rb.linearVelocity.normalized * 1.5f);
        }
    }
}