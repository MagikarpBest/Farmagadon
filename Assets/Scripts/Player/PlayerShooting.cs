using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootSpawnPosition;
    [SerializeField] private float bulletSpeed = 10f;

    private void OnEnable()
    {
        gameInput.OnShootAction += HandleShoot;
    }

    private void OnDisable()
    {
        gameInput.OnShootAction -= HandleShoot;
    }

    private void HandleShoot(object sender, System.EventArgs e)
    {
        Shoot();
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootSpawnPosition.position, shootSpawnPosition.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootSpawnPosition.up * bulletSpeed;
    }
}
