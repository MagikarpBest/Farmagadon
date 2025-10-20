using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Data", fileName = "NewEnemy")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public string enemyName;        // Just for debug purposes i think
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int damage = 1;
    public float attackInterval = 0f;

    [Header("Attributes")]
    public ENEMY_WEAKNESS enemyWeakness = ENEMY_WEAKNESS.Rice;
}
