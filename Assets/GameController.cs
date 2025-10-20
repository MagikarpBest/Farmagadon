using UnityEngine;
using System.Collections.Generic;
public class GameController : MonoBehaviour
{
    public delegate void StartGame();
    public StartGame gameStart;

    

    private void Awake()
    {
        
    }
    public void Start()
    {
        gameStart?.Invoke();
    }

   
}
