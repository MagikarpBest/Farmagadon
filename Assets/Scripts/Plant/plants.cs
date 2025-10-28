using UnityEngine;
using UnityEngine.Tilemaps;

public class plants : MonoBehaviour
{
    private AmmoData plantAmmoData;
    private int dropAmount;
    private int posX;
    private int posY;
    public AmmoData PlantAmmoData { set { plantAmmoData = value; } }
    public int DropAmount { get { return dropAmount; } set { dropAmount = Mathf.Max(value, 1); } }
    public int PosX { set { posX = value; } }
    public int PosY { set { posY = value; } }

    public delegate void Destroyed(int posX, int posY);
    public Destroyed OnDestroyed;
    public delegate void Farmed(AmmoData plantName, int dropAmount);
    public Farmed OnFarmed;
    
    public void DestroySelf()
    {
        OnDestroyed?.Invoke(posX, posY);
        OnFarmed?.Invoke(plantAmmoData, dropAmount);
        Destroy(gameObject);
    }
}
