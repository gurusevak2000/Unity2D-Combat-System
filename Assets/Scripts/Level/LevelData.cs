using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData_1", menuName = "Level System/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [Tooltip("All chunks in this level, in the exact order they should spawn")]
    public List<LevelChunk> chunksInOrder = new List<LevelChunk>();

    [Tooltip("Optional: Name for easier identification in editor")]
    public string levelName = "Level 1";

    public int ChunkCount => chunksInOrder.Count;

    public LevelChunk GetChunkAt(int index)
    {
        if (chunksInOrder == null || chunksInOrder.Count == 0)
            return null;

        return chunksInOrder[Mathf.Clamp(index, 0, chunksInOrder.Count - 1)];
    }
}