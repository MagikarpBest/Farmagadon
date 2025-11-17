using System;
using UnityEngine;
using UnityEngine.UI;

public class FarmTimer : MonoBehaviour
{
    [SerializeField] FarmController farmController;
    [SerializeField] Slider slider;
    [SerializeField] private float dayTimeDuration;

    private float currentTime, maxDuration;
    private float gameStartTime;
    private bool timerStarted = false;

    public event Action OnTimerEnded;

    private void OnEnable()
    {
        farmController.StartFarmCycle += StartTime;
        currentTime = maxDuration = dayTimeDuration;
        gameStartTime = Time.time;
    }
    private void OnDisable()
    {
        
       farmController.StartFarmCycle -= StartTime;
        
    }

    private void StartTime()
    {
        timerStarted = true;
    }

    private void StopTime()
    {
        timerStarted = false;
    }

    private void DecreaseTime()
    {
        gameStartTime = Time.time;
        currentTime -= (1.0f * Time.deltaTime);
        slider.value = currentTime / maxDuration;
        TimeDepleted();
    }

    
    private void TimeDepleted()
    {
        if (currentTime <= 0.0f)
        {
            StopTime();
            OnTimerEnded?.Invoke();
            //farmController.gameEnd?.Invoke();
            
            Debug.Log("Time delpleted");
        }
    }
    private void Update()
    {
        if (timerStarted)
        {
            DecreaseTime();
        }
    }
}
