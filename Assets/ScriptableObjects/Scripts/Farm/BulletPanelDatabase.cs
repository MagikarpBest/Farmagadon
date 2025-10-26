using UnityEngine;

[CreateAssetMenu(fileName = "BulletPanelDatabase", menuName = "Crops/BulletPanelDatabase")]
public class BulletPanelDatabase : ScriptableObject
{
    public BulletPanelData[] bulletPanelDatas;

    public BulletPanelData GetBulletPanelDataByWeaponID(string weaponID)
    {
        foreach (BulletPanelData data in bulletPanelDatas) 
        {
            if (data.weaponID == weaponID)
            {
                return data;
            }
        }

        return bulletPanelDatas[0];
    }
}
