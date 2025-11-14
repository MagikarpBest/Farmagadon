using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadOutManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private WeaponInventory weaponInventory;

    [Header("Inventory Slots Reference")]
    [SerializeField] private List<Button> inventorySlots;

    [Header("Popup Prefabs")]
    [SerializeField] private GameObject equipPopupUI;
    [SerializeField] private GameObject craftPopupUI;

    private int selectedInventoryIndex = 0;
    private Button lastSelectedButton; // store the button that opened equip popup

    private GameObject activePopup; // Only one popup active at a time


    private void OnEnable()
    {
        if (gameInput != null)
        {
            gameInput.OnPause += CloseActivePopup;
        }
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.OnPause -= CloseActivePopup;
        }
    }

    private void Start()
    {
        // Setup button read index on click
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            int index = i;
            inventorySlots[index].onClick.AddListener(() => selectedInventoryIndex = index);
        }
    }

    public void Interact()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        Button selectedButton = selected.GetComponent<Button>();
        if (selectedButton == null) return;

        selectedInventoryIndex = inventorySlots.IndexOf(selectedButton);
        if (selectedInventoryIndex == -1) return;

        OpenEquipPopup(selectedButton);
        Debug.Log("NIGGA");
        Debug.Log(inventorySlots[selectedInventoryIndex]);
    }

    private void OpenEquipPopup(Button inventoryButton)
    {
        if (equipPopupUI == null) return;

        lastSelectedButton = inventoryButton;
        
        // Save loadout last selected
        UINavigationMemory loadoutNav = loadoutPanel?.GetComponent<UINavigationMemory>();
        if (loadoutNav != null)
        {
            loadoutNav.DeactivateUI();
        }

        // Activate popup
        equipPopupUI.SetActive(true);
        activePopup = equipPopupUI;

        // Position popup to the right of the selected button
        RectTransform buttonRect = inventoryButton.GetComponent<RectTransform>();
        RectTransform popupRect = equipPopupUI.GetComponent<RectTransform>();

        // Position popup to the right of the inventory button
        popupRect.position = buttonRect.position + new Vector3(200f, 0f, 0f);

        // Set first button in popup as selected
        UINavigationMemory popupNav = activePopup.GetComponent<UINavigationMemory>();
        if (popupNav != null)
        {
            popupNav.ActivateUI();
        }
    }

    private void CloseEquipPopup()
    {
        if (activePopup != equipPopupUI) return;

        UINavigationMemory popupNav = activePopup.GetComponent<UINavigationMemory>();
        if (popupNav != null)
        {
            popupNav.DeactivateUI();
        }
        // Clear selection to trigger OnDeselect
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        equipPopupUI.SetActive(false);
        activePopup = null;

        // Restore loadout navigation focus
        UINavigationMemory loadoutNav = loadoutPanel.GetComponent<UINavigationMemory>();
        if (loadoutNav != null)
        {
            loadoutNav.ActivateUI();
        }
    }

    private void OpenCraftingPopup()
    {
        if (craftPopupUI == null || activePopup != equipPopupUI) return;

        // Hide equip popup but remember it as previous
        equipPopupUI.SetActive(false);

        craftPopupUI.SetActive(true);
        activePopup = craftPopupUI;

        // Position craft popup at same location as equip popup
        RectTransform buttonRect = lastSelectedButton.GetComponent<RectTransform>();
        RectTransform craftRect = craftPopupUI.GetComponent<RectTransform>();
        craftRect.position = buttonRect.position + new Vector3(200f, 0f, 0f);

        UINavigationMemory craftNav = craftPopupUI.GetComponent<UINavigationMemory>();
        if (craftNav != null)
        {
            craftNav.ActivateUI();
        }
    }

    private void CloseCraftingPopup()
    {
        if (activePopup != craftPopupUI) return;

        craftPopupUI.SetActive(false);
        activePopup = null;

        // Restore equip popup
        equipPopupUI.SetActive(true);
        activePopup = equipPopupUI;
        UINavigationMemory craftNav = craftPopupUI.GetComponent<UINavigationMemory>();
        if (craftNav != null)
        {
            craftNav.DeactivateUI();
        }
    }

    private void CloseActivePopup()
    {
        if (activePopup == craftPopupUI)
        {
            CloseCraftingPopup();
        }
        else if (activePopup == equipPopupUI)
        {
            CloseEquipPopup();
        }
    }

    private void RefreshUI()
    {

    }
}