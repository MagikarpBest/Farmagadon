using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] FarmController farmController;
    public struct TimeData 
    {
        public float CurrentTime;
        public float MaxDuration;
        public readonly float NormalizedTime => CurrentTime / MaxDuration;

        public TimeData(float currentTime, float maxDuration)
        {
            CurrentTime = currentTime;
            MaxDuration = maxDuration;
        }
    }

    public delegate void TimeUpdateCallback(TimeData data);
    public TimeUpdateCallback OnTimeDecreased;
    public TimeUpdateCallback OnTimeFinished;

    [SerializeField] private float dayTimeDuration;
    private float currentTime, maxDuration;
    private float gameStartTime;
    private bool timerStarted = false;

    private void Awake()
    {
        currentTime = maxDuration = dayTimeDuration;
        gameStartTime = Time.time;

        
        
    }
    private void OnEnable()
    {

        farmController.OnFarmStart += startTime;
    }
    private void OnDisable()
    {
        if (farmController.OnFarmStart != null)
        {
            farmController.OnFarmStart -= startTime;
        }   
    }

    public void AddTimeDecreasedListener(TimeUpdateCallback callback)
    {
        OnTimeDecreased += callback;
        OnTimeDecreased?.Invoke(new(currentTime, maxDuration));
    }
    public void RemoveTimeDecreasedListener(TimeUpdateCallback callback)
    {
        OnTimeDecreased -= callback;
    }
    private void decreaseTime()
    {
        gameStartTime = Time.time;
        currentTime -= (1.0f * Time.deltaTime);
        OnTimeDecreased?.Invoke(new(currentTime, maxDuration));
        timeDepleted();
    }

    private void startTime()
    {
        timerStarted = true;
    }
    private void timeDepleted()
    {
        if (currentTime <= 0.0f)
        {
            timerStarted = false;
            farmController.StopGame = true; // stop game bool here
            farmController.OnFarmEnd?.Invoke();
            Debug.Log("Time delpleted");
        }
    }
    private void Update()
    {
        if (timerStarted)
        {
            decreaseTime();
        }
    }

}
