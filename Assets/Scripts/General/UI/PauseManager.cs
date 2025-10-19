using UnityEngine;
using UnityEngine.InputSystem;
public class PauseGame : MonoBehaviour
{
    [SerializeField] public GameObject PauseMenuUI;
    [SerializeField] public GameInput gameInput;
    private InputAction pauseAction;
    bool isPaused = false;

    private void Start()
    {
        PauseMenuUI.SetActive(false);
        gameInput.OnPause += Pause;
    }
    private void Pause()
    {
        if (isPaused)
        {
            Resume();
        }
        else if (!isPaused)
        {
            Pausing();
        }
    }

    private void Pausing()
    {
        Time.timeScale = 0.0f;
        PauseMenuUI.SetActive(true);
        isPaused = true;
    }

    private void Resume()
    {
        Time.timeScale = 1.0f;
        PauseMenuUI.SetActive(false);
        isPaused = false;
    }
}
