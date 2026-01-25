using UnityEngine;
using UnityEngine.Events;

public class Portal : MonoBehaviour
{
    [Header("Boss Trigger")]
    [SerializeField] private Health bossHealth;
    [SerializeField] private float openDelayAfterBossDeath = 1.5f;

    [Header("Animations (Match Your Sprite Sheet)")]
    private Animator portalAnimator;
    [SerializeField] private float openDuration = 1.2f;   // Your Open clip length
    [SerializeField] private float closeDuration = 1.2f;  // Your Close clip length

    [Header("Requirements (Your Old)")]
    public int requiredPlayerLevel = 3;
    [Header("What happens on enter (Your Old)")]
    public Transform playerSpawnPoint;

    private enum PortalState { Closed, Opening, Idle, Closing }
    private PortalState state = PortalState.Closed;
    private Collider2D playerCollider;

    private void Awake()
    {
        portalAnimator = GetComponentInChildren<Animator>();
        gameObject.SetActive(false); // HIDDEN
    }

    private void Start()
    {
        // Auto-find boss OR assign in inspector
        if (bossHealth == null)
            bossHealth = FindFirstObjectByType<Health>(); // Finds first enemy boss

        Debug.Log($"Portal found boss: {bossHealth != null}");
    }

    // 🔥 Called from Boss Health.OnDeath Event
    public void OpenPortal()
    {
        if (state != PortalState.Closed) return;

        gameObject.SetActive(true);
        state = PortalState.Opening;

        // Delay + Open animation
        Invoke(nameof(PlayOpenAnimation), openDelayAfterBossDeath);
    }

    private void PlayOpenAnimation()
    {
        portalAnimator.SetTrigger("Open");
        
        // Auto → Idle after Open finishes
        Invoke(nameof(EnterIdleState), openDuration);
    }

    private void EnterIdleState()
    {
        state = PortalState.Idle;
        portalAnimator.SetTrigger("Idle");
        Debug.Log("<color=cyan>🌀 Portal OPEN! Ready for loot collection...</color>");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || state != PortalState.Idle) return;

        playerCollider = other;
        state = PortalState.Closing;

        // ────────────────────────────────
        //  Make player disappear immediately
        // ────────────────────────────────
        SpriteRenderer playerSprite = other.GetComponentInChildren<SpriteRenderer>();
        if (playerSprite != null)
        {
            playerSprite.enabled = false;           // Option 1: completely hide (recommended)
            // playerSprite.color = new Color(1,1,1,0);  // Option 2: just make transparent
        }

        // Optional: prevent player movement / physics during transition
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        // If your player has a movement script with EnableMovement method:
        // PlayerScript player = other.GetComponent<PlayerScript>();
        // if (player != null) player.EnableMovement(false);

        portalAnimator.SetTrigger("Close");

        Invoke(nameof(LoadNextLevel), closeDuration);
    }

    private void LoadNextLevel()
    {
        if (ProgressionManager.Instance.currentPlayerLevel >= requiredPlayerLevel)
        {
            // 1. Advance world (your old code)
            ProgressionManager.Instance.AdvanceToNextWorld();

            // 2. Teleport player **before** switching chunks
            if (playerSpawnPoint != null && playerCollider != null)
            {
                playerCollider.transform.position = playerSpawnPoint.position;
            }

            // 3. Switch chunks / load new world
            FindFirstObjectByType<ChunkManager>().SwitchToWorldLevel(ProgressionManager.Instance.currentWorldLevel);

            // 4. Show player again
            if (playerCollider != null)
            {
                SpriteRenderer playerSprite = playerCollider.GetComponentInChildren<SpriteRenderer>();
                if (playerSprite != null)
                {
                    playerSprite.enabled = true;           // show again
                    // playerSprite.color = new Color(1,1,1,1); // if you used alpha fade
                }

                Rigidbody2D rb = playerCollider.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = true;

                // If you disabled movement:
                // PlayerScript player = playerCollider.GetComponent<PlayerScript>();
                // if (player != null) player.EnableMovement(true);
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"Need Level {requiredPlayerLevel}!");

            // Important: show player again if portal didn't work
            if (playerCollider != null)
            {
                SpriteRenderer playerSprite = playerCollider.GetComponentInChildren<SpriteRenderer>();
                if (playerSprite != null)
                {
                    playerSprite.enabled = true;
                }
            }

            state = PortalState.Idle;
        }
    }
}