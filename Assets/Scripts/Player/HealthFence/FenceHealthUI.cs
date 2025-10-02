using UnityEngine;
using UnityEngine.UI;

public class FenceHealthUI : MonoBehaviour
{
    [SerializeField] private FenceHealth fenceHealth;
    [SerializeField] private Slider fenceHealthBar;

    private void OnEnable()
    {
        fenceHealth.OnHealthChanged += HandleHealthUpdated;
    }

    private void OnDisable()
    {
        fenceHealth.OnHealthChanged -= HandleHealthUpdated;
    }

    private void Start()
    {
        if (fenceHealth != null)
        {
            HandleHealthUpdated(fenceHealth.GetHealth(), fenceHealth.GetMaxHealth());
        }
    }

    private void HandleHealthUpdated(int current, int max)
    {
        if (fenceHealth != null)
        {
            fenceHealthBar.maxValue = max;
            fenceHealthBar.value = current;
        }
    }
}
