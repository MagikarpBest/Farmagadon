using UnityEngine;


public class RandomEnemyVisualLogic : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] allVisuals;

    private void Awake()
    {
        if (spriteRenderer == null || allVisuals.Length == 0)
        {
            Debug.LogWarning("Missing SpriteRenderer or no visuals assigned!");
            return;
        }

        int index = UnityEngine.Random.Range(0, allVisuals.Length);
        spriteRenderer.sprite = allVisuals[index];
    }
}
