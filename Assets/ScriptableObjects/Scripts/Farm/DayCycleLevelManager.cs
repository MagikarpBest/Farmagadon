using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "DayCycleLevelManager", menuName = "Crops/DayCycleLevelManager")]
public class DayCycleLevelManager : ScriptableObject
{
    [Header("Day Levels")]
    public DayCycleLevelData[] dayLevels;

    public DayCycleLevelData GetLevelData(int level)
    {
        if (dayLevels.Length <= 0) { return null; }
        return dayLevels[Mathf.Clamp(level, 0, dayLevels.Length-1)];
    }
}
