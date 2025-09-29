using UnityEngine;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] int totalWave = 3;
    [SerializeField] int enemyPerWave = 10;
    [SerializeField] float timeBetweenWave = 5.0f;

    private int currWave = 0;
    private int enemyCount = 0;
    private float spawnTime;
    private float lastSpawnTime = 0.0f;
    private bool isCurrWaveDone = false;
    private bool isAllWaveDone = false;
    private bool goingNextWave = false;

    public GameObject enemyPrefab;
    

    int waveCount;

    private void Start()
    {
        Wave();
    }

    private void Update()
    {
        if (isAllWaveDone)
        {
            return;
        }
        if (!isCurrWaveDone)
        {
            Spawn();
        }
        else if (!goingNextWave)
        {
            goingNextWave = true;
            Invoke("Wave", timeBetweenWave);
        }
    }

    private void Wave()
    {
        currWave++;
        if (currWave > totalWave)
        {
            isAllWaveDone = true;
            Debug.Log("Waves Done");
            return;
        }
        enemyCount = 0;
        isCurrWaveDone = false;
        goingNextWave = false;
        
        Debug.Log(currWave);

    }

    private void Spawn()
    {
        spawnTime = Random.Range(1.0f,3.0f);
        lastSpawnTime += Time.deltaTime;
        //only spawn if total count of enemy spawned is less than 30
        if (enemyCount < enemyPerWave)
        {
            if (lastSpawnTime > spawnTime)
            {
                lastSpawnTime -= spawnTime;
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.position = new Vector3(Random.Range(-4.2f, 4.2f), 5.5f, 0.0f);
                enemyCount++;
                Debug.Log(enemyCount);
            }
        }
        else
        {
            isCurrWaveDone = true;
            Debug.Log($"Wave {currWave} finished!");
        }
    }
}
