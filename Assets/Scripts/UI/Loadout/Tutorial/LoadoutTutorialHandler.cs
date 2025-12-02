using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadoutTutorialHandler : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject tutorialPanel;

    private UINavigationMemory tutorialNav;
    private UINavigationMemory inventoryNav;

    private void Start()
    {
        inventoryNav = inventoryPanel.GetComponent<UINavigationMemory>();
        tutorialNav = tutorialPanel.GetComponent<UINavigationMemory>();
    }
    public void CloseUI()
    {
        uiManager.HideLoadoutTutorial();
        tutorialNav.DeactivateUI();
        StartCoroutine(SetInventorySelectedDelayed());
    }

    private IEnumerator SetInventorySelectedDelayed()
    {
        // Wait a frame to let any UI scripts finish
        yield return null;

        if (inventoryNav != null)
        {
            inventoryNav.ActivateUI();
        }
    }
}
