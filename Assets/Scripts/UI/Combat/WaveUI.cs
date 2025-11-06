using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Slider waveProgressBar;

    private void OnEnable()
    {
        waveManager.OnTimeUpdated += HandleTimeUpdated;
        waveManager.OnLevelCompleted += HandleLevelCompleted;
    }

    private void HandleTimeUpdated(float elapsedTime, float totalDuration)
    {
        if (waveProgressBar != null)
        {
            waveProgressBar.value = elapsedTime / totalDuration;
        }
    }

    private void HandleLevelCompleted()
    {
        if (waveProgressBar != null) 
        {
            waveProgressBar.value = 1f;
        }
    }
}
