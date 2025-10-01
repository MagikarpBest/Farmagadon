using UnityEngine;

[CreateAssetMenu(menuName = "Weapons")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float lifeTime = 0f;
    public float bulletSpeed = 10f;
    public float damage = 1f;
    public int pierceCount = 0;
    public float explosionRadius = 0f; //edit if explosive weapon


    [Header("Fire Settings")]
    public float fireRate = 0.2f;
    public int pelletCount = 1; //set to more than 1 if shotgun
    public float spread = 0; //set to more than 1 if shotgun
}
