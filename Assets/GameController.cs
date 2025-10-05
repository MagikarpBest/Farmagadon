using UnityEngine;
using System.Collections.Generic;
public class GameController : MonoBehaviour
{
    public delegate void StartGame();
    public StartGame gameStart;

    public delegate void GetNextWave(List<string> enemies);
    public GetNextWave nextWave;

    private void Awake()
    {
        
    }
    public void Start()
    {
        gameStart?.Invoke();
        getWave();
    }

    private void getWave()
    {
        List<string> enemies = new List<string>();
        enemies.Add("Test");
       
        
        nextWave?.Invoke(enemies);
    }
}
