using UnityEngine;
using System.Collections.Generic;
public class EnemyMatchupData
{
    public Dictionary<ENEMY_WEAKNESS, string> enemyMatchupDict = new Dictionary<ENEMY_WEAKNESS, string>();
    public Dictionary<ENEMY_WEAKNESS, string> bulletPathDict = new Dictionary<ENEMY_WEAKNESS, string>();
    public EnemyMatchupData()
    {
        enemyMatchupDict.Add(ENEMY_WEAKNESS.Rice, "Enemy/SmallAnt");
        bulletPathDict.Add(ENEMY_WEAKNESS.Rice, "ammo/Rice");
    }
}


