using UnityEngine;

[CreateAssetMenu(fileName = "CropsDatabase", menuName = "Crops/CropsDatabase")]
public class CropsDatabase : ScriptableObject
{
    [Header("Crops")]
    public CropsData[] allCrops;

    private CropsData getCropByName(CROP_NAMES name)
    {
        if (allCrops.Length <= 0) { return null; }
        
            
        
        foreach (CropsData crops in allCrops) 
        {
            if (crops.cropName == name)
            {
                return crops;
            }
        }

        return null;
    }
}
