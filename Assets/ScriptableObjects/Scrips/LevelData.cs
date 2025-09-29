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
    public EnemySpawnInfo[] enemies; // What to spawn
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;      
    public int count;                // How much it spawn
    public float interval;           // How long the interval to spawn 1?
}