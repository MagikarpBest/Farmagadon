using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.U2D;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect;
    private WeaponData weaponData;
    private Rigidbody2D rb;
    private int pierceCountRemaining;
    private SpriteRenderer spriteRender;
    private HashSet<Collider2D> hitEnemies= new HashSet<Collider2D>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRender = GetComponentInChildren<SpriteRenderer>();

        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    // Called by shooter (PlayerShooting) to launch bullet
    public void Fire(Vector2 velocity, WeaponData data)
    {
        weaponData = data;
        pierceCountRemaining = data.pierceCount;

        rb.linearVelocity = velocity;

        // Rotate sprite to match movement direction
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Destroy bullet after lifetime (why cause this can make some gun with shorter range without changing much code)
        Destroy(gameObject, data.lifeTime);
    }

    private void HitEnemy(Collider2D other)
    {
        if (hitEnemies.Contains(other))
        {
            return;
        }

        Debug.Log($"{name} hit {other.name}, pierceRemaining={pierceCountRemaining}");

        hitEnemies.Add(other);

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(Mathf.RoundToInt(weaponData.damage));
        }
        if (hitEffect != null)
        {
            GameObject instantiatedHitEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Apply any special effects (slow, split, etc.)
        HandleSpecialEffects();

        if (weaponData.pierceCount > 0)
        {
            pierceCountRemaining--;
            if (pierceCountRemaining <= 0)
            {
                Destroy(gameObject, 0.1f); // tiny delay
            }
        }
        else
        {
            Destroy(gameObject, 0.1f);
        }
    }

    private void Explode()
    {
        // Stop movement immediately
        if (rb != null)
        {
            spriteRender.enabled = false;
            rb.linearVelocity = Vector2.zero;
        }

        if (hitEffect != null)
        {
            GameObject instantiatedHitEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        // Apply explosion damage first
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weaponData.explosionRadius);

        // Check every enemy in zone and reduce HP
        foreach (var hit in hits)
        {

            if (hit.CompareTag("Enemy"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(Mathf.RoundToInt(weaponData.damage));
                }
            }
        }

        HandleSpecialEffects();
        if (weaponData.pierceCount > 0)
        {
            pierceCountRemaining--;
            if (pierceCountRemaining <= 0)
            {
                Destroy(gameObject, 0.1f); // tiny delay
            }
        }
        else
        {
            Destroy(gameObject, 0.1f);
        }

    }

    private IEnumerator ReenableCollision(Collider2D collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    #region Special Effects
    private void HandleSpecialEffects()
    {
        if (weaponData.shrapnel != null && weaponData.shrapnel.enable) 
        {
            StartCoroutine(SpawnShrapnelDelayed());
        }   
        if (weaponData.split != null && weaponData.split.enable)
        {
            StartCoroutine(ShootSplitBullets());
        }
        if (weaponData.slowEffect != null && weaponData.slowEffect.enable)
        {
            StartCoroutine(ApplySlowEffect());
        }
        if (weaponData.firecracker != null && weaponData.firecracker.enable)
        {
            Debug.Log("fired firecracker");
            StartCoroutine(FireCracker());
        }
    }

    private IEnumerator SpawnShrapnelDelayed()
    {
        var shrapnel = weaponData.shrapnel;

        // Validate setup
        if (!shrapnel.enable || shrapnel.count <= 0 || shrapnel.bulletPrefab == null)
        {
            Debug.LogWarning($"SpawnShrapnel: invalid config for {weaponData.weaponName}");
            Destroy(gameObject);
            yield break;
        }

        // Optional delay so the shrapnel doesn’t hit enemies instantly
        yield return new WaitForSeconds(0.05f);

        // Calculate spread angles
        float angleStep = 360f / shrapnel.count;
        // So bullets don’t overlap exactly at center
        float spawnOffset = 0.5f;

        // Loop for each shrapnel bullet
        for (int i = 0; i < shrapnel.count; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            Vector2 direction = rotation * Vector2.up;

            // Spawn bullet prefab
            Vector3 spawnPos = transform.position + (Vector3)(direction * spawnOffset) + new Vector3(0f, 0.5f, 0f);
            GameObject bulletGO = Instantiate(shrapnel.bulletPrefab, spawnPos, rotation);

            // Get its Bullet component
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("Spawned shrapnel bullet prefab has no Bullet component!");
                continue;
            }

            //  Properly create a temporary ScriptableObject for shrapnel stats
            bullet.Fire(direction.normalized * shrapnel.shrapnelWeaponData.bulletSpeed, shrapnel.shrapnelWeaponData);

            // Immediately ignore colliders after instantiation
            Collider2D collider = bullet.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
                bullet.StartCoroutine(ReenableCollision(collider, 0.15f));
            }
        }
        Debug.Log($"Spawned {shrapnel.count} shrapnel bullets for {weaponData.weaponName}");

        // Finally destroy the original exploding bullet
        Destroy(gameObject);
    }

    private IEnumerator ShootSplitBullets()
    {
        var split = weaponData.split;
        if (split.bulletPrefab == null)
        {
            Debug.LogWarning($"Split bullet prefab missing for {weaponData.weaponName}");
            yield return null;
        }

        float halfAngle = split.spreadAngle * 0.5f;

        // Spawn left/right bullets
        SplitBulletLogic(halfAngle);
        SplitBulletLogic(-halfAngle);
    }

    private void SplitBulletLogic(float angle)
    {
        var split = weaponData.split;
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, transform.eulerAngles.z + angle);
        Vector2 direction = rotation * Vector2.up;

        GameObject bulletGO = Instantiate(split.bulletPrefab, transform.position+new Vector3(0f,0.3f,0f) , rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet == null)
        {
            Debug.LogError("Split bullet prefab has no Bullet script!");
            return;
        }

        // Make temp weapon data for split bullets
        bullet.Fire(direction.normalized * split.splitWeaponData.bulletSpeed, split.splitWeaponData);

        // Immediately ignore colliders after instantiation
        Collider2D collider = bullet.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            bullet.StartCoroutine(ReenableCollision(collider, 0.15f));
        }
    }

    private IEnumerator ApplySlowEffect()
    {
        var slow = weaponData.slowEffect;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weaponData.explosionRadius);

        // Check every enemy in zone and reduce HP
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.ApplySlow(slow.slowAmount, slow.duration);
                    Debug.Log($"Applied slow {slow.slowAmount * 100f}% for {slow.duration}s to {enemy.name}");
                }
            }
        }
        yield return null;
    }
    private IEnumerator FireCracker()
    {
        Debug.Log("successfully fired firecracker ammo");
        var fc = weaponData.firecracker;
        yield return new WaitForSeconds(0.05f);

        Vector2 forward = transform.up; // now base directions on forward

        float angleStep = fc.backSpreadAngle / (fc.pelletCount - 1);
        float startAngle = -fc.backSpreadAngle / 2f;

        for (int i = 0; i < fc.pelletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            // rotate forward vector by angle
            Vector2 dir = Quaternion.Euler(0f, 0f, angle) * forward;

            Vector3 spawnPos = transform.position + (Vector3)(forward * fc.backwardOffset);

            // Instantiate with rotation matching dir
            Quaternion rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);
            GameObject bulletGO = Instantiate(fc.bulletPrefab, spawnPos, rotation);

            Bullet bullet = bulletGO.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("Firecracker bullet prefab missing Bullet script!");
                continue;
            }

            // Fire bullet
            bullet.Fire(dir.normalized * fc.firecrackerWeaponData.bulletSpeed, fc.firecrackerWeaponData);

            // Disable collider briefly
            Collider2D collider = bullet.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
                bullet.StartCoroutine(ReenableCollision(collider, 0.15f));
            }
        }
    }



    #endregion Special Effects

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        if (weaponData.explosionRadius > 0)
        {
            Explode();
        }
        else
        {
            HitEnemy(other);
        }
    }


    private void OnDrawGizmos()
    {
        if (weaponData == null)
        {
            return;
        }

        // Explosive range debug tools
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, weaponData.explosionRadius);
    }
}
