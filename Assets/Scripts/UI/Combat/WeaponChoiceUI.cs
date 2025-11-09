using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class WeaponChoiceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;                     // Reward choice panel
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private GameObject[] rewardCards;             // Display for reward cards
    [SerializeField] private Image[] rewardIcons;                  // Display for weapon icon
    [SerializeField] private TextMeshProUGUI[] weaponNames;        // Display for weapon names
    [SerializeField] private TextMeshProUGUI[] weaponDescription;
    [SerializeField] private WeaponDatabase weaponDatabase;

    private Action<string> onWeaponChosen;

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void Show(List<string> weaponIDs, Action<string> onChosen)
    {
        if (panel == null)
        {
            Debug.LogError("WeaponChoiceUIManager: No panel assigned!");
            return;
        }

        if (weaponIDs == null || weaponIDs.Count == 0)
        {
            Debug.LogWarning("WeaponChoiceUIManager: No weapon options provided!");
            return;
        }

        onWeaponChosen = onChosen;
        panel.SetActive(true);

        // Setup button options
        for (int i = 0; i < rewardCards.Length; i++)
        {
            if (i < weaponIDs.Count)
            {
                string weaponID = weaponIDs[i];
                var data = weaponDatabase.GetWeaponByID(weaponID);
                rewardCards[i].SetActive(true);

                // Update button label if available
                if (weaponNames != null && i < weaponNames.Length && weaponNames[i] != null && data != null) 
                {
                    weaponNames[i].text = data.weaponName;
                }

                // Set weapon icon from database
                if (rewardIcons != null && i < rewardIcons.Length && rewardIcons[i] != null && data != null && data.weaponSprite != null) 
                {
                    rewardIcons[i].sprite = data.weaponSprite;
                }

                if (weaponDescription != null && i < weaponDescription.Length && weaponDescription[i] != null && data != null)
                {
                    weaponDescription[i].text = data.weaponDescription;
                }

                // Reset previous listeners and add new one
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => ChooseWeapon(weaponID));
            }
            else
            {
                rewardCards[i].gameObject.SetActive(false);
            }
        }
        // Delay and force select 1 frame after cuz fuck unity ui garbage bug
        StartCoroutine(SelectAfterFrame());
    }
    private IEnumerator SelectAfterFrame()
    {
        if (choiceButtons.Length > 0 && choiceButtons[0] != null)
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(null); // Clear old selection
            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject); // Force-select first button
            Debug.Log("Default reward button selected: " + choiceButtons[0].name);
        }
    }

    /// <summary>
    /// Called when the player picks a weapon button.
    /// </summary>
    private void ChooseWeapon(string weaponID)
    {
        panel.SetActive(false);
        onWeaponChosen.Invoke(weaponID);
        onWeaponChosen = null;
    }
}
