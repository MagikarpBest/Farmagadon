using UnityEngine;

[CreateAssetMenu(menuName = "Weapons")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
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

    [System.Serializable]
    public class ShrapnelSettings
    {
        [Header("Shrapnel Settings (special combination)")]
        public bool enable = false;
        public GameObject bulletPrefab; // Which bullet prefab to shoot (eg rice after explode)
        public int count = 0;
        public float bulletSpeed = 10f;
        public float damage = 1f;
        public int pierceCount = 1;
        public float explosionRadius = 0f; // Edit if explosive weapon
        public float lifeTime = 0f;
    }

    [Header("Explosion / Shrapnel Settings")]
    public ShrapnelSettings shrapnel = new ShrapnelSettings();

}
