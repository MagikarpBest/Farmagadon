using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private Image leftWeaponImage;
    [SerializeField] private Image centerWeaponImage;
    [SerializeField] private Image rightWeaponImage;
    [SerializeField] private Image bottomWeaponImage; // 4th slot 
    [SerializeField] private Sprite emptySlotSprite;

    private void OnEnable()
    {
        if (weaponInventory != null)
        {
            weaponInventory.OnWeaponChanged += UpdateUI;

        }
    }
    private void OnDisable()
    {
        if (weaponInventory != null)
        {
            weaponInventory.OnWeaponChanged -= UpdateUI;
        }
    }

    private void Start()
    {
        if (weaponInventory != null)
        {
            UpdateUI(weaponInventory.GetWeaponSlot(weaponInventory.GetCurrentWeaponIndex()));
        }
            
    }

    private void UpdateUI(WeaponSlot currentSlot)
    {
        if (weaponInventory == null || currentSlot == null)  
        {
            Debug.LogWarning("[WeaponUI] Missing inventory or current slot.");
            return;
        }

        // Always show all slots (up to max)
        int totalSlots = weaponInventory.UnlockedSlotCount; // using maxSlot for full UI display
        List<WeaponSlot> allSlots = new();

        for (int i = 0; i < totalSlots; i++)
        {
            allSlots.Add(weaponInventory.GetWeaponSlot(i)); // may include nulls (locked or empty)
        }

        if (allSlots.Count == 0)
        {
            SetEmptyAll();
            return;
        }

        // Find current index within equipped list
        int currentIndex = allSlots.IndexOf(currentSlot);

        // --- Wrap-around neighbors ---
        WeaponSlot centerSlot = allSlots[currentIndex];
        WeaponSlot rightSlot = allSlots[(currentIndex + 1) % totalSlots];
        WeaponSlot bottomSlot = allSlots[(currentIndex + 2) % totalSlots];
        WeaponSlot leftSlot = allSlots[(currentIndex + 3) % totalSlots];

        // Assign Images
        SetImage(centerWeaponImage, centerSlot);
        SetImage(rightWeaponImage, rightSlot);
        SetImage(bottomWeaponImage, bottomSlot);
        SetImage(leftWeaponImage, leftSlot);
    }

    private void SetImage(Image image, WeaponSlot slot)
    {
        if (image == null)
        {
            return;
        }

        if (slot != null && slot.weaponData != null & slot.weaponData.weaponSprite != null)
        {
            image.sprite = slot.weaponData.weaponSprite;
        }
        else
        {
            image.sprite = emptySlotSprite;
        }
    }

    private void SetEmptyAll()
    {
        SetImage(leftWeaponImage, null);
        SetImage(centerWeaponImage, null);
        SetImage(rightWeaponImage, null);
        SetImage(bottomWeaponImage, null);
    }

}
