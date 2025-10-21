using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMatchupData", menuName = "Crops/EnemyMatchupData")]
public class EnemyMatchupData : ScriptableObject
{
    [Header("Enemy")]
    public EnemyData enemy;
    [Header("Ammo")]
    public AmmoData ammo;
}
