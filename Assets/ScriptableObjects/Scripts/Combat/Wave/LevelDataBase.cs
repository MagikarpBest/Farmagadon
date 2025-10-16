using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "Game/LevelDatabase")] 
public class LevelDatabase : ScriptableObject
{
    // An array that holds all level configurations (each LevelData = 1 level)
    // You fill this manually in the Unity Inspector by dragging all your LevelData assets here
    public LevelData[] allLevels;

    /// <summary>
    /// Returns the LevelData object for a given level number.
    /// Handles invalid numbers gracefully by returning the last level instead of crashing
    /// </summary>
    public LevelData GetLevelData(int levelNumber)
    {
        // Check that the requested level number is within the valid range
        if (levelNumber <= 0 || levelNumber > allLevels.Length)
        {
            Debug.LogWarning($"Level {levelNumber} is out of range! Returning last level.");
            return allLevels[allLevels.Length - 1];
        }
        return allLevels[levelNumber - 1];
    }
}
