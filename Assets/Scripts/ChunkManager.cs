using System.Collections.Generic;
using UnityEngine;

// This script is compatible with Unity 6000.2.14f1
public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private List<LevelChunk> chunkPrefabs;
    [SerializeField] private Transform player;
    [SerializeField] private int initialChunks = 3;
    [SerializeField] private int maxChunks = 5;

    private Queue<LevelChunk> activeChunks = new Queue<LevelChunk>();
    private Transform lastEndPoint;

    private void Start()
    {
        SpawnInitialChunks();
    }

    private void Update()
    {
        if (player.position.x > lastEndPoint.position.x - 5f)
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
        LevelChunk prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Count)];
        LevelChunk newChunk;

        if (lastEndPoint == null)
        {
            newChunk = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Vector3 spawnPos =
                lastEndPoint.position -
                prefab.startPoint.localPosition;

            newChunk = Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        lastEndPoint = newChunk.endPoint;
        activeChunks.Enqueue(newChunk);

        if (activeChunks.Count > maxChunks)
        {
            Destroy(activeChunks.Dequeue().gameObject);
        }
    }
}
