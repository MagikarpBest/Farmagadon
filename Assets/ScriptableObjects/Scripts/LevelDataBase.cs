using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "Game/LevelDatabase")] 
public class LevelDatabase : ScriptableObject
{
    public LevelData[] allLevels;

    public LevelData GetLevelData(int levelNumber)
    {
        if (levelNumber <= 0 || levelNumber > allLevels.Length)
        {
            Debug.LogWarning($"Level {levelNumber} is out of range! Returning last level.");
            return allLevels[allLevels.Length - 1];
        }
        return allLevels[levelNumber - 1];
    }
}
