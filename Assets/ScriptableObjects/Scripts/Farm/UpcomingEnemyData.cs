using UnityEngine;

[CreateAssetMenu(fileName = "UpcomingEnemyData", menuName = "Crops/UpcomingEnemyData")]
public class UpcomingEnemyData : ScriptableObject
{
    [Header("Place only maximum 3")]
    public UpcomersData[] upcomingEnemyDatas;
}

[System.Serializable]
public class UpcomersData
{
    public Sprite enemySprite;
    public ENEMY_DENSITY density;
    public Sprite enemyWeakness;
}


