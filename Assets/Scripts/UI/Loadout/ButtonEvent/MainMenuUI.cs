using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject creditMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject blocker;
    [SerializeField] private Button creditCloseButton;
    [SerializeField] private Button optionCloseButton;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private GameObject volumeSlider;

    [SerializeField] private CircleTransition circleTransition;


    private SaveData saveData;
    private float switchDelay = 0.8f;

    [SerializeField] private AudioClip clip;
    public void Start()
    {
        creditMenu.SetActive(false);
        optionMenu.SetActive(false);
        blocker.SetActive(false);

        startButton.onClick.AddListener(StartActive);
        creditButton.onClick.AddListener(CreditActive);
        optionButton.onClick.AddListener(OptionActive);
        creditCloseButton.onClick.AddListener(CreditInactive);
        optionCloseButton.onClick.AddListener(OptionInactive);
        saveData = SaveSystem.LoadGame();
        StartCoroutine(circleTransition.GoingOutTransition());
    }

    private void Update()
    {
        if (!optionMenu.activeSelf) return;

        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == null)
        {
            EventSystem.current.SetSelectedGameObject(volumeSlider);
        }
        //prevent losing focus while submit on slider
        if (current == volumeSlider)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
            {

            }
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {

            }
        }

    }
    public void StartActive()
    {
        SaveSystem.ClearSave();
        saveData = SaveSystem.LoadGame();
    }

    public void CreditActive()
    {
        GameObject creditClose = creditCloseButton.gameObject;
        creditMenu.SetActive(true);
        blocker.SetActive(true);
        StartCoroutine(SelectOnNextFrame(creditClose));
    }
    public void OptionActive()
    {
        optionMenu.SetActive(true);
        blocker.SetActive(true);
        StartCoroutine(SelectOnNextFrame(volumeSlider));
    }
    public void CreditInactive()
    {
        GameObject creditBtn = creditButton.gameObject;
        creditMenu.SetActive(false);
        blocker.SetActive(false);
        StartCoroutine(SelectOnNextFrame(creditBtn));
    }
    public void OptionInactive()
    {
        GameObject optionBtn = optionButton.gameObject;
        optionMenu.SetActive(false);
        blocker.SetActive(false);
        StartCoroutine(SelectOnNextFrame(optionBtn));
        
    }

    public void OnClickSound()
    {
        AudioService.AudioManager.PlayOneShot(clip, 1f);
    }

    private IEnumerator SelectOnNextFrame(GameObject obj)
    {
        yield return null; // wait 1 frame
        EventSystem.current.SetSelectedGameObject(obj);
    }

    public void Switch()
    {
        AudioService.AudioManager.FadeOutBGM();
        StartCoroutine(SwitchDelay(switchDelay));
    }

    public IEnumerator SwitchDelay(float delay)
    {
        yield return circleTransition.GoingInTransition();
        SceneManager.LoadScene(GetSceneName());
    }

    private string GetSceneName()
    {
        switch (saveData.currentPhase)
        {
            case GamePhase.Farm:
                Debug.Log("Farm phase detected");
                return "FarmScene";

            case GamePhase.Loadout:
                Debug.Log("Loadout phase detected");
                return "LoadOut";

            case GamePhase.Combat:
                Debug.Log("Combat phase detected");
                return "CombatScene";

            default:
                Debug.LogWarning($"Unknown game phase: {saveData.currentPhase}. Returning to FarmScene.");
                return "FarmScene";
        }
    }
}
