using UnityEngine;

[CreateAssetMenu(fileName = "LevelingData", menuName = "Progression/Leveling Data")]
public class LevelingData : ScriptableObject
{
    [Header("XP required for each level (index 0 = level 1 to 2)")]
    public int[] xpToNextLevel = new int[] { 100, 200, 350, 500, 700 }; // Easy to extend

    public int GetXpForLevel(int level)
    {
        int total = 0;
        for (int i = 0; i < Mathf.Min(level - 1, xpToNextLevel.Length); i++)
        {
            total += xpToNextLevel[i];
        }
        return total;
    }

    public int GetXpToReachLevel(int targetLevel)
    {
        return GetXpForLevel(targetLevel);
    }

    public int GetLevelForXp(int xp)
    {
        int level = 1;
        int accumulated = 0;
        for (int i = 0; i < xpToNextLevel.Length; i++)
        {
            accumulated += xpToNextLevel[i];
            if (xp < accumulated) break;
            level++;
        }
        if (xp >= accumulated) level = xpToNextLevel.Length + 1; // Max level exceeded
        return level;
    }
}