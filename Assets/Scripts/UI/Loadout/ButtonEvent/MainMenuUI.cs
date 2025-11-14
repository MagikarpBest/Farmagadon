using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject creditMenu;
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject blocker;
    [SerializeField] private Button creditCloseButton;
    [SerializeField] private Button optionCloseButton;

    [SerializeField] private Button creditButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private GameObject volumeSlider;
    public void Start()
    {
        creditMenu.SetActive(false);
        optionMenu.SetActive(false);
        blocker.SetActive(false);
        creditButton.onClick.AddListener(CreditActive);
        optionButton.onClick.AddListener(OptionActive);
        Debug.Log("aaaaaaaaaa");
        creditCloseButton.onClick.AddListener(CreditInactive);
        optionCloseButton.onClick.AddListener(OptionInactive);
    }

    public void CreditActive()
    {
        creditMenu.SetActive(true);
        blocker.SetActive(true);
    }
    public void OptionActive()
    {
        optionMenu.SetActive(true);
        blocker.SetActive(true);
        StartCoroutine(SelectOnNextFrame(volumeSlider));
        Debug.Log("option");
    }
    public void CreditInactive()
    {
        creditMenu.SetActive(false);
        blocker.SetActive(false);
        
    }
    public void OptionInactive()
    {
        GameObject optionBtn = optionButton.gameObject;
        optionMenu.SetActive(false);
        blocker.SetActive(false);
        StartCoroutine(SelectOnNextFrame(optionBtn));
        
    }

    private IEnumerator SelectOnNextFrame(GameObject obj)
    {
        yield return null; // wait 1 frame
        EventSystem.current.SetSelectedGameObject(obj);
    }
}
