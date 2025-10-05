using UnityEngine;

[CreateAssetMenu(menuName = "Weapons")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public AmmoData ammoType;
    public float lifeTime = 0f;
    public float bulletSpeed = 10f;
    public float damage = 1f;
    public int pierceCount = 0;
    public float explosionRadius = 0f; // Edit if explosive weapon


    [Header("Fire Settings")]
    public float fireRate = 0.2f;
    public int pelletCount = 1; // Set to more than 1 if shotgun
    public float spread = 0; // Set to more than 1 if shotgun
}
