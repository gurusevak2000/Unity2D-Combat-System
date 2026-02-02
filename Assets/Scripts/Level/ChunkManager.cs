using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Level Progression")]
    [SerializeField] private List<LevelData> levels = new List<LevelData>();

    private int currentLevelIndex = 0;
    private int currentChunkIndex = 0;

    private LevelData CurrentLevel => 
        (levels != null && currentLevelIndex < levels.Count && currentLevelIndex >= 0) 
            ? levels[currentLevelIndex] 
            : null;

    [Header("Spawning Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private int initialChunks = 3;
    
    // LOWER THIS! 3–4 is usually perfect for 2D platformers
    [SerializeField] private int maxActiveChunks = 4;  

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float spawnBuffer = 8f;  // Increased a bit for safety
    private float levelLeftBoundary = float.MinValue;
    [Header("Camera Integration")]
    [SerializeField] private CameraFollowWithLookAhead cameraFollow;

    private Queue<LevelChunk> activeChunks = new Queue<LevelChunk>();
    private Transform lastEndPoint;

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        if (CurrentLevel == null || CurrentLevel.ChunkCount == 0)
        {
            Debug.LogError($"No level data for Level {currentLevelIndex + 1}");
            return;
        }
        if (cameraFollow == null)
        cameraFollow = FindFirstObjectByType<CameraFollowWithLookAhead>();
        
        // Set initial right bound
        UpdateCameraRightBound();

        ResetChunkProgress();
        SpawnInitialChunks();
    }

    private void Update()
    {
        if (mainCamera == null || lastEndPoint == null || CurrentLevel == null) return;

        float cameraRightEdge = mainCamera.transform.position.x +
                                mainCamera.orthographicSize * mainCamera.aspect;

        // Spawn next chunk when needed
        if (cameraRightEdge + spawnBuffer >= lastEndPoint.position.x)
        {
            SpawnNextChunkInSequence();
        }

        // EXTRA SAFETY: Distance-based cleanup (in case queue fails)
        CleanUpFarChunks();
    }

    private void SpawnInitialChunks()
    {
        for (int i = 0; i < initialChunks && currentChunkIndex < CurrentLevel.ChunkCount; i++)
        {
            SpawnNextChunkInSequence();
            
            // After spawning first chunk → remember left edge
            if (i == 0 && lastEndPoint != null)
            {
                // assuming startPoint is roughly the left side
                levelLeftBoundary = transform.position.x - 2f;   // tune offset
            }
        }
    }

    private void SpawnNextChunkInSequence()
    {
        if (currentChunkIndex >= CurrentLevel.ChunkCount)
        {
            // Optional: Loop level or stop spawning
            // Debug.Log("End of level reached!");
            return;
        }

        LevelChunk prefab = CurrentLevel.GetChunkAt(currentChunkIndex);
        if (prefab == null)
        {
            Debug.LogError($"Null chunk at index {currentChunkIndex}!");
            currentChunkIndex++;
            return;
        }

        Vector3 spawnPos = lastEndPoint == null 
            ? Vector3.zero 
            : lastEndPoint.position - prefab.startPoint.localPosition;

        LevelChunk newChunk = Instantiate(prefab, spawnPos, Quaternion.identity);
        lastEndPoint = newChunk.endPoint;
        activeChunks.Enqueue(newChunk);

        currentChunkIndex++;
        UpdateCameraRightBound();

        // CRITICAL: Remove old chunks when limit exceeded
        while (activeChunks.Count > maxActiveChunks)
        {
            LevelChunk oldChunk = activeChunks.Dequeue();
            if (oldChunk != null)
            {
                Destroy(oldChunk.gameObject);
            }
        }
    }

    // Extra safety net: destroy chunks far behind the player
    private void CleanUpFarChunks()
    {
        if (player == null) return;

        float destroyThreshold = player.position.x - 50f; // Destroy anything 50 units behind player

        while (activeChunks.Count > 0)
        {
            LevelChunk oldest = activeChunks.Peek();
            if (oldest.transform.position.x + 20f < destroyThreshold) // +20f buffer for chunk width
            {
                activeChunks.Dequeue();
                Destroy(oldest.gameObject);
            }
            else
            {
                break; // Since queue is in order, no need to check further
            }
        }
    }

    public void SwitchToWorldLevel(int newWorldLevel) // 1-based
    {
        int newIndex = newWorldLevel - 1;
        if (newIndex < 0 || newIndex >= levels.Count)
        {
            Debug.LogError($"Invalid level {newWorldLevel}");
            return;
        }

        // FULL CLEANUP — destroy everything old
        while (activeChunks.Count > 0)
        {
            var chunk = activeChunks.Dequeue();
            if (chunk != null) Destroy(chunk.gameObject);
        }

        currentLevelIndex = newIndex;
        ResetChunkProgress();
        SpawnInitialChunks();

        Debug.Log($"Switched to Level {newWorldLevel}");
    }

    private void ResetChunkProgress()
    {
        currentChunkIndex = 0;
        lastEndPoint = null;
        // activeChunks.Clear(); → already cleared in SwitchToWorldLevel
    }
    private void UpdateCameraRightBound()
    {
        if (cameraFollow != null && lastEndPoint != null)
        {
            // Extend right boundary 20 units past current end point
            float newRightBound = lastEndPoint.position.x;
            cameraFollow.UpdateMaxCameraX(newRightBound);
            
            //Debug.Log($"📹 Camera right bound extended to: {newRightBound:F1}");
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Force Cleanup All Chunks")]
    private void EditorForceCleanup()
    {
        while (activeChunks.Count > 0)
        {
            var c = activeChunks.Dequeue();
            if (c != null) DestroyImmediate(c.gameObject);
        }
    }
#endif
}   