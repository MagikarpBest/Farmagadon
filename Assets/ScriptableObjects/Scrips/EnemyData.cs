using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Data", fileName = "NewEnemy")]
public class NewMonoBehaviourScript : ScriptableObject
{
    [Header("General Info")]
    public GameObject prefab; // Enemy prefab

    [Header("Stats")]
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public float damage = 1f;
}
