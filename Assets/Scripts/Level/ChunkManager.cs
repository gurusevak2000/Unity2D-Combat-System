using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Settings - World Level 1")]
    [SerializeField] private List<LevelChunk> world1Chunks;

    [Header("Chunk Settings - World Level 2")]
    [SerializeField] private List<LevelChunk> world2Chunks;

    // Add more world lists here as needed...

    private int currentWorldIndex = 0; // 0-based

    private List<LevelChunk> CurrentChunkList
    {
        get
        {
            return currentWorldIndex switch
            {
                0 => world1Chunks,
                1 => world2Chunks,
                // Add more...
                _ => world1Chunks // fallback to world 1
            };
        }
    }

    [SerializeField] private Transform player;
    [SerializeField] private int initialChunks = 3;
    [SerializeField] private int maxChunks = 5;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float spawnBuffer = 5f;

    private Queue<LevelChunk> activeChunks = new Queue<LevelChunk>();
    private Transform lastEndPoint;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        var currentList = CurrentChunkList;
        if (currentList == null || currentList.Count == 0)
        {
            Debug.LogError($"ChunkManager: No chunks assigned for World Level {currentWorldIndex + 1}");
            return;
        }

        SpawnInitialChunks();
    }

    private void Update()
    {
        if (mainCamera == null || lastEndPoint == null) return;

        float cameraRightEdge = mainCamera.transform.position.x +
                                mainCamera.orthographicSize * mainCamera.aspect;

        if (cameraRightEdge + spawnBuffer >= lastEndPoint.position.x)
        {
            SpawnChunk();
        }
    }

    private void SpawnInitialChunks()
    {
        for (int i = 0; i < initialChunks; i++)
        {
            SpawnChunk();
        }
    }

    private void SpawnChunk()
    {
        var currentList = CurrentChunkList;
        if (currentList == null || currentList.Count == 0)
        {
            Debug.LogError("ChunkManager: Current chunk list is empty or null!");
            return;
        }

        LevelChunk prefab = currentList[Random.Range(0, currentList.Count)];
        if (prefab == null)
        {
            Debug.LogError("ChunkManager: A chunk prefab in the list is null!");
            return;
        }

        LevelChunk newChunk;

        if (lastEndPoint == null)
        {
            newChunk = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Vector3 spawnPos = lastEndPoint.position - prefab.startPoint.localPosition;
            newChunk = Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        lastEndPoint = newChunk.endPoint;
        activeChunks.Enqueue(newChunk);

        if (activeChunks.Count > maxChunks)
        {
            var oldChunk = activeChunks.Dequeue();
            if (oldChunk != null)
                Destroy(oldChunk.gameObject);
        }
    }

    public void SwitchToWorldLevel(int newWorldLevel) // 1-based
    {
        int newIndex = newWorldLevel - 1;

        List<LevelChunk> targetList = newIndex switch
        {
            0 => world1Chunks,
            1 => world2Chunks,
            // Add more cases here if needed
            _ => null
        };

        if (targetList == null || targetList.Count == 0)
        {
            Debug.LogError($"World Level {newWorldLevel} has no chunk prefabs assigned or invalid index!");
            return;
        }

        // Clear old chunks
        while (activeChunks.Count > 0)
        {
            var chunk = activeChunks.Dequeue();
            if (chunk != null)
                Destroy(chunk.gameObject);
        }

        lastEndPoint = null;
        currentWorldIndex = newIndex;

        SpawnInitialChunks();
    }
}