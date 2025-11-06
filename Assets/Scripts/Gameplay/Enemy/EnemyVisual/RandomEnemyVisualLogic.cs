using UnityEngine;
using System.Collections;

public class NormalEnemySpawnVisualLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] normalVisuals;

    private Enemy enemy;
    private int currentIndex = -1; // which visual we are using

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            Debug.LogWarning($"{name} could not find Enemy in parent!");
        }

        if (spriteRenderer == null || normalVisuals.Length == 0)
        {
            Debug.LogWarning("Missing SpriteRenderer or no visuals assigned!");
            return;
        }

        currentIndex = UnityEngine.Random.Range(0, normalVisuals.Length);
        spriteRenderer.sprite = normalVisuals[currentIndex];
    }
}
