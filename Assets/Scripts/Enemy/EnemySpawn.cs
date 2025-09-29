using UnityEngine;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] int totalWave = 1;
    [SerializeField] int totalEnemy = 30;
    private int enemyCount = 0;
    public int maxOnScreen = 10;
    private float spawnTime;
    private float lastSpawnTime = 0.0f;
    public GameObject enemyPrefab;
    private List<GameObject> enemiesOnScreen = new List<GameObject>();

    int waveCount;

    private void Start()
    {

    }
    private void Update()
    {
        if (enemiesOnScreen.Count < maxOnScreen)
        {
            Spawn();
            Debug.Log(enemiesOnScreen.Count);
        }
        else
        {
            return;
        }
    }

    private void Wave()
    {

    }

    private void Spawn()
    {
        spawnTime = Random.Range(1.0f,3.0f);
        lastSpawnTime += Time.deltaTime;
        //only spawn if total count of enemy spawned is less than 30
        if (enemyCount < totalEnemy)
        {
            if (lastSpawnTime > spawnTime)
            {
                lastSpawnTime -= spawnTime;
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.position = new Vector3(Random.Range(-4.2f, 4.2f), 5.5f, 0.0f);

                enemiesOnScreen.Add(enemy);
                //Set manager for enemy die so can remove from list 
                enemy.GetComponent<Enemy>().SetManager(this);
                enemyCount++;
                Debug.Log(enemyCount);
            }
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemiesOnScreen.Contains(enemy))
        {
            enemiesOnScreen.Remove(enemy);
        }
    }
}
