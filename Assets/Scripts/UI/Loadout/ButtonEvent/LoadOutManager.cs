using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadOutManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private CraftManager craftManager;
    [SerializeField] private WeaponInventory weaponInventory;

    [Header("Inventory Slots Reference")]
    [SerializeField] private List<Button> equippedSlots; 
    [SerializeField] private List<Button> inventorySlots;

    [Header("Popup Prefabs")]
    [SerializeField] private GameObject equipPopupUI;
    [SerializeField] private GameObject craftPopupUI;

    private int selectedInventoryIndex = 0;

    private GameObject activeEquipPopup;
    private GameObject activeCraftPopup;
    

    private void OnEnable()
    {
        if (gameInput != null)
        {
            gameInput.OnPause += CloseEquipPopup;
        }
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.OnPause -= CloseEquipPopup;
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
        if (selectedInventoryIndex < 0 || selectedInventoryIndex >= inventorySlots.Count)
        {
            Debug.Log("NIGG2");
            return;
        }

        Button selectedButton = inventorySlots[selectedInventoryIndex];
        if (activeEquipPopup == null)
        {
            OpenEquipPopup(selectedButton);
        }
        Debug.Log("NIGGA");
    }

    private void OpenEquipPopup(Button inventoryButton)
    {
        if (equipPopupUI == null)
        {
            return;
        }

        // Save loadout last selected
        UINavigationMemory loadoutNav = loadoutPanel.GetComponent<UINavigationMemory>();
        loadoutNav?.DeactivateUI();

        equipPopupUI.SetActive(true);
        activeEquipPopup = equipPopupUI;

        RectTransform buttonRect = inventoryButton.GetComponent<RectTransform>();
        RectTransform popupRect = equipPopupUI.GetComponent<RectTransform>();

        // Position popup to the right of the inventory button
        popupRect.position = buttonRect.position + new Vector3(200f, 0f, 0f);

        // Set first button in popup as selected
        UINavigationMemory popupNav = activeEquipPopup.GetComponent<UINavigationMemory>();
        if (popupNav != null)
        {
            popupNav.ActivateUI();
        }
    }

    private void CloseEquipPopup()
    {
        if (activeEquipPopup == null)
            return;

        UINavigationMemory popupNav = activeEquipPopup.GetComponent<UINavigationMemory>();
        if (popupNav != null)
        {
            popupNav.DeactivateUI();
        }

        activeEquipPopup.SetActive(false);
        activeEquipPopup = null;

        // Restore loadout navigation focus
        UINavigationMemory loadoutNav = loadoutPanel.GetComponent<UINavigationMemory>();
        if (loadoutNav != null)
        {
            loadoutNav.ActivateUI();
        }
    }

    private void RefreshUI()
    {

    }
}