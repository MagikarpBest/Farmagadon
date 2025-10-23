using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyMatchupDatabase", menuName = "Crops/EnemyMatchupDatabase")]
public class EnemyMatchupDatabase : ScriptableObject
{
    [Header("Matchup Database List")]
    public EnemyMatchupData[] enemyMatchupDatas;
    
}


