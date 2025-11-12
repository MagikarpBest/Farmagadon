using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerMovement playerMovement; // Make it stop while shooting
    [SerializeField] private PlayerVisualHandler playerVisualHandler;

    private WeaponSlot currentSlot;
    private float nextFireTime;
    private bool isShooting = false;

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
        
    private void HandleWeaponChange(WeaponSlot slot, WeaponSwitchDirection direction)
    {
        currentSlot = slot;
        if (slot != null && slot.weaponData != null)
        {
            Debug.Log("Switched to: " + slot.weaponData.weaponName + " Ammo: " + slot.weaponData.ammoType);
        }
        else
        {
            Debug.Log("Empty slot selected");
        }
    }

    private void HandleShoot()
    {
        if (isShooting)
        {   
            // Make player can only stop and shoot
            return;
        }

        if (!weaponInventory.CanSwitchWeapon)
        {
            // Prevent shooting during weapon swap animation
            return;
        }

        if (currentSlot == null || currentSlot.weaponData == null)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            // Fire rate check
            return;
        }
        // Reduce ammo after shoot
        if (!ammoInventory.ConsumeAmmo(currentSlot.weaponData.ammoType, 1))
        {
            Debug.Log("Out of ammo for: " + currentSlot.weaponData.weaponName);
            return;
        }

        StartCoroutine(ShootRoutine(currentSlot.weaponData));
    }

    private IEnumerator ShootRoutine(WeaponData weapon)
    {
        isShooting = true;

        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        Coroutine visualAnimation = StartCoroutine(playerVisualHandler.PlayShootAnimation());

        yield return new WaitForSeconds(0.5f);

        nextFireTime = Time.time + currentSlot.weaponData.fireRate;

        if (currentSlot.weaponData.pelletCount > 1)
        {
            ShotgunShoot(currentSlot.weaponData);
        }
        else
        {
            Shoot(currentSlot.weaponData);
        }

        // Wait for the weapon's fire rate duration
        yield return visualAnimation;
        yield return new WaitForSeconds(weapon.fireRate);

        if (playerMovement != null)
        {
            playerMovement.canMove = true;
        }

        isShooting = false; 
        Debug.Log(currentSlot.weaponData.weaponName + " fired. Ammo left: " + currentSlot.weaponData.ammoType);
    }

    private void Shoot(WeaponData weapon)
    {
        GameObject bulletGameObject = Instantiate(weapon.bulletPrefab, firePoint.position, firePoint.rotation);

        Bullet bullet = bulletGameObject.GetComponent<Bullet>();
        bullet.Fire(firePoint.up * weapon.bulletSpeed, weapon);
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
            bullet.Fire(spreadRot * Vector2.up * weapon.bulletSpeed, weapon);
        }
    }
}
