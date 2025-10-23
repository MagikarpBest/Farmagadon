using UnityEngine;
using System;
using System.Collections;
public class WaveManager : MonoBehaviour
{
    // ScriptableObject that stores all wave events for this level
    [SerializeField] private Transform spawnPointA; 
    [SerializeField] private Transform spawnPointB;
    [SerializeField] private LevelDatabase levelDatabase;

    private LevelData levelData;
    private float elapsedTime;
    private int activeEnemies = 0;
    private int nextEventIndex = 0; // Which wave event to trigger next (Go check levelData.cs SO eg.Element 1 is index 1, element 2 is index 2)

    public event Action<float, float> OnTimeUpdated;
    public event Action OnLevelCompleted;

    private Coroutine levelCoroutine;

    public void setLevel(LevelData data)
    {
        levelData = data;
    }
    public void BeginLevel(int levelIndex)
    {
        if (levelDatabase == null)
        {
            Debug.LogError("WaveManager: LevelDatabase not set!");
            return;
        }

        levelData = levelDatabase.GetLevelData(levelIndex);

        if (levelData == null)
        {
            Debug.LogError("WaveManager: LevelData not set before starting level!");
            return;
        }

        if (levelCoroutine != null)
        {
            StopCoroutine(levelCoroutine);
        }

        levelCoroutine = StartCoroutine(LevelRoutine());
    }

    private IEnumerator LevelRoutine()
    {
        elapsedTime = 0f;
        nextEventIndex = 0;

        // Run until total level duration is finished
        while (elapsedTime < levelData.totalDuration)
        {
            // Send current time
            OnTimeUpdated?.Invoke(elapsedTime, levelData.totalDuration);

            // Check if it still have events left AND the current time has reached the next event's time
            if (nextEventIndex < levelData.events.Length && elapsedTime >= levelData.events[nextEventIndex].time) 
            {
                // Grab event from levelData
                // Start spawning enemy (how it works check below)
                StartCoroutine(RunWaveEvent(levelData.events[nextEventIndex]));
                nextEventIndex++;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Level timer complete!");
    }

    private IEnumerator RunWaveEvent(WaveEvent waveEvent)
    {
        // Go through all enemy in a wave (Cuz it might contain more than 1 type of enemy in future)
        foreach (EnemySpawnInfo enemy in waveEvent.wavePattern.enemies)
        {
            // Start spawning all enemy in parrallel
            StartCoroutine(SpawnEnemyGroup(enemy));
        }
        yield return null;
    }

    private IEnumerator SpawnEnemyGroup(EnemySpawnInfo info)
    {
        // Calculate interval between spawns (So you dont need to do it yourself incase you are lazy)
        float interval = info.duration / info.count;

        for (int i = 0; i < info.count; i++)
        {
            // Pick random position between spawnPointA and spawnPointB
            Vector3 spawnPos = Vector3.Lerp(spawnPointA.position, spawnPointB.position, UnityEngine.Random.value);
            GameObject enemyObject = Instantiate(info.enemyPrefab, spawnPos, Quaternion.identity);

            activeEnemies++;

            // Subscribe to enemy death event
            Enemy enemy =enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnDeath += HandleEnemyDeath;
            }
            else
            {
                Debug.LogWarning($"Spawned enemy '{info.enemyPrefab.name}' has no Enemy Script");
            }

            // Wait before spawning new one (idk to prevent stacking i guess)
            yield return new WaitForSeconds(interval);
        }
    }

    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
        Debug.Log(activeEnemies);

        // All wave finished + no enemy alive
        if (activeEnemies == 0 && nextEventIndex >= levelData.events.Length)
        {
            OnLevelCompleted?.Invoke();
            Debug.Log("All enemy died");
        }
    }
}
