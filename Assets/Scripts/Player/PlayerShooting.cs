using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private Transform firePoint;

    private WeaponData currentWeapon;
    private float nextFireTime;


    private void OnEnable()
    {
        gameInput.OnShootAction += HandleShoot;
        gameInput.OnPreviousWeapon += HandlePreviousWeapon;
        gameInput.OnNextWeapon += HandleNextWeapon;

        weaponInventory.OnWeaponChanged += UpdateWeapon;
    }

    private void OnDisable()
    {
        gameInput.OnShootAction -= HandleShoot;
        gameInput.OnPreviousWeapon -= HandlePreviousWeapon;
        gameInput.OnNextWeapon -= HandleNextWeapon;

        weaponInventory.OnWeaponChanged -= UpdateWeapon;
    }

    private void HandleNextWeapon()
    {
        weaponInventory.NextWeapon();
    }

    private void HandlePreviousWeapon()
    {
        weaponInventory.PreviousWeapon();
    }

    private void HandleShoot()
    {
        if (currentWeapon != null)
        {
            if (Time.time < nextFireTime)
            {
                return;
            }
            nextFireTime = Time.time + currentWeapon.fireRate;

            if (currentWeapon.pelletCount > 1)
            {
                ShotgunShoot(currentWeapon);
            }
            else
            {
                Shoot(currentWeapon);
            }
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

    private void UpdateWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        if (newWeapon != null)
        {
            Debug.Log("Switched to: " + newWeapon.weaponName);
        }
        else
        {
            Debug.Log("Empty slot selected");
        }
    }

}
