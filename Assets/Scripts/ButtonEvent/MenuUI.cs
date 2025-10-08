using UnityEngine;
using UnityEngine.UI;
public class MenuUI : MonoBehaviour
{
    [SerializeField] public GameObject Menu;
    [SerializeField] public Button button;
    private bool isActive = false;
    public void Start()
    {
        Menu.SetActive(false);
    }

    public void OnClick()
    {
        if (!isActive)
        {
            Active();
            isActive = true;
        }
        else if (isActive)
        {
            Inactive();
            isActive = false;
        }
    }

    private void Active()
    {
        Menu.SetActive(true);
    }
    private void Inactive()
    {
        Menu.SetActive(false);
    }
}
