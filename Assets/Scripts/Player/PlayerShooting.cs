using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

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
        //Shoot();
        ShotgunShoot(5, 40f);
    }

    private void Shoot()
    {
        GameObject bulletGameObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Bullets bullet = bulletGameObject.GetComponent<Bullets>();
        bullet.Fire(firePoint.up);
    }

    private void ShotgunShoot(int pelletCount, float spreadAngle)
    {
        float halfSpread = spreadAngle * 0.5f;

        for (int i = 0; i < pelletCount; i++)
        {
            float angleOffset = Random.Range(-halfSpread, halfSpread);
            Vector2 shootDir = Quaternion.Euler(0, 0, angleOffset) * firePoint.up;

            GameObject bulletGameObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            Bullets bullet = bulletGameObject.GetComponent<Bullets>();
            bullet.Fire(shootDir);
        }
    }
}
