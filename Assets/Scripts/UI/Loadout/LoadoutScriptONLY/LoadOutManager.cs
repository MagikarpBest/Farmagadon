using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private CraftManager craftManager;

    [Header("Inventory Slots Reference")]
    [SerializeField] private List<Button> inventorySlots;   

    [Header("Popup Prefabs")]
    [SerializeField] private GameObject equipPopupUI;
    [SerializeField] private Button equipButton;
    [SerializeField] private TextMeshProUGUI equipButtonText;
    [SerializeField] private GameObject craftButton;
    [SerializeField] private GameObject craftPopupUI;

    // EVENT
    public Action<int> OnInventorySlotChanged;
    public Action OnTriggerUIUpdate;

    private int selectedInventoryIndex = 0;
    private Button lastSelectedButton; // store the button that opened equip popup
    private GameObject activePopup; // Only one popup active at a time
    private List<WeaponSlot> allOwned;


    // For navigation memory and button navigation logics
    private UINavigationMemory loadoutNav;
    private UINavigationMemory equipNav;
    private UINavigationMemory craftNav;

    private void OnEnable()
    {
        if (gameInput != null)
        {
            gameInput.OnPause += () => CloseActivePopup(true);
        }
    }

    private void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.OnPause -= () => CloseActivePopup(true);
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        // Setup button read index on click
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            int index = i;
            inventorySlots[index].onClick.AddListener(() => selectedInventoryIndex = index);
        }
        
        allOwned = weaponInventory.GetAllOwnedWeapons();
        loadoutNav = loadoutPanel.GetComponent<UINavigationMemory>();
        equipNav = equipPopupUI.GetComponent<UINavigationMemory>();
        craftNav = craftPopupUI.GetComponent<UINavigationMemory>();
    }

    // Clean slot index selection function
    // Call by OnSlotSelected (for item description) and Interact (loadout buttons)
    private void SelectSlot(int index)
    {
        selectedInventoryIndex = index;
        OnInventorySlotChanged?.Invoke(selectedInventoryIndex);
    }

    public void OnSlotSelected(Button selectedButton)
    {
        int index = inventorySlots.IndexOf(selectedButton);

        if (index == -1)
        {
            Debug.LogWarning("Selected button is not in inventorySlots list!");
            return;
        }

        SelectSlot(index);
    }

    // Assign to button for interact
    public void Interact()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        Button selectedButton = selected.GetComponent<Button>();
        if (selectedButton == null) return;

        int index = inventorySlots.IndexOf(selectedButton);
        if (index == -1) return;

        SelectSlot(index);
        OpenEquipPopup(selectedButton);
        Debug.Log(inventorySlots[selectedInventoryIndex]);
    }

    private void EquipSelectedWeapon()
    {
        if (selectedInventoryIndex < 0 || selectedInventoryIndex >= allOwned.Count)
        {
            Debug.LogWarning("Selected inventory index is out of range!");
            return;
        }

        WeaponSlot selectedWeapon = allOwned[selectedInventoryIndex];
        List<WeaponSlot> equippedWeapon = weaponInventory.GetOnlyEquippedWeapon();

        for (int i = 0; i < equippedWeapon.Count; i++)
        {
            Debug.Log($"{equippedWeapon[i].weaponData.weaponName}");
        }

        weaponInventory.EquipWeapon(selectedWeapon);
        
        CloseActivePopup();
        OnTriggerUIUpdate?.Invoke();
    }

    private void UnequipSelectedWeapon()
    {
        if (selectedInventoryIndex < 0 || selectedInventoryIndex >= allOwned.Count)
        {
            Debug.LogWarning("Selected inventory index is out of range!");
            return;
        }

        WeaponSlot selectedWeapon = allOwned[selectedInventoryIndex];

        if (selectedWeapon == null || selectedWeapon.weaponData == null)
        {
            Debug.LogWarning("Selected weapon data is null!");
            return;
        }

        // Use weapon ID to find its slot in equipped array
        weaponInventory.UnEquipWeaponByID(selectedWeapon.weaponData.weaponID);

        CloseActivePopup();
        OnTriggerUIUpdate?.Invoke();
    }

    #region Equip Popup
    private void OpenEquipPopup(Button inventoryButton)
    {
        // Prevent trying to open popup for an empty slot
        if (selectedInventoryIndex < 0 || selectedInventoryIndex >= allOwned.Count)
        {
            Debug.LogWarning("Tried to open equip popup on an empty slot.");
            return;
        }
        lastSelectedButton = inventoryButton;
        // Save loadout last selected
        SwitchNavigation(loadoutNav, false);

        CloseActivePopup(false);

        // Activate popup
        equipPopupUI.SetActive(true);
        activePopup = equipPopupUI;

        WeaponSlot selected = allOwned[selectedInventoryIndex];

        bool isEquipped = weaponInventory.IsWeaponEquipped(selected);
        bool isCraftable = selected.weaponData.ammoType.canBeCrafted;

        equipButton.onClick.RemoveAllListeners();
        if (isEquipped) 
        {
            equipButtonText.text = "Unequip";
            equipButton.onClick.AddListener(UnequipSelectedWeapon);
        }
        else
        {
            equipButtonText.text = "Equip";
            equipButton.onClick.AddListener(EquipSelectedWeapon);
        }
        
        if (!isCraftable)
        {
            craftButton.SetActive(false);
        }

        // Position poup
        PositionPopup(equipPopupUI, inventoryButton);

        SwitchNavigation(equipNav, true);
    }
    #endregion Equip Popup

    #region Crafting Popup
    /// <summary>
    /// Assign on button
    /// </summary>
    private void OpenCraftingPopup()
    {
        if (selectedInventoryIndex < 0 || selectedInventoryIndex >= allOwned.Count)
        {
            Debug.LogWarning("Selected inventory index is out of range!");
            return;
        }

        WeaponSlot selectedWeapon = allOwned[selectedInventoryIndex];

        // Hide equip popup but remember it as previous
        SwitchNavigation(equipNav, false);
        CloseActivePopup(false);

        PositionPopup(craftPopupUI, lastSelectedButton);
        craftPopupUI.SetActive(true);
        craftManager.OpenCraft(selectedWeapon.weaponData.ammoType);
        activePopup = craftPopupUI;

        SwitchNavigation(craftNav, true);
    }
    #endregion Crafting

    #region Helpers
    private void CloseActivePopup(bool returnToLoadout = true)
    {
        if (activePopup == craftPopupUI)
        {
            craftPopupUI.SetActive(false);
            activePopup = equipPopupUI;

            // Restore equip popup
            equipPopupUI.SetActive(true);

            SwitchNavigation(craftNav, false);
            SwitchNavigation(equipNav, true);
            return;
        }
        else if (activePopup == equipPopupUI)
        { 
            if (returnToLoadout)
            {
                // Clear selection to trigger OnDeselect
                // IMPORTANT OR ELSE WHEN OPEN WILL HAVE VISUAL ERROR
                ClearSelection();

                // Restore loadout navigation focus
                SwitchNavigation(loadoutNav, true);
            }
            craftButton.SetActive(true);
            equipPopupUI.SetActive(false);
            activePopup = null;

            SwitchNavigation(equipNav, false);
        }
    }

    private void SwitchNavigation(UINavigationMemory navigation, bool active)
    {
        if (navigation == null)
        {
            return;
        }

        if (active)
        {
            navigation.ActivateUI();
        }
        else
        {
            navigation.DeactivateUI();
        }
    }

    private void ClearSelection()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void PositionPopup(GameObject popup, Button sourceButton)
    {
        RectTransform Button = sourceButton.GetComponent<RectTransform>();
        RectTransform popupLocation = popup.GetComponent<RectTransform>();

        popupLocation.position = Button.position + new Vector3(200f, 0f, 0f);
    }
    #endregion
}