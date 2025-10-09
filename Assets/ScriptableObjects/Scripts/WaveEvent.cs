using UnityEngine;

[CreateAssetMenu(menuName = "WaveSystem", fileName = "NewWaveEvent")] 
public class WaveEvent : ScriptableObject
{
    [Header("Timing")]
    public float time;               // When this happen
    public EnemySpawnInfo[] enemies; // What to spawn
}
