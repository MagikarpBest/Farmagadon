using UnityEngine;
using System.Collections.Generic;

public class AssetKeeper : MonoBehaviour
{
    // The key is to declare a list of the asset type that is being stripped.
    // In the Inspector, drag ALL your WeaponData ScriptableObjects here.
    [SerializeField] private List<WeaponData> allGameWeaponData;

    // Unity won't strip anything referenced by a DontDestroyOnLoad object.
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        foreach (var weaponData in allGameWeaponData)
        {
            if (weaponData.weaponSprite != null)
            {
                Debug.Log($"Keeper found sprite for: {weaponData.name}");
            }
        }
    }
}