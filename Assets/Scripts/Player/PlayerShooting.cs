using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private WeaponData[] weapons;
    [SerializeField] private Transform firePoint;

    private int currentWeaponIndex = 0;
    private float nextFireTime;


    private void OnEnable()
    {
        gameInput.OnShootAction += HandleShoot;
        gameInput.OnPreviousWeapon += HandlePreviousWeapon;
        gameInput.OnNextWeapon += HandleNextWeapon;
    }

    private void OnDisable()
    {
        gameInput.OnShootAction -= HandleShoot;
        gameInput.OnPreviousWeapon -= HandlePreviousWeapon;
        gameInput.OnNextWeapon -= HandleNextWeapon;
    }

    private void HandleNextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        Debug.Log("Switched to: " + weapons[currentWeaponIndex].weaponName);
    }

    private void HandlePreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weapons.Length - 1;
        }
        Debug.Log("Switched to: " + weapons[currentWeaponIndex].weaponName);
    }

    private void HandleShoot()
    {
        //Switch weapon with Q and E
        WeaponData weapon = weapons[currentWeaponIndex];

        if (Time.time < nextFireTime)
        {
            return;
        }

        nextFireTime = Time.time + weapon.fireRate;
        if (weapon.pelletCount > 1)
        {
            ShotgunShoot(weapon);
        }
        else
        {
            Shoot(weapon);
        }
        
    }

    private void Shoot(WeaponData weapon)
    {
        GameObject bulletGameObject = Instantiate(weapon.bulletPrefab, firePoint.position, firePoint.rotation);

        Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.Fire(firePoint.up * weapon.bulletSpeed, weapon.damage, weapon.explosionRadius, weapon.lifeTime, weapon.pierceCount);
    }

    private void ShotgunShoot(WeaponData weapon)
    {
        //Dont ask me what are these equation i also dk i just chatgpt.
        float halfSpread = weapon.spread * 0.5f;

        for (int i = 0; i < weapon.pelletCount; i++)
        {
            float angleOffset = Random.Range(-halfSpread, halfSpread);
            Quaternion spreadRot = firePoint.rotation * Quaternion.Euler(0, 0, angleOffset);

            GameObject bulletGameObject = Instantiate(weapon.bulletPrefab, firePoint.position, spreadRot);
            Bullet bullet = bulletGameObject.GetComponent<Bullet>();
            bullet.Fire(spreadRot * Vector2.up * weapon.bulletSpeed, weapon.damage, weapon.explosionRadius, weapon.lifeTime, weapon.pierceCount);
        }
    }
    
}
