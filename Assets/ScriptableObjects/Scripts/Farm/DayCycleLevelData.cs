using UnityEngine;

[CreateAssetMenu(fileName = "DayCycleLevelData", menuName = "Crops/DayCycleLevelData")]
public class DayCycleLevelData : ScriptableObject
{
    [Header("Crops that will show")]
    [SerializeField] public CropEntityData[] cropsEntityList;
    public int dayCycleDuration;

    [Header("Enemy that will show up")]
    [SerializeField] public UpcomingEnemyData upcomingEnemyDatas;

}

[System.Serializable]
public class CropEntityData
{
    public CropsData cropData;
    public int cropGrowChance;
    public int cropGrowRate;
    public int cropDropAmount;
}