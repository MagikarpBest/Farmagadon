using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] WeaponInventory weaponInventory;
    [SerializeField] TextMeshProUGUI weaponNameText;

    private void OnEnable()
    {
        weaponInventory.OnWeaponChanged += UpdateUI;
    }
    private void OnDisable()
    {
        weaponInventory.OnWeaponChanged -= UpdateUI;
    }

    private void UpdateUI(WeaponSlot slot)
    {
        if (slot != null && slot.weaponData != null)
        {
            weaponNameText.text = slot.weaponData.weaponName;
        }
        else
        {
            weaponNameText.text = "No Weapon";
        }
    }
}
