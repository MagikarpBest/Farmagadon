using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ammo")] 
public class AmmoData : ScriptableObject
{
    public string ammoID;
    public string ammoName;
    public Sprite icon;
    public Sprite cropIcon;

    [Header("Crafting setting")]
    public bool canBeCrafted = false;
    public int amountProduced = 2;

    public List<CraftingRequirement> craftingRequirements = new List<CraftingRequirement>();
    
    [Serializable]
    public class CraftingRequirement
    {
        public AmmoData ammo; // What ammo is needed
        public int amountNeeded = 1;
    }
}
