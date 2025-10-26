using UnityEngine;
using UnityEngine.Tilemaps;

public class plants : MonoBehaviour
{
    private AmmoData plantName;
    private int dropAmount;

    public AmmoData PlantName { set { plantName = value; } }
    public int DropAmount { get { return dropAmount; } set { dropAmount = Mathf.Max(value, 1); } }

    public delegate void Destroyed(Vector3 pos);
    public Destroyed OnDestroyed;
    public delegate void Farmed(AmmoData plantName, int dropAmount);
    public Farmed OnFarmed;
    
    public void DestroySelf()
    {
        OnDestroyed?.Invoke(gameObject.transform.position);
        OnFarmed?.Invoke(plantName, dropAmount);
        Destroy(gameObject);
    }
}
