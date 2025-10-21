using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Handles level completion reward (choose one or auto give)
/// </summary>
public class LevelRewardManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private WeaponDatabase weaponDatabase;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WeaponChoiceUI weaponChoiceUI;
    [SerializeField] private AmmoInventory ammoInventory;      // To add ammo debug

    [Header("Rewards Data")]
    [SerializeField] private LevelRewardData rewardData;
    [SerializeField] private int debugOnReceiveAmmoAmount = 50;

    public event Action OnRewardGiven;

    private void Awake()
    {
        ValidateReference();
    }

    /// <summary>
    /// Check is any of the refenrece missing
    /// </summary>
    private void ValidateReference()
    {
        Debug.Assert(weaponInventory != null, $"{name}: Missing WeaponInventory reference!");
        Debug.Assert(weaponDatabase != null, $"{name}: Missing WeaponDatabase reference!");
        Debug.Assert(uiManager != null, $"{name}: Missing UIManager reference!");
        Debug.Assert(rewardData != null, $"{name}: Missing LevelRewardData reference!");
        Debug.Assert(ammoInventory != null, $"{name}: Missing Ammo reference!");
    }

    public void GetRewardForLevel(int level)
    {
        // Search the rewardData list and find the reward entry that matches the given level number
        if (rewardData == null)
        {
            Debug.LogError("[Reward] RewardData is null — cannot give rewards!");
            return;
        }

        // Find reward for this level
        var reward = rewardData.rewards.Find(rewards => rewards.levelNumber == level);
        if (reward == null)
        {
            Debug.LogWarning($"[Reward] No reward set for this level{level}");
        }

        // Filter out already owned weapons
        var availableRewards = GetUnownedWeapons(reward.rewardWeaponIDs);
        if (availableRewards.Count == 0)
        {
            Debug.Log($"[Reward] Player already owned all rewards");
            return;
        }

        if (reward.chooseOne)
        {
            // Pick up to 3 random unique choices
            int choiceCount = Mathf.Min(3, availableRewards.Count);
            List<string> randomChoices = new List<string>();

            // Copy list to avoid modifying the original
            List<string> pool = new List<string>(availableRewards);

            for (int i = 0; i < choiceCount; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, pool.Count);
                randomChoices.Add(pool[randomIndex]);
                pool.RemoveAt(randomIndex);
            }

            // Show a weapon choice popup
            weaponChoiceUI.Show(randomChoices, OnWeaponChosen);
        }
        else
        {
            uiManager.Show(UIScreen.Victory);

            // Give all reward directly
            foreach (var id in availableRewards)
            {
                GetWeaponReward(id);
            }
        }
    }

    /// <summary>
    /// Filters out owned or equipped weapons
    /// </summary>
    private List<string> GetUnownedWeapons(List<string> rewardWeaponIDs)
    {
        var unowned = new List<string>();

        foreach (var id in rewardWeaponIDs)
        {
            bool alreadyOwned = weaponInventory.IsWeaponOwned(id);
            if (!alreadyOwned)
            {
                unowned.Add(id);
            }
        }
        return unowned;
    }

    /// <summary>
    /// Handles actually giving the weapon and ammo reward.
    /// Used by both auto and choice modes.
    /// </summary>
    private void GetWeaponReward(string weaponID)
    {
        WeaponData weapon = weaponDatabase.GetWeaponByID(weaponID);
        if (weapon == null)
        {
            Debug.LogWarning("$[Reward] Invadlid weapon ID:{weapon");
            return;
        }

        weaponInventory.AddWeapon(weapon);
        Debug.Log($"[Reward] Granted weapon: {weapon.weaponName}");

        // Debug test give ammo when receive weapon
        // Set to 0 in final game   
        if (weapon.ammoType != null)
        {
            Debug.Log($"[DEBUG] Giving {debugOnReceiveAmmoAmount}x Ammo for {weapon.ammoType.ammoName} ({weapon.weaponName})");
            ammoInventory.AddAmmo(weapon.ammoType, debugOnReceiveAmmoAmount);
        }
    }

    /// <summary>
    /// Called when player selects a weapon from the choice UI.
    /// </summary>
    private void OnWeaponChosen(string chosenID)
    {
        GetWeaponReward(chosenID);
        OnRewardGiven?.Invoke();
    }
}