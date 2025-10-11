using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Player player;

    private bool gameEnded = false;

    private void Awake()
    {
        // Safety check
        if (uiManager == null)
        {
            uiManager =FindFirstObjectByType<UIManager>();
        }
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }

    }
}
