using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Requirements")]
    public int requiredPlayerLevel = 3; // Open only at this level or higher

    [Header("What happens on enter")]
    public Transform playerSpawnPoint; // Where player teleports to in new world

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (ProgressionManager.Instance.currentPlayerLevel >= requiredPlayerLevel)
        {
            // Advance world
            ProgressionManager.Instance.AdvanceToNextWorld();

            // Tell ChunkManager to switch
            FindFirstObjectByType<ChunkManager>().SwitchToWorldLevel(ProgressionManager.Instance.currentWorldLevel);

            // Teleport player to fresh start
            if (playerSpawnPoint != null)
            {
                other.transform.position = playerSpawnPoint.position;
            }

            // Optional: VFX, sound, destroy portal
            Destroy(gameObject);
        }
        else
        {
            // Optional: UI hint "Need level X"
        }
    }
}