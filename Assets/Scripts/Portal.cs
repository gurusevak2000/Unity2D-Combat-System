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
        portalAnimator.SetTrigger("Close");

        // Wait for Close anim → Load Level (YOUR OLD LOGIC)
        Invoke(nameof(LoadNextLevel), closeDuration);
    }

    private void LoadNextLevel()
    {
        // YOUR EXACT OLD CODE
        if (ProgressionManager.Instance.currentPlayerLevel >= requiredPlayerLevel)
        {
            ProgressionManager.Instance.AdvanceToNextWorld();
            FindFirstObjectByType<ChunkManager>().SwitchToWorldLevel(ProgressionManager.Instance.currentWorldLevel);

            if (playerSpawnPoint != null)
                playerCollider.transform.position = playerSpawnPoint.position;

            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"Need Level {requiredPlayerLevel}!");
            state = PortalState.Idle; // Back to idle
        }
    }
}