using UnityEngine;
using UnityEngine.UI;
public class MenuUI : MonoBehaviour
{
    [SerializeField] public GameObject Menu;
    [SerializeField] public GameObject Blocker;
    [SerializeField] public Button closeButton;
    public void Start()
    {
        Menu.SetActive(false);
        Blocker.SetActive(false);
        closeButton.onClick.AddListener(Inactive);
    }

    public void Active()
    {
        Menu.SetActive(true);
        Blocker.SetActive(true);
    }
    public void Inactive()
    {
        Menu.SetActive(false);
        Blocker.SetActive(false);
    }
}
