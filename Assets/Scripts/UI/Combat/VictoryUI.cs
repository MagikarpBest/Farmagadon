using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class VictoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponDescription;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private WeaponDatabase weaponDatabase;

    public void Show(string weaponID)
    {
        if (uiManager == null)
        {
            Debug.LogError("SimpleWeaponDisplayUI: No uiManager assigned!");
            return;
        }

        var data = weaponDatabase.GetWeaponByID(weaponID);
        if (data == null)
        {
            Debug.LogError($"SimpleWeaponDisplayUI: No data found for weapon ID: {weaponID}");
            return;
        }
        // Show panel
        uiManager.ShowVictory();
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
        StartCoroutine(SelectAfterFrame());
    }
    private IEnumerator SelectAfterFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null); // Clear old selection
        EventSystem.current.SetSelectedGameObject(confirmButton);
        ExecuteEvents.Execute(
            confirmButton,
            new BaseEventData(EventSystem.current),
            ExecuteEvents.selectHandler
        );

    }
}