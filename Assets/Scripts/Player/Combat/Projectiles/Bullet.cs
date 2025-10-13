using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Bullet : MonoBehaviour
{
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

    private void Explode()
    {
        // Stop movement immediately
        if (rb != null)
        {
            spriteRender.enabled = false;
            rb.linearVelocity = Vector2.zero;
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

        // Check if we should spawn shrapnel
        if (weaponData.shrapnel != null && weaponData.shrapnel.enable && weaponData.shrapnel.count > 0)
        {
            // Start a coroutine that will spawn shrapnel and THEN destroy this bullet
            StartCoroutine(SpawnShrapnelDelayed());
        }
        else
        {
            // If no shrapnel, just destroy immediately
            Destroy(gameObject);
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
            Vector3 spawnPos = transform.position + (Vector3)(direction * spawnOffset);
            GameObject bulletGO = Instantiate(shrapnel.bulletPrefab, spawnPos, rotation);

            // Get its Bullet component
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("Spawned shrapnel bullet prefab has no Bullet component!");
                continue;
            }

            //  Properly create a temporary ScriptableObject for shrapnel stats
            WeaponData shrapnelData = ScriptableObject.CreateInstance<WeaponData>();
            shrapnelData.damage = shrapnel.damage;
            shrapnelData.lifeTime = (shrapnel.lifeTime > 0 ? shrapnel.lifeTime : weaponData.lifeTime);
            shrapnelData.bulletSpeed = shrapnel.bulletSpeed;

            // Fire with its independent settings
            bullet.Fire(direction * shrapnel.bulletSpeed, shrapnelData);
        }
        Debug.Log($"Spawned {shrapnel.count} shrapnel bullets for {weaponData.weaponName}");

        // Finally destroy the original exploding bullet
        Destroy(gameObject);
    }


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

    private void HitEnemy(Collider2D other)
    {
        if (hitEnemies.Contains(other))
        {
            return;
        }

        hitEnemies.Add(other);

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(Mathf.RoundToInt(weaponData.damage));
        }


        pierceCountRemaining--;
        if (weaponData.pierceCount > 0)
        {
            if (pierceCountRemaining <= 0)
            {
                Destroy(gameObject);
            }
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
