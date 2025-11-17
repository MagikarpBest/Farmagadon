using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponDescription;
    [SerializeField] private WeaponDatabase weaponDatabase;

    public void Show(string weaponID)
    {
        if (panel == null)
        {
            Debug.LogError("SimpleWeaponDisplayUI: No panel assigned!");
            return;
        }

        var data = weaponDatabase.GetWeaponByID(weaponID);
        if (data == null)
        {
            Debug.LogError($"SimpleWeaponDisplayUI: No data found for weapon ID: {weaponID}");
            return;
        }
        // Show panel
        panel.SetActive(true);
        // Set the UI elements
        if (weaponName != null)
        {
            weaponName.text = data.weaponName;
        }

        if (rewardIcon != null && data.weaponSprite != null)
        {
            rewardIcon.sprite = data.weaponSprite;
        }

        if (weaponDescription != null)
        {
            weaponDescription.text = data.weaponDescription;
        }
    }
}