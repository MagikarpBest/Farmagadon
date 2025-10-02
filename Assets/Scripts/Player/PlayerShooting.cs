using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private Transform firePoint;

    private WeaponSlot currentSlot;
    private float nextFireTime;


    private void OnEnable()
    {
        gameInput.OnShootAction += HandleShoot;
        gameInput.OnPreviousWeapon += HandlePreviousWeapon;
        gameInput.OnNextWeapon += HandleNextWeapon;

        weaponInventory.OnWeaponChanged += HandleWeaponChange;
    }

    private void OnDisable()
    {
        gameInput.OnShootAction -= HandleShoot;
        gameInput.OnPreviousWeapon -= HandlePreviousWeapon;
        gameInput.OnNextWeapon -= HandleNextWeapon;

        weaponInventory.OnWeaponChanged -= HandleWeaponChange;
    }

    private void HandleNextWeapon()
    {
        weaponInventory.NextWeapon();
    }

    private void HandlePreviousWeapon()
    {
        weaponInventory.PreviousWeapon();
    }
    private void HandleWeaponChange(WeaponSlot slot)
    {
        currentSlot = slot;
        if (slot != null && slot.weaponData != null)
        {
            Debug.Log("Switched to: " + slot.weaponData.weaponName + " Ammo: " + slot.currentAmmo);
        }
        else
        {
            Debug.Log("Empty slot selected");
        }
    }

    private void HandleShoot()
    {
        if (currentSlot == null || currentSlot.weaponData == null) 
        {
            return;
        }

        // Fire rate check
        if (Time.time < nextFireTime)
        {
            return;
        }

        // Reduce ammo after shoot
        if (!weaponInventory.ConsumeAmmo(1)) 
        {
            Debug.Log("Out of ammo for: " + currentSlot.weaponData.weaponName);
            return;
        }

        nextFireTime = Time.time + currentSlot.weaponData.fireRate;

        if (currentSlot.weaponData.pelletCount > 1)
        {
            ShotgunShoot(currentSlot.weaponData);
        }
        else
        {
            Shoot(currentSlot.weaponData);
        }

        Debug.Log(currentSlot.weaponData.weaponName + " fired. Ammo left: " + currentSlot.currentAmmo);

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
