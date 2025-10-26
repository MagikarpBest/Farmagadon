using UnityEngine;

[CreateAssetMenu(fileName = "DayCycleLevelData", menuName = "Crops/DayCycleLevelData")]
public class DayCycleLevelData : ScriptableObject
{
    [Header("Crops that will show")]
    [SerializeField] public CropEntityData[] cropsEntityList;
    public int dayCycleDuration;

}

[System.Serializable]
public class CropEntityData
{
    public CropsData cropData;
    public int cropGrowChance;
    public int cropGrowRate;
    public int cropDropAmount;
}