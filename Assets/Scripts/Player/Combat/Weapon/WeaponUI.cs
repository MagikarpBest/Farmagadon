using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private Image leftWeaponImage;
    [SerializeField] private Image centerWeaponImage;
    [SerializeField] private Image rightWeaponImage;

    private void OnEnable()
    {
        weaponInventory.OnWeaponChanged += UpdateUI;
    }
    private void OnDisable()
    {
        weaponInventory.OnWeaponChanged -= UpdateUI;
    }

    private void UpdateUI(WeaponSlot currentSlot)
    {
        if (weaponInventory == null || currentSlot == null)  
        {
            leftWeaponImage.gameObject.SetActive(false);
            centerWeaponImage.gameObject.SetActive(false);
            rightWeaponImage.gameObject.SetActive(false);
            return;
        }

        // Get all non-empty weapon slots
        var equippedWeapons = weaponInventory.GetEquippedWeapon();

        if (equippedWeapons.Count == 0)
        {
            leftWeaponImage.gameObject.SetActive(false);
            centerWeaponImage.gameObject.SetActive(false);
            rightWeaponImage.gameObject.SetActive(false);
            return;
        }
        // Find current index within equipped list
        int currentIndex =equippedWeapons.IndexOf(currentSlot);

        // Get previous weapon index and next
        int previousIndex = (currentIndex - 1 + equippedWeapons.Count) % equippedWeapons.Count;
        int nextIndex = (currentIndex + 1) % equippedWeapons.Count;

        WeaponSlot previousWeapon = equippedWeapons[previousIndex];
        WeaponSlot nextWeapon = equippedWeapons[nextIndex];

        // Assign sprites
        leftWeaponImage.sprite = previousWeapon?.weaponData?.weaponSprite;
        centerWeaponImage.sprite = currentSlot.weaponData?.weaponSprite;
        rightWeaponImage.sprite = nextWeapon?.weaponData?.weaponSprite;

        // Enable only the images that have a sprite
        leftWeaponImage.gameObject.SetActive(leftWeaponImage.sprite != null);
        centerWeaponImage.gameObject.SetActive(centerWeaponImage.sprite != null);
        rightWeaponImage.gameObject.SetActive(rightWeaponImage.sprite != null);
    }
}
