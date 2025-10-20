using UnityEngine;

[CreateAssetMenu(menuName = "Ammo")] 
public class AmmoData : ScriptableObject
{
    public string ammoID;
    public string ammoName;
    public Sprite icon;
}
