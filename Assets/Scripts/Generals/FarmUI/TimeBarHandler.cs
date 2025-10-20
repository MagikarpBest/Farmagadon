using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;



public class TimeBarHandler : MonoBehaviour
{
    [SerializeField] Timer initialTimerBinding;
    [SerializeField] Slider timeSlider;

    private Timer timerBinding;

    private void Start()
    {
        if (initialTimerBinding  != null)
        {
            BindTo(initialTimerBinding);
        }
    }

    private void OnEnable()
    {
        if (timerBinding != null)
        {
            timerBinding.AddTimeDecreasedListener(TimerBinding_OnTimerDecreased);
        }
        
    }
    private void OnDisable()
    {
        if (timerBinding != null)
        {
            timerBinding.RemoveTimeDecreasedListener(TimerBinding_OnTimerDecreased);
        }
    }

    private void BindTo(Timer newTimerBinding)
    {
        if (timerBinding != null)
        {
            timerBinding.RemoveTimeDecreasedListener(TimerBinding_OnTimerDecreased);
        }

        timerBinding = newTimerBinding;

        if (timerBinding != null)
        {
            timerBinding.AddTimeDecreasedListener(TimerBinding_OnTimerDecreased);
        }
    }

    private void TimerBinding_OnTimerDecreased(Timer.TimeData data)
    {
        timeSlider.value = data.NormalizedTime;
    }
}
