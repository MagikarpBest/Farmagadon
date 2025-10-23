 using UnityEngine;

[CreateAssetMenu(menuName = "Wave System/Level Data", fileName = "NewLevel")]
public class LevelData : ScriptableObject
{
    public float totalDuration = 90f; // Whole level length
    public WaveEvent[] events; // timeline of wave
}

[System.Serializable]
public class WaveEvent
{
    public float time;               // When this happen
    public WavePatternData wavePattern;  // What pattern to follow
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;   // What to spawn
    public int count;                // How much it spawn
    public float duration;           // How long the interval to spawn 1?
}