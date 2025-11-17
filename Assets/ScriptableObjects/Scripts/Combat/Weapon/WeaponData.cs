using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Weapons")]
public class WeaponData : ScriptableObject
{
    public string weaponID; 
    public string weaponName;
    public string weaponDescription;
    public Sprite weaponSprite;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public AmmoData ammoType;
    public float lifeTime = 0f;
    public float bulletSpeed = 10f;
    public float damage = 1f;
    public int pierceCount = 1; // Idk why 1 is 0 but yes, set default to 1 so it wont break
    public float explosionRadius = 0f; // Edit if explosive weapon


    [Header("Fire Settings")]
    public float fireRate = 0.2f;
    public int pelletCount = 1; // Set to more than 1 if shotgun
    public float spread = 0; // Set to more than 1 if shotgun

    [Serializable]
    public class ShrapnelSettings
    {
        [Header("Shrapnel Settings (special combination)")]
        public bool enable = false;
        public int count = 0;
        public GameObject bulletPrefab; // Which bullet prefab to shoot (eg rice after explode)
        public WeaponData shrapnelWeaponData;
    }

    [Header("Explosion / Shrapnel Settings")]
    public ShrapnelSettings shrapnel = new ShrapnelSettings();

    [Serializable]
    public class SlowExplosion
    {
        [Header("Slow effects")]
        public bool enable = false;
        [Range(0f, 1f)] public float slowAmount = 0.5f;
        public float duration = 1.0f;
    }

    [Header("Slow Zone Effect")]
    public SlowExplosion slowEffect = new SlowExplosion();

    [Serializable]
    public class SplitSettings
    {
        [Header("Split-on-Hit")]
        public bool enable = false;
        public GameObject bulletPrefab; // Which bullet prefab to shoot (eg rice after explode)
        public WeaponData splitWeaponData;
        public float spreadAngle = 45f;
    }

    [Header("Split Settings")]
    public SplitSettings split = new SplitSettings();
}
